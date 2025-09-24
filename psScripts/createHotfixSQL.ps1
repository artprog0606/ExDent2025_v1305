#--------------------------------------------------
# Project: AdvantShop.NET
# Web site: https:\\www.advantshop.net
#--------------------------------------------------

#region Params
param(
# Path to published site (folder with published ADVANTSHOP)
    [String]$SitePath = "",
# Path to sql patches folder 
    [String]$SQLPatchesPath = ""
)
#endregion Params

#region Local params
$PS_firstLevelLog = "   "
$PS_secondLevelLog = "      "
#endregion Local params

# Importing helpers scripts
$ScriptDir = Split-Path -parent $MyInvocation.MyCommand.Path
Import-Module $ScriptDir\helpers\PsHelper.psm1
Import-Module $ScriptDir\helpers\IISHelper.psm1

# Writing params to log (check the correctness of the received parameters)
#region Correctness
Write-Host "!Check the correctness"
Write-Host $PS_firstLevelLog "SitePath -->" $SitePath
Write-Host $PS_firstLevelLog "SitePath -->" $SQLPatchesPath
Write-Host "!End of params"
#endregion Correctness

# Check if params is not set
#region Param checks
Write-Host "!Checking params"
if ($SitePath -eq $null -or $SitePath -eq "")
{
    Write-Host $PS_firstLevelLog "sitePath param is empty"
    exit 1
}
if ($SQLPatchesPath -eq $null -or $SQLPatchesPath -eq "")
{
    Write-Host $PS_firstLevelLog "SQLPatchesPath param is empty"
    exit 1
}
Write-Host "!Params checked"
#endregion Param checks

try
{
    Write-Host $PS_firstLevelLog "Reading Version parameter"
    $version = GetWebConfigAppSetting -SitePath $SitePath "Version"
    Write-Host $PS_secondLevelLog $version

    Write-Host $PS_firstLevelLog "Looking for hotfix sql patch file"
    $hotfixSqlPatchName = "SQL_" + $version + "_hotfix.sql"
    $hotfixSqlPatchPath = $SQLPatchesPath + "\" + $hotfixSqlPatchName
    if (Test-Path $hotfixSqlPatchPath)
    {
        Write-Host $PS_secondLevelLog "found" $hotfixSqlPatchName
        $hotfixSqlDestPath = $SitePath + "\_SQL\"

        Write-Host $PS_secondLevelLog "creating path" $hotfixSqlDestPath
        New-Item -ItemType Directory -Force -Path $hotfixSqlDestPath | Out-Null
        Write-Host $PS_secondLevelLog "copy file to dest"
        Copy-Item -Path $hotfixSqlPatchPath -Destination $hotfixSqlDestPath
        Write-Host $PS_secondLevelLog "renaming file to hotfix.sql"
        Rename-Item -Path $( $hotfixSqlDestPath + $hotfixSqlPatchName ) -NewName "hotfix.sql"
    }
    else
    {
        Write-Host $PS_secondLevelLog "no files found"
    }
}
catch [Exception]
{
    FormatErrors $Error
    exit 1
}

Write-Host "!Done!"
