#--------------------------------------------------
# Project: AdvantShop.NET
# Web site: https:\\www.advantshop.net
#--------------------------------------------------

#region Params
param(
#region SQL
# Name of sql server and instance
    [String]$SqlName = "localhost\SQLEXPRESS",
# Name of sql user
    [String]$SqlUserName = "sa",
# SQL password
    [String]$SqlPassword = "",
# SQL database name
    [String]$SqlDatabaseName = "",
#endregion SQL

# Path to published site (folder with published ADVANTSHOP)
    [String]$SitePath = "",
# Path to chekout folder (ADVANTSHOP project src folder)
    [String]$CheckoutPath = "",
# Path to published criticalcss
    [String]$CriticalCssPath = "",
# Output path
    [String]$OutputPath = "",
# Version
    [String]$Version = "",
# Site mode
    [String]$SiteMode = "Lic",

# Zip bak file
    [String]$ZipBak = "true"
)
#endregion Params

#region Local params
$PS_firstLevelLog = "   "
$PS_secondLevelLog = "      "
#endregion Local params

# Importing helpers scripts
$ScriptDir = Split-Path -parent $MyInvocation.MyCommand.Path
Import-Module $ScriptDir\helpers\PsHelper.psm1
Import-Module $ScriptDir\helpers\FilesHelper.psm1
Import-Module $ScriptDir\helpers\IISHelper.psm1

# Writing params to log (check the correctness of the received parameters)
#region Correctness
Write-Host "!Check the correctness"
Write-Host $PS_secondLevelLog "SQL --> "
Write-Host $PS_secondLevelLog "SqlName --> " $SqlName
Write-Host $PS_secondLevelLog "SqlUserName --> " $SqlUserName
Write-Host $PS_secondLevelLog "SqlPassword --> " $SqlPassword
Write-Host $PS_secondLevelLog "SqlDatabaseName --> " $SqlDatabaseName
Write-Host "---"
Write-Host $PS_firstLevelLog "SitePath --> " $SitePath
Write-Host $PS_firstLevelLog "CriticalCssPath --> " $CriticalCssPath
Write-Host $PS_firstLevelLog "OutputPath --> " $OutputPath
Write-Host $PS_firstLevelLog "SiteMode --> " $SiteMode
Write-Host "---"
Write-Host $PS_firstLevelLog "ZipBak --> " $ZipBak
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
if ($SitePath -eq $null -or $SitePath -eq "")
{
    Write-Host $PS_firstLevelLog "SitePath param is empty"
    exit 1
}
if ($CriticalCssPath -eq $null -or $CriticalCssPath -eq "")
{
    Write-Host $PS_firstLevelLog "CriticalCssPath param is empty"
    exit 1
}
if ($OutputPath -eq $null -or $OutputPath -eq "")
{
    Write-Host $PS_firstLevelLog "OutputPath param is empty"
    exit 1
}
if ($SiteMode -eq $null -or $SiteMode -eq "")
{
    Write-Host $PS_firstLevelLog "SiteMode param is empty"
    exit 1
}
if ($ZipBak -eq $null -or $ZipBak -eq "")
{
    Write-Host $PS_firstLevelLog "ZipBak param is empty"
    exit 1
}
Write-Host "!Params checked"
#endregion Param checks

# Converting params because in case of TeamCity params converts to strings
#region Converting
try
{
    $ZipBakBool = [System.Convert]::ToBoolean($ZipBak)
}
catch [FormatException]
{
    Write-Host "!Error on convertion ZipBak param to boolean - settings will be false"
    $ZipBakBool = $false
}
#endregion Converting

