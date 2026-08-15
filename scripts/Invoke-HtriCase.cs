using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

internal static class InvokeHtriCase
{
    private static readonly CultureInfo Ci = CultureInfo.InvariantCulture;
    private const double MissingDouble = -9.9E23;

    private static void Log(StreamWriter writer, string message)
    {
        writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", Ci) + " " + message);
        writer.Flush();
    }

    private static string Safe(Func<object> getter)
    {
        try
        {
            object value = getter();
            if (value == null) return "<null>";
            if (value is double) return ((double)value).ToString("G17", Ci);
            return Convert.ToString(value, Ci);
        }
        catch (Exception ex)
        {
            return "<ERR:" + ex.GetBaseException().Message.Replace(',', ';') + ">";
        }
    }

    private static double ReadDouble(Func<double> getter)
    {
        try
        {
            double value = getter();
            return value <= MissingDouble ? double.NaN : value;
        }
        catch
        {
            return double.NaN;
        }
    }

    private static string Number(double value)
    {
        return double.IsNaN(value) || double.IsInfinity(value) ? "" : value.ToString("G17", Ci);
    }

    private static string Sha256(string path)
    {
        using (var stream = File.OpenRead(path))
        using (var sha = SHA256.Create())
        {
            return BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", "");
        }
    }

    private static void DumpMessages(StreamWriter writer, dynamic messages, string label)
    {
        short count = Convert.ToInt16(messages.GetCount());
        Log(writer, label + "_COUNT=" + count.ToString(Ci));
        for (short index = 0; index < count; index++)
        {
            Log(writer, label + "[" + index.ToString(Ci) + "]"
                + " SEV=" + Safe(() => messages.GetSeverity(index))
                + " TYPE=" + Safe(() => messages.GetType(index))
                + " NO=" + Safe(() => messages.GetNumber(index))
                + " TEXT=" + Safe(() => messages.GetText(index)));
        }
    }

    private static void WriteResult(StreamWriter writer, string key, double raw, string rawUnit, double si, string siUnit)
    {
        writer.WriteLine(key + "," + Number(raw) + "," + rawUnit + "," + Number(si) + "," + siUnit);
    }

