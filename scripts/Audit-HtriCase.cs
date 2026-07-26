using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

internal static class AuditHtriCase
{
    private static readonly CultureInfo Ci = CultureInfo.InvariantCulture;

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

    private static string Csv(string value)
    {
        if (value == null) return "";
        return "\"" + value.Replace("\"", "\"\"") + "\"";
    }

    private static string Optional(Func<object> getter)
    {
        try
        {
            object value = getter();
            if (value == null) return "";
            if (value is double) return ((double)value).ToString("G17", Ci);
            return Convert.ToString(value, Ci);
        }
        catch
        {
            return "";
        }
    }

    private static string Sha256(string path)
    {
        using (var stream = File.OpenRead(path))
        using (var sha = SHA256.Create())
        {
            return BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", "");
        }
    }

    private static void Value(StreamWriter writer, string objectName, dynamic obj, short key, string name)
    {
        dynamic data = obj.GetHtriObjData();
        writer.WriteLine(string.Join(",", new[]
        {
            Csv(objectName),
            key.ToString(Ci),
            Csv(name),
            Csv(Safe(() => obj.GetDouble(key))),
            Csv(Optional(() => obj.GetShort(key))),
            Csv(Optional(() => obj.GetString(key))),
            Csv(Safe(() => data.GetUOMString(key)))
        }));
    }

