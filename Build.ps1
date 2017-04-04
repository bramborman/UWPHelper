# Before build
$buildVersion = $env:APPVEYOR_BUILD_VERSION

# Check whether this is commit in branch 'master' and not just PR to the branch
if (($env:APPVEYOR_REPO_BRANCH -eq "master") -and ($env:APPVEYOR_PULL_REQUEST_TITLE -eq $null) -and ($env:APPVEYOR_PULL_REQUEST_NUMBER -eq $null))
{
	$newVersion	= $buildVersion.Split("-") | Select-Object -first 1
	$newVersion = "$newVersion-preview"
	$message    = "Build version changed from '$buildVersion' to '$newVersion'."

	$buildVersion = $newVersion
	Update-AppveyorBuild -Version $buildVersion
	# Set the environment variable explicitly so it will be preserved to deployments (specifically GitHub Releases)
	Set-AppveyorBuildVariable "APPVEYOR_BUILD_VERSION" $buildVersion

	Add-AppveyorMessage $message
	Write-Host $message
}

Start-FileDownload "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"
.\nuget restore

# Build
$uwpHelperProjectFolder = Get-ChildItem -Directory -Filter "UWPHelper"

if (!(Test-Path $uwpHelperProjectFolder))
{
	throw "Unable to find UWPHelper project folder. `$uwpHelperProjectFolder: '$uwpHelperProjectFolder'"
}

$platforms = "x86", "x64", "ARM"

foreach ($platform in $platforms)
{
	Write-Host "`n`nPlatform $platform"
	Write-Host     "============"

	MSBuild "$uwpHelperProjectFolder\UWPHelper.csproj" /verbosity:minimal /logger:"C:\Program Files\AppVeyor\BuildAgent\Appveyor.MSBuildLogger.dll" /p:Configuration=Release /p:Platform=$platform

	$releaseFolder = Join-Path $uwpHelperProjectFolder.FullName "\bin\$platform\Release\"

	if (!(Test-Path $releaseFolder))
	{
		throw "Unable to find $platform release folder. `$releaseFolder: '$releaseFolder'"
	}

	$zipFileName = "UWPHelper_$platform.$buildVersion.zip"
	7z a $zipFileName "$releaseFolder\*"
	
	Push-AppveyorArtifact $zipFileName
}

# After build - create NuGet package
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
	throw "Unable to find CorFlags.exe. `$corFlags: '$corFlags'"
}

Write-Host "`nSelected CorFlags file:" $corFlags

$uwpHelperProjectFolder = Get-ChildItem -Directory -Filter "UWPHelper"
$binFolders 			= $uwpHelperProjectFolder | ForEach-Object{ Get-ChildItem $_.FullName -Directory -Filter "bin" }
$referenceCreated		= $false

# Create reference assemblies, because NuGet packages cannot be used otherwise.
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

	$referenceCreated = $true
}

if ($referenceCreated -eq $false)
{
	throw "Reference assemblies were not created.`n`$binFolders: '$binFolders'"
}

nuget pack -Version $buildVersion

# Throw the exception if NuGet creating fails to make the AppVeyor build fail too
if($LastExitCode -ne 0)
{
	$host.SetShouldExit($LastExitCode)
}

Push-AppveyorArtifact *.nupkg

# Deployment skipping
$skipDeploymentDirective = "[skip deployment]"

if (($env:APPVEYOR_REPO_COMMIT_MESSAGE -match $skipDeploymentDirective) -or ($env:APPVEYOR_REPO_COMMIT_MESSAGE_EXTENDED -match $skipDeploymentDirective))
{
	$message = "Commit message contains $skipDeploymentDirective so deployment is skipped in this build."
	Add-AppveyorMessage $message
	Write-Host $message

	Set-AppveyorBuildVariable "SKIP_DEPLOYMENT" $true
}
else
{
	Set-AppveyorBuildVariable "SKIP_DEPLOYMENT" $false
}
