[CmdletBinding()]
param(
    [switch]$ProbeCom,
    [switch]$AsJson
)

$ErrorActionPreference = 'Stop'

$candidateRoots = @(
    'D:\Program Files (x86)\HTRI',
    'C:\Program Files\HTRI',
    'C:\Program Files (x86)\HTRI'
)

$executables = foreach ($root in $candidateRoots) {
    if (Test-Path -LiteralPath $root) {
        Get-ChildItem -LiteralPath $root -Filter 'HtriGui.exe' -File -Recurse -ErrorAction SilentlyContinue
    }
}

$installations = foreach ($exe in $executables) {
    $version = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($exe.FullName)
    $sample = Get-ChildItem -LiteralPath $exe.Directory.FullName -Filter 'Xist_Sample.htri' -File -Recurse -ErrorAction SilentlyContinue | Select-Object -First 1
    [pscustomobject]@{
        Executable = $exe.FullName
        FileVersion = $version.FileVersion
        ProductVersion = $version.ProductVersion
        SampleCase = if ($sample) { $sample.FullName } else { $null }
    }
}

$progId = 'HtriCalc.HeatExchangerNetwork'
$comType = [Type]::GetTypeFromProgID($progId, $false)
$comAvailable = $null -ne $comType
$comProbeSucceeded = $null
$comProbeError = $null

if ($ProbeCom) {
    if (-not $comAvailable) {
        $comProbeSucceeded = $false
        $comProbeError = "COM ProgID is not registered: $progId"
    }
    else {
        $network = $null
        try {
            $network = [Activator]::CreateInstance($comType)
            [void]$network.EnableReturnDefault(1)
            $comProbeSucceeded = $true
        }
        catch {
            $comProbeSucceeded = $false
            $comProbeError = $_.Exception.GetBaseException().Message
        }
        finally {
            if ($null -ne $network -and [Runtime.InteropServices.Marshal]::IsComObject($network)) {
                [void][Runtime.InteropServices.Marshal]::FinalReleaseComObject($network)
            }
        }
    }
}

$result = [pscustomobject]@{
    Timestamp = (Get-Date).ToString('o')
    IsWindows = $env:OS -eq 'Windows_NT'
    ProgId = $progId
    ComRegistered = $comAvailable
    ComProbeRequested = [bool]$ProbeCom
    ComProbeSucceeded = $comProbeSucceeded
    ComProbeError = $comProbeError
    Installations = @($installations)
}

if ($AsJson) {
    $result | ConvertTo-Json -Depth 5
}
else {
    $result
}

if (-not $executables -or -not $comAvailable -or ($ProbeCom -and -not $comProbeSucceeded)) {
    exit 1
}
