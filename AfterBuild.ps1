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

$libraryProjectFolder 	= Get-ChildItem -Directory -Filter "UWPHelper"
$releaseFolder 			= Join-Path $libraryProjectFolder.FullName "\bin\Release"

if (!(Test-Path $releaseFolder))
{
	throw "Something happend :( releaseFolder: $releaseFolder"
}

$zipFileName = "$libraryProjectFolder.$buildVersion.zip"
7z a $zipFileName "$releaseFolder\*"

Push-AppveyorArtifact $zipFileName

& "NuGet\CreatePackage.ps1"