try
{
    CleanFolder $OutputPath
    CreateFolder $OutputPath

    Write-Host "!Copy bak file"
    $originalDbBackupFilePath = "$SitePath\$SiteMode.bak"
    $copyDbBackupFilePath = "$OutputPath\$SiteMode.bak"
    Copy-Item $originalDbBackupFilePath -Destination $copyDbBackupFilePath
    RemoveFile $originalDbBackupFilePath

    Write-Host "!Rename bak file"
    $dbBackupFile = "$SiteMode$Version.bak"
    if ($SiteMode -eq "Lic")
    {
        $dbBackupFile = "Advantshop_$Version.bak"
    }
    Rename-Item $copyDbBackupFilePath $dbBackupFile

    if($ZipBakBool)
    {
        Write-Host "!Zip bak file"
        $copyDbBackupFilePath = "$OutputPath\$dbBackupFile"
        $dbZipFilePath = "$OutputPath\$SiteMode$Version" + "_db.zip"
        ZipWith7Zip $dbZipFilePath $copyDbBackupFilePath

        Write-Host "!Deleting bak file"
        RemoveFile $copyDbBackupFilePath
    }

    Write-Host "!Set default connection string"
    SetDefaultConnectionString $SitePath

    $exclude = @("ttt.ttx");
    $excludeMatch = @("qwqwqwq");
    Write-Host "!Copy critical css to site"
    $copyFrom = $CriticalCssPath + "\_criticalcss"
    $copyTo = $SitePath + "\_criticalcss"
    CopyFolderRetry $copyFrom $copyTo $exclude $excludeMatch
    $copyFrom = $CriticalCssPath + "\Areas\Mobile\_criticalcss"
    $copyTo = $SitePath + "\Areas\Mobile\_criticalcss"
    CopyFolderRetry $copyFrom $copyTo $exclude $excludeMatch

    if ($SiteMode -eq "Trial")
    {
        Write-Host $PS_firstLevelLog "!Copy critical css for Templates"
        $copyFrom = $CriticalCssPath + "\Templates"
        $copyTo = $SitePath + "\Templates"
        CopyFolderRetry $copyFrom $copyTo $exclude $excludeMatch
    }

    if ($SiteMode -ne "Lic")
    {
        Write-Host "!Zip site"
        $zipSiteFilePath = "$OutputPath\$SiteMode$Version" + "_code.zip"
        $zipFolderPath = $SitePath + "\*"
        ZipWith7Zip $zipSiteFilePath $zipFolderPath
    }
    else
    {
        Write-Host $PS_firstLevelLog "!Copy published site to published folder"

        $exclude = @('AdvantShop.sln.metaproj','AdvantShop.sln.metaproj.tmp','Advantshop.Web.Site.csproj.teamcity','Advantshop.Web.Site.csproj.teamcity.msbuild.tcargs', 'gulpfile.js','job_scheduling_data_2_0.xsd','karma.conf.js','robots.txt', 'Web.ConnectionString.config.etalon', 'package.json.etalon', 'karma.conf.js', 'job_scheduling_data_2_0.xsd', 'gulpfile.js', 'AdvantShop.Web.Site.csproj.teamcity.msbuild.tcargs', 'AdvantShop.Web.Site.csproj.teamcity','.babelrc', '.browserslistrc', '.npmrc', 'postcss.config.js', 'webpack.config.js', 'webpack.config.dev.js', 'webpack.config.prod.js', 'webpack.config.rules.js', 'package.json', 'package-lock.json');
        $excludeMatch = @('\obj\','\errlog\', '\bundle_config\', '\node_modules\', '\node_scripts\', '\Jobs\');
        $copyFrom = $SitePath
        $copyTo = $OutputPath + "\published"
        CopyFolderRetry $copyFrom $copyTo $exclude $excludeMatch

        Write-Host "!Set default connection string in published folder"
        SetDefaultConnectionString $copyTo

        Write-Host $PS_firstLevelLog "!Copy git files to source folder"

        $exclude = @('AdvantShop.sln.metaproj','AdvantShop.sln.metaproj.tmp');
        $excludeMatch = @('\bin\','\obj\','\errlog\');
        $copyFrom = $CheckoutPath
        $copyTo = $OutputPath + "\source"
        CopyFolderRetry $copyFrom $copyTo $exclude $excludeMatch

        Write-Host "!Zip lic files"
        $zipSiteFilePath = $OutputPath + ".zip"
        $zipFolderPath = $OutputPath + "\*"
        ZipWith7Zip $zipSiteFilePath $zipFolderPath

        Write-Host "!Delete lic files"
        CleanFolder $OutputPath
    }

    Write-Host "!Set connection string"
    SetConnectionString $SitePath $SqlName $SqlDatabaseName $SqlUserName $SqlPassword
}
catch [Exception]
{
    FormatErrors $Error
    exit 1
}

Write-Host "!Done!"