    [STAThread]
    private static int Main(string[] args)
    {
        if (args.Length != 4)
        {
            Console.Error.WriteLine("Usage: Invoke-HtriCase.exe <input.htri> <saved-copy.htri> <log.txt> <results.csv>");
            return 2;
        }

        string inputPath = Path.GetFullPath(args[0]);
        string savedPath = Path.GetFullPath(args[1]);
        string logPath = Path.GetFullPath(args[2]);
        string resultPath = Path.GetFullPath(args[3]);
        if (string.Equals(inputPath, savedPath, StringComparison.OrdinalIgnoreCase))
        {
            Console.Error.WriteLine("Refusing to overwrite the source model.");
            return 2;
        }

        Directory.CreateDirectory(Path.GetDirectoryName(savedPath));
        Directory.CreateDirectory(Path.GetDirectoryName(logPath));
        Directory.CreateDirectory(Path.GetDirectoryName(resultPath));

        object networkObject = null;
        using (var log = new StreamWriter(logPath, false, new UTF8Encoding(true)))
        {
            try
            {
                Log(log, "SOURCE=" + inputPath);
                Log(log, "SOURCE_SHA256=" + Sha256(inputPath));
                Log(log, "CREATE_COM_BEGIN");
                Type networkType = Type.GetTypeFromProgID("HtriCalc.HeatExchangerNetwork", true);
                networkObject = Activator.CreateInstance(networkType);
                dynamic network = networkObject;
                Log(log, "CREATE_COM_OK");

                network.EnableReturnDefault(1);
                network.EnableSaveOutputData(1);
                int opened = Convert.ToInt32(network.OpenFile(inputPath));
                Log(log, "OPEN_RESULT=" + opened.ToString(Ci));
                if (opened == 0) throw new InvalidOperationException(Safe(() => network.GetLastErrorMsgEx()));

                Log(log, "UNIT_COUNT=" + Safe(() => network.GetHeatTransferUnitCount()));
                Log(log, "STREAM_COUNT=" + Safe(() => network.GetMaterialStreamCount()));
                dynamic unit = network.GetHeatTransferUnit((short)0);
                Log(log, "UNIT_TYPE=" + Safe(() => unit.GetUnitTypeName()));

                short check = Convert.ToInt16(network.CheckNetworkForRun());
                Log(log, "CHECK=" + check.ToString(Ci));
                DumpMessages(log, network.GetRunCheckMessageList(), "CHECKMSG");
                if (check != 0) throw new InvalidOperationException("HTRI input check failed.");

                network.SetRunDialog(0, 0, "AI-HTRI-SKILL controlled rerun", 0);
                Log(log, "RUN_BEGIN");
                int run = Convert.ToInt32(network.Run(0, 0));
                Log(log, "RUN=" + run.ToString(Ci));
                DumpMessages(log, network.GetMessageList(), "RUNMSG");

                dynamic exchanger = unit.GetHeatExchanger();
                dynamic output = exchanger.GetOutputHeatExchanger();
                dynamic assembly = output.GetExchangerAssembly((short)0);
                dynamic performance = assembly.GetExchangerPerformance();
                dynamic shell = performance.GetShellsidePerformance();
                dynamic tube = performance.GetTubesidePerformance();

                double duty = ReadDouble(() => (double)performance.GetDouble((short)1582));
                double grossArea = ReadDouble(() => (double)performance.GetDouble((short)1583));
                double effectiveArea = ReadDouble(() => (double)performance.GetDouble((short)1584));
                double emtd = ReadDouble(() => (double)performance.GetDouble((short)1585));
                double actualU = ReadDouble(() => (double)performance.GetDouble((short)1587));
                double overdesign = ReadDouble(() => (double)performance.GetDouble((short)1588));
                double shellFilm = ReadDouble(() => (double)shell.GetDouble((short)2808));
                double tubeFilm = ReadDouble(() => (double)tube.GetDouble((short)3378));
                double tubeDp = ReadDouble(() => (double)tube.GetDouble((short)3377));

                using (var results = new StreamWriter(resultPath, false, new UTF8Encoding(true)))
                {
                    results.WriteLine("key,raw_value,raw_unit,si_value,si_unit");
                    results.WriteLine("run_code," + run.ToString(Ci) + ",code," + run.ToString(Ci) + ",code");
                    WriteResult(results, "duty", duty, "MMBtu/h", duty * 0.29307107, "MW");
                    WriteResult(results, "gross_area", grossArea, "ft2", grossArea * 0.09290304, "m2");
                    WriteResult(results, "effective_area", effectiveArea, "ft2", effectiveArea * 0.09290304, "m2");
                    WriteResult(results, "emtd", emtd, "degF-difference", emtd * 5.0 / 9.0, "K");
                    WriteResult(results, "actual_u", actualU, "Btu/(h.ft2.F)", actualU * 5.67826334, "W/(m2.K)");
                    WriteResult(results, "overdesign", overdesign, "percent", overdesign, "percent");
                    WriteResult(results, "shell_film", shellFilm, "Btu/(h.ft2.F)", shellFilm * 5.67826334, "W/(m2.K)");
                    WriteResult(results, "tube_film", tubeFilm, "Btu/(h.ft2.F)", tubeFilm * 5.67826334, "W/(m2.K)");
                    WriteResult(results, "tube_dp", tubeDp, "psi", tubeDp * 6.894757293, "kPa");
                }
                Log(log, "RESULTS_WRITTEN=" + resultPath);

                int saved = Convert.ToInt32(network.SaveFile(savedPath));
                Log(log, "SAVE_RESULT=" + saved.ToString(Ci));
                if (saved == 0) throw new InvalidOperationException("HTRI SaveFile failed.");
                Log(log, "SAVED_SHA256=" + Sha256(savedPath));
                Log(log, "COMPLETE");

                Console.WriteLine("LOG=" + logPath);
                Console.WriteLine("RESULTS=" + resultPath);
                Console.WriteLine("MODEL=" + savedPath);
                Console.Out.Flush();
                Environment.Exit(run == 3 ? 0 : 4);
                return 0;
            }
            catch (Exception ex)
            {
                Log(log, "ERROR=" + ex.ToString());
                Console.Error.WriteLine(ex.ToString());
                Console.Error.Flush();
                Environment.Exit(1);
                return 1;
            }
            finally
            {
                if (networkObject != null && Marshal.IsComObject(networkObject))
                {
                    Marshal.FinalReleaseComObject(networkObject);
                }
            }
        }
    }
}