    [STAThread]
    private static int Main(string[] args)
    {
        if (args.Length != 3)
        {
            Console.Error.WriteLine("Usage: Audit-HtriCase.exe <input.htri> <audit.csv> <input-report.txt>");
            return 2;
        }

        string inputPath = Path.GetFullPath(args[0]);
        string auditPath = Path.GetFullPath(args[1]);
        string reportPath = Path.GetFullPath(args[2]);
        Directory.CreateDirectory(Path.GetDirectoryName(auditPath));
        Directory.CreateDirectory(Path.GetDirectoryName(reportPath));

        object networkObject = null;
        try
        {
            Type networkType = Type.GetTypeFromProgID("HtriCalc.HeatExchangerNetwork", true);
            networkObject = Activator.CreateInstance(networkType);
            dynamic network = networkObject;
            network.EnableReturnDefault(1);
            network.EnableSaveOutputData(1);
            if (Convert.ToInt32(network.OpenFile(inputPath)) == 0)
                throw new InvalidOperationException(Safe(() => network.GetLastErrorMsgEx()));

            dynamic unit = network.GetHeatTransferUnit((short)0);
            unit.WriteInputFile(reportPath, (short)0);

            using (var writer = new StreamWriter(auditPath, false, new UTF8Encoding(true)))
            {
                writer.WriteLine("object,key,name,double_value,short_value,string_value,uom");
                writer.WriteLine("META,0,Source,,," + Csv(inputPath) + ",");
                writer.WriteLine("META,0,SourceSha256,,," + Sha256(inputPath) + ",");
                writer.WriteLine("META,0,UnitType,,," + Csv(Safe(() => unit.GetUnitTypeName())) + ",");
                writer.WriteLine("META,0,UnitCount," + Safe(() => network.GetHeatTransferUnitCount()) + ",,,");
                writer.WriteLine("META,0,StreamCount," + Safe(() => network.GetMaterialStreamCount()) + ",,,");

                dynamic exchanger = unit.GetHeatExchanger();
                dynamic input = exchanger.GetInputHeatExchanger();
                dynamic design = input.GetExchangerDesignCriteria();
                dynamic shellDesign = design.GetShellsideDesign();
                dynamic tubeDesign = design.GetTubesideDesign();
                dynamic shellProcess = shellDesign.GetProcessConditions();
                dynamic tubeProcess = tubeDesign.GetProcessConditions();

                Value(writer, "UNIT", input, 2525, "FlowDirection");
                Value(writer, "UNIT", input, 2538, "ShellFouling");
                Value(writer, "UNIT", input, 2539, "TubeFouling");
                Value(writer, "DESIGN", design, 1404, "HotFluidAllocation");
                Value(writer, "DESIGN", design, 1406, "ModeOfOperation");
                Value(writer, "DESIGN", design, 1408, "VaporizationOption");
                Value(writer, "DESIGN", design, 1409, "NucleateBoiling");
                Value(writer, "DESIGN", design, 1410, "ConvectiveBoiling");

                foreach (var item in new[]
                {
                    new { Key = (short)2714, Name = "Pressure" },
                    new { Key = (short)2715, Name = "Temperature" },
                    new { Key = (short)2719, Name = "FilmCoefficient" },
                    new { Key = (short)2722, Name = "FilmCoefMultiplier" },
                    new { Key = (short)2723, Name = "PressureDropMultiplier" },
                    new { Key = (short)2725, Name = "AllowableDeltaP" }
                }) Value(writer, "SHELL_DESIGN", shellDesign, item.Key, item.Name);

                foreach (var item in new[]
                {
                    new { Key = (short)3332, Name = "Pressure" },
                    new { Key = (short)3333, Name = "Temperature" },
                    new { Key = (short)3338, Name = "FilmCoefficient" },
                    new { Key = (short)3341, Name = "FilmCoefMultiplier" },
                    new { Key = (short)3342, Name = "PressureDropMultiplier" },
                    new { Key = (short)3344, Name = "AllowableDeltaP" }
                }) Value(writer, "TUBE_DESIGN", tubeDesign, item.Key, item.Name);

                foreach (var item in new[]
                {
                    new { Key = (short)2400, Name = "PhaseCondition" },
                    new { Key = (short)2401, Name = "InletTemperature" },
                    new { Key = (short)2402, Name = "OutletTemperature" },
                    new { Key = (short)2403, Name = "InletVaporFraction" },
                    new { Key = (short)2404, Name = "OutletVaporFraction" },
                    new { Key = (short)2405, Name = "FlowRate" },
                    new { Key = (short)2406, Name = "UseOutletPressure" },
                    new { Key = (short)2407, Name = "OutletPressure" }
                })
                {
                    Value(writer, "SHELL_PROCESS", shellProcess, item.Key, item.Name);
                    Value(writer, "TUBE_PROCESS", tubeProcess, item.Key, item.Name);
                }

                dynamic assembly = input.GetExchangerAssembly((short)0);
                dynamic shell = assembly.GetExchangerShell();
                dynamic bundle = assembly.GetExchangerBundle();
                dynamic tubeType = bundle.GetTubeType((short)0);
                foreach (var item in new[]
                {
                    new { Key = (short)1632, Name = "InsideDiameter" },
                    new { Key = (short)1661, Name = "Length" }
                }) Value(writer, "SHELL", shell, item.Key, item.Name);
                foreach (var item in new[]
                {
                    new { Key = (short)1300, Name = "TubePassesPerShell" },
                    new { Key = (short)1301, Name = "Tubes" },
                    new { Key = (short)1302, Name = "Baffles" },
                    new { Key = (short)1321, Name = "OuterTubeLimit" },
                    new { Key = (short)1327, Name = "BaffleSpacing" },
                    new { Key = (short)1334, Name = "TubeStraightLength" },
                    new { Key = (short)1339, Name = "TubeTotalLength" }
                }) Value(writer, "BUNDLE", bundle, item.Key, item.Name);
                foreach (var item in new[]
                {
                    new { Key = (short)3504, Name = "HeatedLength" },
                    new { Key = (short)3506, Name = "EffectiveLength" },
                    new { Key = (short)3508, Name = "InsideDiameter" },
                    new { Key = (short)3509, Name = "OutsideDiameter" },
                    new { Key = (short)3511, Name = "Conductivity" },
                    new { Key = (short)3513, Name = "ThermalResistance" },
                    new { Key = (short)3515, Name = "WallThickness" },
                    new { Key = (short)3517, Name = "TotalLength" },
                    new { Key = (short)3537, Name = "TubeCount" }
                }) Value(writer, "TUBETYPE", tubeType, item.Key, item.Name);

                short streamCount = Convert.ToInt16(network.GetMaterialStreamCount());
                for (short streamIndex = 0; streamIndex < streamCount; streamIndex++)
                {
                    dynamic stream = network.GetMaterialStream(streamIndex);
                    dynamic source = stream.GetInputStream();
                    string prefix = "STREAM" + streamIndex.ToString(Ci);
                    Value(writer, prefix, stream, 2150, "Name");
                    foreach (var item in new[]
                    {
                        new { Key = (short)2104, Name = "PhaseChange" },
                        new { Key = (short)2106, Name = "PropertyOption" },
                        new { Key = (short)2107, Name = "HROption" },
                        new { Key = (short)2121, Name = "HRType" },
                        new { Key = (short)2122, Name = "HRFlowRate" },
                        new { Key = (short)2134, Name = "StartTemperature" },
                        new { Key = (short)2135, Name = "EndTemperature" },
                        new { Key = (short)2140, Name = "StartPressure" },
                        new { Key = (short)2141, Name = "EndPressure" },
                        new { Key = (short)2142, Name = "EndPressureDrop" }
                    }) Value(writer, prefix, source, item.Key, item.Name);

                    short curveCount = Convert.ToInt16(source.GetHeatReleaseCurveCount());
                    for (short curveIndex = 0; curveIndex < curveCount; curveIndex++)
                    {
                        dynamic curve = source.GetHeatReleaseCurve(curveIndex);
                        short enabled;
                        try { enabled = Convert.ToInt16(curve.GetShort((short)1958)); }
                        catch { continue; }
                        if (enabled <= 0) continue;
                        Value(writer, prefix + "_CURVE" + curveIndex.ToString(Ci), curve, 1951, "Phase");
                        Value(writer, prefix + "_CURVE" + curveIndex.ToString(Ci), curve, 1954, "Isobaric");
                        Value(writer, prefix + "_CURVE" + curveIndex.ToString(Ci), curve, 1958, "Enabled");
                        short pointCount = Convert.ToInt16(curve.GetHeatReleasePointCount());
                        if (pointCount <= 0) continue;
                        short[] pointIndexes = pointCount == 1 ? new short[] { 0 } : new short[] { 0, (short)(pointCount - 1) };
                        foreach (short pointIndex in pointIndexes)
                        {
                            dynamic point = curve.GetHeatReleasePoint(pointIndex);
                            dynamic thermo = point.GetHRThermodynamicProperties();
                            string pointPrefix = prefix + "_CURVE" + curveIndex.ToString(Ci) + "_POINT" + pointIndex.ToString(Ci);
                            Value(writer, pointPrefix, thermo, 2000, "Temperature");
                            Value(writer, pointPrefix, thermo, 2001, "Pressure");
                            Value(writer, pointPrefix, thermo, 2003, "SpecificEnthalpyMass");
                            Value(writer, pointPrefix, thermo, 2005, "VaporMassFraction");
                            Value(writer, pointPrefix, thermo, 2006, "MassDuty");
                        }
                    }
                }
            }

            Console.WriteLine("AUDIT=" + auditPath);
            Console.WriteLine("INPUT_REPORT=" + reportPath);
            Console.Out.Flush();
            Environment.Exit(0);
            return 0;
        }
        catch (Exception ex)
        {
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
