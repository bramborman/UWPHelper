$projectName = "UWPHelper"

# Any assembly matching this filter will be transformed into an AnyCPU assembly.
$referenceDllFilter = $projectName + ".dll"

# Find the newest version of the NETFX Tools
$programFilesx86 		= "${Env:ProgramFiles(x86)}"
$netfxParentDirectory 	= Join-Path $programFilesx86 "Microsoft SDKs\Windows\v10.0A\bin\"
$netfxDirectories 		= Get-ChildItem $netfxParentDirectory -Directory -Filter "NETFX * Tools"
$netfxDirectoriesSorted = $netfxDirectories | Sort-Object { [regex]::Replace($_.Name, '\d+', { $args[0].Value.PadLeft(20) }) }
$netfxDirectory 		= $netfxDirectoriesSorted | Select-Object -Last 1
$corFlags				= Join-Path (Join-Path $netfxParentDirectory $netfxDirectory) "x64\CorFlags.exe"

If (!(Test-Path $corFlags))
{
	# Try find x86 version
	$corFlags = Join-Path (Join-Path $netfxParentDirectory $netfxDirectory) "CorFlags.exe"
}

Write-Host "`nSelected CorFlags file:" $corFlags

If (!(Test-Path $corFlags))
{
	Throw "Unable to find CorFlags.exe"
}


$solutionRoot 		= Resolve-Path ..\
$topLevelDirectory 	= Get-ChildItem $solutionRoot -Directory -Filter $projectName
$binDirectories 	= $topLevelDirectory | ForEach-Object{ Get-ChildItem $_.FullName -Directory -Filter "bin" }

# Create reference assemblies, because otherwise the NuGet packages cannot be used.
# This creates them for all outputs that match the filter, in all output directories of all projects.
# It's a bit overkill but who cares - the process is very fast and keeps the script simple.
Foreach ($bin in $binDirectories)
{
	$x86 = Join-Path $bin.FullName "x86"
	$any = Join-Path $bin.FullName "Reference"
		
	If (!(Test-Path $x86))
	{
		Write-Host "Skipping reference assembly generation for $($bin.FullName) because it has no x86 directory."
		continue;
	}
	
	if (Test-Path $any)
	{
		Remove-Item -Recurse $any
	}
	
	New-Item $any -ItemType Directory
	New-Item "$any\Release" -ItemType Directory
	
	$dlls = Get-ChildItem "$x86\Release" -File -Filter $referenceDllFilter
	
	Foreach ($dll in $dlls)
	{
		Copy-Item $dll.FullName "$any\Release"
	}
	
	$dlls = Get-ChildItem "$any\Release" -File -Filter $referenceDllFilter
	
	Foreach ($dll in $dlls)
	{
		Write-Host "`n`nConverting to AnyCPU: $dll"
		& $corFlags /32bitreq- $($dll.FullName)
	}
}

NuGet pack "UWPHelper.nuspec" -Version $env:APPVEYOR_BUILD_VERSION
Push-AppveyorArtifact *.nupkg
