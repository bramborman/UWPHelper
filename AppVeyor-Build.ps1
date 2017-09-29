Write-Host "`nAppVeyor-Build script executed"
Write-Host   "=============================="

# Before build
Start-FileDownload "https://raw.githubusercontent.com/bramborman/AppVeyorBuildScripts/master/Scripts/Set-BuildVersion.ps1"
.\Set-BuildVersion.ps1

Start-FileDownload "https://raw.githubusercontent.com/bramborman/AppVeyorBuildScripts/master/Scripts/Set-PureBuildVersion.ps1"
.\Set-PureBuildVersion.ps1

nuget restore -source "https://api.nuget.org/v3/index.json;https://www.myget.org/F/bramborman/api/v3/index.json"

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
