# Before build
# Use this script after UWPHelper went out of preview
# Start-FileDownload "https://raw.githubusercontent.com/bramborman/AppVeyorBuildScripts/master/Scripts/Set-PureBuildVersion.ps1"
# .\Set-PureBuildVersion.ps1

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

# Have to use different version because of a bug in 4.0.0
Start-FileDownload "https://dist.nuget.org/win-x86-commandline/v4.1.0/nuget.exe"
.\nuget restore -FallbackSource https://www.myget.org/F/bramborman/api/v3/index.json

if($LastExitCode -ne 0)
{
	$host.SetShouldExit($LastExitCode)
}

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

	$zipFileName = "UWPHelper_$platform.$env:APPVEYOR_BUILD_VERSION.zip"
	7z a $zipFileName "$releaseFolder\*"
	
	Push-AppveyorArtifact $zipFileName
}

Start-FileDownload "https://raw.githubusercontent.com/bramborman/AppVeyorBuildScripts/master/Scripts/NuGet-Pack.ps1"
.\NuGet-Pack.ps1 -UWPMultiArchitecture -DllFilter "UWPHelper.dll" -ProjectFoldersFilter "UWPHelper"

Start-FileDownload "https://raw.githubusercontent.com/bramborman/AppVeyorBuildScripts/master/Scripts/Deployment-Skipping.ps1"
.\Deployment-Skipping.ps1
