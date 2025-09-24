#--------------------------------------------------
# Project: AdvantShop.NET
# Web site: https:\\www.advantshop.net
#--------------------------------------------------

#region Params
param(
# Path to published site (folder with published ADVANTSHOP)
    [String]$SitePath = "",
# Name of environment variable in TeamCity for Version
    [String]$TeamCityVersionEnvName = "",
# Name of environment variable in TeamCity for PublicVersion
    [String]$TeamCityPublicVersionEnvName = "",
# Split PublicVersion by separator (takes right part of the string)
    [String]$SplitPublicVersionBy = "FIX_"
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
Write-Host $PS_firstLevelLog "TeamCityVersionEnvName -->" $TeamCityVersionEnvName
Write-Host $PS_firstLevelLog "TeamCityPublicVersionEnvName -->" $TeamCityPublicVersionEnvName
Write-Host $PS_firstLevelLog "SplitPublicVersionBy -->" $SplitPublicVersionBy
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
if ($TeamCityVersionEnvName -eq $null -or $TeamCityVersionEnvName -eq "")
{
    Write-Host $PS_firstLevelLog "TeamCityVersionEnvName param is empty"
    exit 1
}
if ($TeamCityPublicVersionEnvName -eq $null -or $TeamCityPublicVersionEnvName -eq "")
{
    Write-Host $PS_firstLevelLog "TeamCityPublicVersionEnvName param is empty"
    exit 1
}
if ($SplitPublicVersionBy -eq $null -or $SplitPublicVersionBy -eq "")
{
    Write-Host $PS_firstLevelLog "SplitPublicVersionBy param is empty"
    exit 1
}
Write-Host "!Params checked"
#endregion Param checks

try
{
    Write-Host $PS_firstLevelLog "Reading Version parameter"
    $version = GetWebConfigAppSetting -SitePath $SitePath "Version"
    Write-Host $PS_secondLevelLog $version
    Write-Host "##teamcity[setParameter name='$TeamCityVersionEnvName' value='$version']"


    Write-Host $PS_firstLevelLog "Reading PublicVersion parameter"
    $publicVersion = GetWebConfigAppSetting -SitePath $SitePath "PublicVersion"
    Write-Host $PS_secondLevelLog "Raw PublicVersion -" $publicVersion
    if (($publicVersion -match $SplitPublicVersionBy))
    {
        $publicVersion = ($publicVersion -split $SplitPublicVersionBy)[-1]
        Write-Host $PS_secondLevelLog "Splitted PublicVersion -" $publicVersion
    }
    Write-Host "##teamcity[setParameter name='$TeamCityPublicVersionEnvName' value='$publicVersion']"
}
catch [Exception]
{
    FormatErrors $Error
    exit 1
}

Write-Host "!Done!"
