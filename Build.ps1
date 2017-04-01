# Before build
$buildVersion = $env:APPVEYOR_BUILD_VERSION

if ($env:APPVEYOR_REPO_BRANCH -eq "master")
{
	$newVersion	= $buildVersion.Split("-") | Select-Object -first 1
	$message    = "Build version changed from '$buildVersion' to '$newVersion'"

	$buildVersion = $newVersion
	Update-AppveyorBuild -Version $buildVersion

	Add-AppveyorMessage $message
	Write-Host $message
}

Start-FileDownload 'https://dist.nuget.org/win-x86-commandline/latest/nuget.exe'
.\nuget restore

# Build
$platforms 					= "x86", "x64", "ARM"
$uwpHelperProjectDirectory 	= Get-ChildItem -Directory -Filter "UWPHelper"

if (!(Test-Path $uwpHelperProjectDirectory))
{
	Throw "Unable to find UWPHelper project directory. uwpHelperProjectDirectory: $uwpHelperProjectDirectory"
}

foreach ($platform in $platforms)
{
	MSBuild "$uwpHelperProjectDirectory\UWPHelper.csproj" /verbosity:minimal /logger:"C:\Program Files\AppVeyor\BuildAgent\Appveyor.MSBuildLogger.dll" /p:Configuration=Release /p:Platform=$platform

	$zipFileName = "UWPHelper_$platform.$buildVersion.zip"
	7z a $zipFileName "$uwpHelperProjectDirectory\bin\$platform\Release\*"
	
	Push-AppveyorArtifact $zipFileName
}

# After build
& "NuGet\CreatePackage.ps1"
