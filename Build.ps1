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
$uwpHelperProjectFolder = Get-ChildItem -Directory -Filter "UWPHelper"

if (!(Test-Path $uwpHelperProjectFolder))
{
	Throw "Unable to find UWPHelper project folder. uwpHelperProjectFolder: $uwpHelperProjectFolder"
}

$platforms = "x86", "x64", "ARM"

foreach ($platform in $platforms)
{
	Write-Host "`n`nStarted $platform build"
	Write-Host     "======================="

	MSBuild "$uwpHelperProjectFolder\UWPHelper.csproj" /verbosity:minimal /logger:"C:\Program Files\AppVeyor\BuildAgent\Appveyor.MSBuildLogger.dll" /p:Configuration=Release /p:Platform=$platform

	$releaseFolder = Join-Path $uwpHelperProjectFolder.FullName "\bin\$platform\Release\"

	if (!(Test-Path $releaseFolder))
	{
		throw "Something happend :( releaseFolder: $releaseFolder"
	}

	$zipFileName = "UWPHelper_$platform.$buildVersion.zip"
	7z a $zipFileName "$releaseFolder\*"
	
	Push-AppveyorArtifact $zipFileName
}

# After build
& NuGet\CreatePackage.ps1
