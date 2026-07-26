[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$InputFile,

    [Parameter(Mandatory)]
    [string]$OutputDirectory,

    [string]$CaseName
)

$ErrorActionPreference = 'Stop'

$source = (Resolve-Path -LiteralPath $InputFile).Path
if (-not (Test-Path -LiteralPath $source -PathType Leaf)) {
    throw "HTRI input file not found: $InputFile"
}

$outputRoot = [IO.Path]::GetFullPath($OutputDirectory)
[IO.Directory]::CreateDirectory($outputRoot) | Out-Null

if ([string]::IsNullOrWhiteSpace($CaseName)) {
    $CaseName = [IO.Path]::GetFileNameWithoutExtension($source)
}
$CaseName = $CaseName -replace '[^A-Za-z0-9._-]', '_'

$savedModel = Join-Path $outputRoot ($CaseName + '.rerun.htri')
$logFile = Join-Path $outputRoot ($CaseName + '.run.log')
$resultFile = Join-Path $outputRoot ($CaseName + '.results.csv')

if ([string]::Equals($source, $savedModel, [StringComparison]::OrdinalIgnoreCase)) {
    throw 'The output model must not overwrite the source model.'
}

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$sourceCode = Join-Path $scriptRoot 'Invoke-HtriCase.cs'
$cacheRoot = Join-Path $env:LOCALAPPDATA 'ai-htri-skill'
[IO.Directory]::CreateDirectory($cacheRoot) | Out-Null
$runner = Join-Path $cacheRoot 'Invoke-HtriCase.exe'

$frameworkRoots = @(
    (Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'),
    (Join-Path $env:WINDIR 'Microsoft.NET\Framework\v4.0.30319\csc.exe')
)
$compiler = $frameworkRoots | Where-Object { Test-Path -LiteralPath $_ } | Select-Object -First 1
if (-not $compiler) {
    throw 'The .NET Framework C# compiler was not found.'
}

if (-not (Test-Path -LiteralPath $runner) -or (Get-Item -LiteralPath $sourceCode).LastWriteTimeUtc -gt (Get-Item -LiteralPath $runner).LastWriteTimeUtc) {
    & $compiler /nologo /target:exe "/out:$runner" /reference:Microsoft.CSharp.dll $sourceCode
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to compile the HTRI runner. Exit code: $LASTEXITCODE"
    }
}

& $runner $source $savedModel $logFile $resultFile
$exitCode = $LASTEXITCODE
if ($exitCode -ne 0) {
    throw "HTRI run failed with exit code $exitCode. Review $logFile"
}

[pscustomobject]@{
    SourceModel = $source
    SavedModel = $savedModel
    RunLog = $logFile
    Results = $resultFile
    ExitCode = $exitCode
}
