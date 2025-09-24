#--------------------------------------------------
# Project: AdvantShop.NET
# Web site: https:\\www.advantshop.net
#--------------------------------------------------

#region Params
param(
# Path to published site (folder with published ADVANTSHOP)
    [String]$SitePath = ""
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

# Writing params to log (check the correctness of the received parameters)
#region Correctness
Write-Host "!Check the correctness"
Write-Host $PS_firstLevelLog "SitePath --> " $SitePath
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
Write-Host "!Params checked"
#endregion Param checks

try
{
    Write-Host "!Adding app_offline.htm"
    $appOfflineFile = $SitePath + "\app_offline.htm";
    if (Test-Path $appOfflineFile)
    {
        Remove-Item $appOfflineFile
    }
    $appOfflineFile = $SitePath + "\app_offline.html";
    if (Test-Path $appOfflineFile)
    {
        Rename-Item $appOfflineFile "app_offline.htm"
    }

    Write-Host "!Cleaning Site folder"
    CleanFolder -root $SitePath -exclude ('app_offline.htm')
}
catch [Exception]
{
    FormatErrors $Error
    exit 1
}

Write-Host "!Done!"
