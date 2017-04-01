# Any assembly matching this filter will be transformed into an AnyCPU assembly.
$dllName = "UWPHelper.dll"

# Find the newest version of the NETFX Tools
$netfxParentFolder 	= Join-Path ${Env:ProgramFiles(x86)} "Microsoft SDKs\Windows\v10.0A\bin\"
$netfxFolders 		= Get-ChildItem $netfxParentFolder -Directory -Filter "NETFX * Tools"
# Sort folders using regex to sort numbers logically - 1.0.0, 2.0.0, 10.0.0 - instead of 1.0.0, 10.0.0, 2.0.0
# Select the newest NETFX Tools folder
$netfxFolder		= $netfxFolders | Sort-Object { [regex]::Replace($_.Name, '\d+', { $args[0].Value.PadLeft(20) }) } | Select-Object -Last 1
$corFlags			= Join-Path (Join-Path $netfxParentFolder $netfxFolder) "x64\CorFlags.exe"

if (!(Test-Path $corFlags))
{
	# Try to find x86 version (not sure if it really exists)
	$corFlags = Join-Path (Join-Path $netfxParentFolder $netfxFolder) "CorFlags.exe"
}

if (!(Test-Path $corFlags))
{
	Throw "Unable to find CorFlags.exe"
}

Write-Host "`nSelected CorFlags file:" $corFlags

$uwpHelperProjectFolder = Get-ChildItem -Directory -Filter "UWPHelper"
$binFolders 			= $uwpHelperProjectFolder | ForEach-Object{ Get-ChildItem $_.FullName -Directory -Filter "bin" }

# Create reference assemblies, because otherwise the NuGet packages cannot be used.
# This creates them for all outputs that match the filter, in all output directories of all projects.
# It's a bit overkill but who cares - the process is very fast and keeps the script simple.
foreach ($binFolder in $binFolders)
{
	$x86Folder 			= Join-Path $binFolder.FullName "x86"
	$referenceFolder 	= Join-Path $binFolder.FullName "Reference"
		
	if (!(Test-Path $x86Folder))
	{
		Write-Host "Skipping reference assembly generation for $($binFolder.FullName) because it has no x86 directory."
		continue;
	}
	
	if (Test-Path $referenceFolder)
	{
		Remove-Item -Recurse $referenceFolder
	}
	
	New-Item $referenceFolder -ItemType Directory
	New-Item "$referenceFolder\Release" -ItemType Directory
	
	$dlls = Get-ChildItem "$x86Folder\Release" -File -Filter $dllName
	
	foreach ($dll in $dlls)
	{
		Copy-Item $dll.FullName "$referenceFolder\Release"
	}
	
	$dlls = Get-ChildItem "$referenceFolder\Release" -File -Filter $dllName
	
	foreach ($dll in $dlls)
	{
		Write-Host "`n`nConverting to AnyCPU: $dll"
		& $corFlags /32bitreq- $($dll.FullName)
	}
}

# This script is called from Build.ps1 which is in the parent folder so we must go there using NuGet\ again
NuGet pack "NuGet\UWPHelper.nuspec" -Version $env:APPVEYOR_BUILD_VERSION
Push-AppveyorArtifact *.nupkg
