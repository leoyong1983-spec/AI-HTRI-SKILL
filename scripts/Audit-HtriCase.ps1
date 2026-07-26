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

$auditFile = Join-Path $outputRoot ($CaseName + '.input-audit.csv')
$inputReport = Join-Path $outputRoot ($CaseName + '.input-report.txt')

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$sourceCode = Join-Path $scriptRoot 'Audit-HtriCase.cs'
$cacheRoot = Join-Path $env:LOCALAPPDATA 'ai-htri-skill'
[IO.Directory]::CreateDirectory($cacheRoot) | Out-Null
$runner = Join-Path $cacheRoot 'Audit-HtriCase.exe'

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
        throw "Failed to compile the HTRI audit runner. Exit code: $LASTEXITCODE"
    }
}

& $runner $source $auditFile $inputReport
$exitCode = $LASTEXITCODE
if ($exitCode -ne 0) {
    throw "HTRI audit failed with exit code $exitCode."
}

[pscustomobject]@{
    SourceModel = $source
    InputAudit = $auditFile
    InputReport = $inputReport
    ExitCode = $exitCode
}
