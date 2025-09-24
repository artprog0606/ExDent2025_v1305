#--------------------------------------------------
# Project: AdvantShop.NET
# Web site: https:\\www.advantshop.net
#--------------------------------------------------

#region Params
param (
#region Paths
# Path to published site (folder with published ADVANTSHOP)
    [String]$SitePath = ""
#endregion Paths
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
Import-Module $ScriptDir\helpers\SqlHelper.psm1
Import-Module $ScriptDir\helpers\IISHelper.psm1

# Writing params to log (check the correctness of the received parameters)
#region Correctness
Write-Host "!Check the correctness"
Write-Host $PS_secondLevelLog "SitePath " $SitePath
Write-Host "!End of params"
#endregion Correctness

function CleanTemplateFolder
{
    param(
        [string]$Path,
        [bool]$WithWebpack,
        [bool]$LeavePartialScripts,
        [string]$LogPadding
    )
    Write-Host $LogPadding "!Cleaning" $Path

    Write-Host $LogPadding "!Removing bundle_config, node_modules, node_scripts folders in $Path"
    RemoveFolder("$Path\bundle_config\");

    Write-Host $LogPadding "!Removing 'design' folder in $Path with exlude => .css, images, .config"
    CleanFolder -root "$Path\design" -exclude ('.css', 'images', '.config')

    Write-Host $LogPadding "!Removing 'scripts' folder in $Path with exlude => .html"
    CleanFolder -root "$Path\scripts" -exclude ('.html')

    Write-Host $LogPadding "!Removing 'styles' folder in $Path with exlude => .html"
    CleanFolder -root "$Path\styles" -exclude ('.html')
}

function CleanMobileFolder
{
    param(
        [string]$Path,
        [bool]$WithWebpack,
        [bool]$LeavePartialScripts,
        [string]$LogPadding
    )
    Write-Host $LogPadding "!Cleaning" $Path

    RemoveFile("$Path\webpack.config.js")
    RemoveFile("$Path\webpack.config.dev.js")
    RemoveFile("$Path\webpack.config.prod.js")

    Write-Host $LogPadding "!Removing bundle_config, node_modules, node_scripts folders in $Path"
    RemoveFolder("$Path\bundle_config\");

    Write-Host $LogPadding "!Removing 'scripts' folder in $Path with exlude => .html"
    CleanFolder -root "$Path\scripts" -exclude ('.html')

    Write-Host $LogPadding "!Removing 'styles' folder in $Path with exlude => .html"
    CleanFolder -root "$Path\styles" -exclude ('.html')
}

try
{
    $templatesRoot = $SitePath + '\Templates'
    $templates = Get-ChildItem $templatesRoot

    if ($templates.Count -gt 0)
    {
        Write-Host "!Cleaning Templates folders"
        foreach ($templatesItem in $templates)
        {
            Write-Host $PS_firstLevelLog "Removing files for $templatesItem"
            $templeteRoot = "$templatesRoot\$templatesItem";
            CleanTemplateFolder -Path $templeteRoot -WithWebpack $True -LeavePartialScripts $False -LogPadding $PS_secondLevelLog

            Write-Host $PS_firstLevelLog "Removing files for $templatesItem\Areas\Mobile\"
            $templateMobileRoot = "$templeteRoot\Areas\Mobile\";
            if (Test-Path $templateMobileRoot)
            {
                CleanMobileFolder -Path $templateMobileRoot -WithWebpack $True -LeavePartialScripts $False -LogPadding $PS_secondLevelLog
            }
        }
    }
    else
    {
        Write-Host "Not found templates for clean"
    }
    
    Write-Host "done"
}
catch [Exception]
{
    FormatErrors $Error
    exit 1
}