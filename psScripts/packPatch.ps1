#--------------------------------------------------
# Project: AdvantShop.NET
# Web site: https:\\www.advantshop.net
#--------------------------------------------------

#region Params
param (
#region SQL
# Name of sql server and instance
	[String]$SqlName = "localhost\SQLEXPRESS",
# Name of sql user
	[String]$SqlUserName = "sa",
# SQL password
	[String]$SqlPassword = "",
# SQL database name
	[String]$SqlDatabaseName = "",
# Shrink database
	[String]$ShrinkDatabase = "false",
# Custom sql script path
	[String]$CustomSqlScriptPath = "",
#endregion SQL

#region Paths
# Path to dbs (folder where MSSQL contains db files)
	[String]$PathToDb = "",
# Path to chekout folder (ADVANTSHOP project src folder)
	[String]$CheckoutPath = "",
# Path to published site (folder with published ADVANTSHOP)
	[String]$SitePath = "",
#endregion Paths          

#region IIS
# IIS pool name
	[String]$IISPoolName = "",
# IIS site name
	[String]$IISSiteName = "",
# IIS site url
	[String]$IISUrl = "localhost",
#endregion

#region Output
# Output path
	[String]$OutputPath ="E:\advantshopOut\patch"
#endregion
)
#endregion Params

#region Local params
$PS_firstLevelLog = "   "
$PS_secondLevelLog = "      "
$SqlBackupFile = $CheckoutPath + "\DataBase\AdvantShop_13.0.0_empty.bak"
$SqlPatchesPath = $CheckoutPath + "\DataBase\patches"
#endregion Local params



# Importing helpers scripts
$ScriptDir = Split-Path -parent $MyInvocation.MyCommand.Path
Import-Module $ScriptDir\helpers\PsHelper.psm1
Import-Module $ScriptDir\helpers\FilesHelper.psm1
Import-Module $ScriptDir\helpers\SqlHelper.psm1
Import-Module $ScriptDir\helpers\IISHelper.psm1

# Writing params to log (check the correctness of the received parameters)
#region Correctness
Write-Host "!Check the correctness"
Write-Host $PS_secondLevelLog "SQL"
Write-Host $PS_secondLevelLog "SqlName " $SqlName
Write-Host $PS_secondLevelLog "SqlUserName " $SqlUserName
Write-Host $PS_secondLevelLog "SqlPassword " $SqlPassword
Write-Host $PS_secondLevelLog "SqlDatabaseName " $SqlDatabaseName
Write-Host $PS_secondLevelLog "ShrinkDatabase " $ShrinkDatabase
Write-Host $PS_secondLevelLog "CustomSqlScriptPath " $CustomSqlScriptPath
Write-Host "---"
Write-Host $PS_secondLevelLog "Paths"
Write-Host $PS_secondLevelLog "PathToDb " $PathToDb
Write-Host $PS_secondLevelLog "CheckoutPath " $CheckoutPath
Write-Host $PS_secondLevelLog "SitePath " $SitePath
Write-Host "---"
Write-Host $PS_secondLevelLog "IIS"
Write-Host $PS_secondLevelLog "IISPoolName " $IISPoolName
Write-Host $PS_secondLevelLog "IISSiteName " $IISSiteName
Write-Host $PS_secondLevelLog "IISUrl " $IISUrl
Write-Host "---"
Write-Host $PS_secondLevelLog "OutputPath " $OutputPath
Write-Host "!End of params"
#endregion Correctness

