param([switch]$Package)

$ErrorActionPreference = 'Stop'
$root = $PSScriptRoot
$project = Join-Path $root 'DadsAdventureMapping.csproj'
$manifestPath = Join-Path $root 'package\manifest.json'
$manifest = Get-Content -LiteralPath $manifestPath -Raw | ConvertFrom-Json
$dllPath = Join-Path $root 'bin\Release\net48\DadsAdventureMapping.dll'

dotnet build $project -c Release --nologo
if ($LASTEXITCODE -ne 0) { throw "Build exited with code $LASTEXITCODE" }

$version = [Reflection.AssemblyName]::GetAssemblyName($dllPath).Version
$assemblyVersion = "$($version.Major).$($version.Minor).$($version.Build)"
if ($assemblyVersion -ne $manifest.version_number) {
    throw "Assembly version $assemblyVersion differs from manifest version $($manifest.version_number)."
}

if (-not $Package) { return }

$entries = [ordered]@{
    'DadsAdventureMapping.dll' = $dllPath
    'manifest.json' = $manifestPath
    'README.md' = (Join-Path $root 'README.md')
    'icon.png' = (Join-Path $root 'package\icon.png')
}
foreach ($source in $entries.Values) {
    if (-not (Test-Path -LiteralPath $source -PathType Leaf)) { throw "Missing package file: $source" }
}

$dist = Join-Path $root 'dist'
$archive = Join-Path $root 'Archive\package-builds'
$rootFull = [IO.Path]::GetFullPath($root).TrimEnd([IO.Path]::DirectorySeparatorChar) + [IO.Path]::DirectorySeparatorChar
foreach ($path in @($dist, $archive)) {
    $full = [IO.Path]::GetFullPath($path)
    if (-not $full.StartsWith($rootFull, [StringComparison]::OrdinalIgnoreCase)) {
        throw "Package path is outside the project: $full"
    }
}
New-Item -ItemType Directory -Path $dist -Force | Out-Null
New-Item -ItemType Directory -Path $archive -Force | Out-Null

$folder = Join-Path $dist "DadsAdventureMapping-$($manifest.version_number)"
$zip = Join-Path $dist "DadsAdventureMapping-$($manifest.version_number).zip"
$stamp = Get-Date -Format 'yyyyMMdd-HHmmss-fff'
foreach ($old in @($folder, $zip)) {
    if (Test-Path -LiteralPath $old) {
        $item = Get-Item -LiteralPath $old
        $archivedName = if ($item.PSIsContainer) { "$($item.Name)-$stamp" } else { "$($item.BaseName)-$stamp$($item.Extension)" }
        Move-Item -LiteralPath $old -Destination (Join-Path $archive $archivedName)
    }
}

New-Item -ItemType Directory -Path $folder | Out-Null
foreach ($entry in $entries.GetEnumerator()) {
    Copy-Item -LiteralPath $entry.Value -Destination (Join-Path $folder $entry.Key)
}
Compress-Archive -Path (Join-Path $folder '*') -DestinationPath $zip

Add-Type -AssemblyName System.IO.Compression
Add-Type -AssemblyName System.IO.Compression.FileSystem
$archiveFile = [IO.Compression.ZipFile]::OpenRead($zip)
try {
    $names = @($archiveFile.Entries | ForEach-Object FullName)
    foreach ($name in $entries.Keys) {
        if ($name -notin $names) { throw "ZIP is missing $name" }
    }
}
finally { $archiveFile.Dispose() }

Write-Host "Package: $zip"