# Check if params is not set
#region Param checks
Write-Host "!Checking params"
if ($SqlName -eq $null -or $SqlName -eq "")
{
	Write-Host $PS_firstLevelLog "SqlName param is empty"
	exit 1
}
if ($SqlUserName -eq $null -or $SqlUserName -eq "")
{
	Write-Host $PS_firstLevelLog "SqlUserName param is empty"
	exit 1
}
if ($SqlPassword -eq $null -or $SqlPassword -eq "")
{
	Write-Host $PS_firstLevelLog "SqlPassword param is empty"
	exit 1
}
if ($SqlDatabaseName -eq $null -or $SqlDatabaseName -eq "")
{
	Write-Host $PS_firstLevelLog "SqlDatabaseName param is empty"
	exit 1
}
if ($ShrinkDatabase -eq $null -or $ShrinkDatabase -eq "")
{
	Write-Host $PS_firstLevelLog "ShrinkDatabase param is empty"
	exit 1
}
if ($PathToDb -eq $null -or $PathToDb -eq "")
{
	Write-Host $PS_firstLevelLog "PathToDb param is empty"
	exit 1
}
if ($CheckoutPath -eq $null -or $CheckoutPath -eq "")
{
	Write-Host $PS_firstLevelLog "CheckoutPath param is empty"
	exit 1
}
if ($SitePath -eq $null -or $SitePath -eq "")
{
	Write-Host $PS_firstLevelLog "SitePath param is empty"
	exit 1
}
if ($IISPoolName -eq $null -or $IISPoolName -eq "")
{
	Write-Host $PS_firstLevelLog "IISPoolName param is empty"
	exit 1
}
if ($IISSiteName -eq $null -or $IISSiteName -eq "")
{
	Write-Host $PS_firstLevelLog "IISSiteName param is empty"
	exit 1
}
if ($IISUrl -eq $null -or $IISUrl -eq "")
{
	Write-Host $PS_firstLevelLog "IISUrl param is empty"
	exit 1
}
if ($OutputPath -eq $null -or $OutputPath -eq "")
{
	Write-Host $PS_firstLevelLog "OutputPath param is empty"
	exit 1
}
Write-Host "!Params checked"
#endregion Param checks

# Converting params because in case of TeamCity params converts to strings
#region Converting
try
{
	$ShrinkDatabaseBool = [System.Convert]::ToBoolean($ShrinkDatabase)
}
catch [FormatException]
{
	Write-Host "!Error on convertion ShrinkDatabase param to boolean - settings will be false"
	$ShrinkDatabaseBool = $false
}
#endregion Converting


$version = GetVersion $sitepath

$database = "patch" + $version
$site ="patch" + $version

RenameFolder $sitepath $site

$sitepath=$sitepath.Replace('current',$site)

try 
{
	Write-Host "patch"
	$outPath = $OutputPath + "\"+$site

	RemoveFolderRetry $outPath
	CreateFolder $outPath

	$sqlpath = $outPath +"\_SQL"

	createFolder $sqlpath

	foreach ($f in Get-ChildItem -path $SqlPatchesPath -Filter *.sql | sort-object { [regex]::Replace($_.Name, '\d+', { $args[0].Value.PadLeft(20) }) } )
	{
		if($f.FullName.Contains($version)){
			Copy-Item $f.FullName $sqlpath
		}
	}

	$patchPath = $outPath +"\patch"
	$exclude = @('AdvantShop.sln.metaproj','AdvantShop.sln.metaproj.tmp', 'install.txt', 'Web.ModeSettings.config', 'Web.ConnectionString.config', 'Advantshop.Web.Site.csproj.teamcity','Advantshop.Web.Site.csproj.teamcity.msbuild.tcargs','gulpfile.js','job_scheduling_data_2_0.xsd','karma.conf.js','robots.txt' )
	$excludeMatch = @('\pictures\','\userfiles\' , '\combine\' , '^\modules\', '\pictures\' , '\userfiles\' , '\export\',  '\errlog\')
	CopyFolderRetry $sitepath $patchPath $exclude $excludeMatch

	RemoveCommonFiles -rootPath $patchPath
	RemoveCommonFiles -rootPath "$patchPath/Areas/Mobile"
	
	Write-Host "remove PDB"

	del ($patchPath + "\bin\*.pdb")	
	
    Write-Host "zip site"
    #zip site
    $zipSite = $outPath + "\" + $site + ".zip"
    $zipFolderPath = $outPath + "\*"
	ZipWith7Zip $zipSite $zipFolderPath

	RemoveFolderRetry $patchPath
	RemoveFolderRetry $sqlpath
	
	Write-Host "done"
}
catch [Exception]
{
	FormatErrors $Error
	exit 1
}