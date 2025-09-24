#--------------------------------------------------
# Project: AdvantShop.NET
# Web site: https:\\www.advantshop.net
#--------------------------------------------------

#region Params
param(
# Path to published site (folder with published ADVANTSHOP)
    [String]$SitePath = "",
# Clean files after webpack build and files that dont need anymore
    [String]$CleanAfterWebpack = "false"
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
Write-Host $PS_firstLevelLog "CleanAfterWebpack --> " $CleanAfterWebpack
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

# Converting params because in case of TeamCity params converts to strings
#region Converting
try
{
    $IsWebpack = [System.Convert]::ToBoolean($CleanAfterWebpack)
    Write-Host $PS_firstLevelLog "CleanAfterWebpack converted to boolean --> " $IsWebpack
}
catch [FormatException]
{
    Write-Host $PS_firstLevelLog "!Error on convertion CleanAfterWebpack param to boolean - settings will be false"
    $IsWebpack = $false
}
#endregion Converting

function CleanRootFolder
{
    param(
        [string]$Path,
        [bool]$WithWebpack,
        [bool]$LeavePartialScripts,
        [string]$LogPadding
    )
    Write-Host $LogPadding "!Cleaning" $Path

    RemoveFile("$Path\Web.ConnectionString.config.etalon")
    RemoveFile("$Path\package.json.etalon")
    RemoveFile("$Path\karma.conf.js")
    RemoveFile("$Path\job_scheduling_data_2_0.xsd")
    RemoveFile("$Path\gulpfile.js")
    RemoveFile("$Path\AdvantShop.Web.Site.csproj.teamcity.msbuild.tcargs")
    RemoveFile("$Path\AdvantShop.Web.Site.csproj.teamcity")
    RemoveFile("$Path\.babelrc")
    RemoveFile("$Path\.browserslistrc")
    RemoveFile("$Path\.npmrc")
    RemoveFile("$Path\postcss.config.js")
    RemoveFile("$Path\webpack.config.js")
    RemoveFile("$Path\webpack.config.dev.js")
    RemoveFile("$Path\webpack.config.prod.js")
    RemoveFile("$Path\webpack.config.rules.js")
    RemoveFile("$Path\webpack.config.shimming.js")
    RemoveFile("$Path\package.json")
    RemoveFile("$Path\package-lock.json")
    RemoveFile("$Path\.eslintignore")
    RemoveFile("$Path\.prettierrc")
    RemoveFile("$Path\.eslintrc.json")
    RemoveFile("$Path\.stylelintrc.json")
    RemoveFile("$Path\app.config")
    RemoveFile("$Path\job_scheduling_data_2_0.xsd")
    RemoveFile("$Path\jest.config.js")
    RemoveFile("$Path\.lintstagedrc.json")
    RemoveFile("$Path\.prettierignore")
    RemoveFile("$Path\.stylelintignore")
    RemoveFile("$Path\.stylelintrc.json")
    RemoveFile("$Path\.editorconfig")
    RemoveFile("$Path\tsconfig.json")
    RemoveFile("$Path\eslint.config.mjs")
    
    Write-Host $LogPadding "!Removing bundle_config, node_modules, node_scripts folders in $Path"
    RemoveFolder("$Path\bundle_config\");
    RemoveFolder("$Path\node_modules\");
    RemoveFolder("$Path\node_scripts\");
    RemoveFolder("$Path\coverage\");
    RemoveFolder("$Path\.git");
    RemoveFolder("$Path\.idea");
    RemoveFolder("$Path\.vs");
    RemoveFolder("$Path\.githooks");

    Write-Host $LogPadding "!Removing 'design' folder in $Path with exlude => .css, images, .config"
    CleanFolder -root "$Path\design" -exclude ('.css', 'images', '.config')

    Write-Host $LogPadding "!Cleaning 'vendors' folder in $Path with exlude => angular, ckeditor"
    CleanFolder -root "$Path\vendors" -exclude ('angular', 'ckeditor', 'jquery.min.js', 'signalr')

    Write-Host $LogPadding "!Removing 'scripts' folder in $Path with exlude => .html"
    CleanFolder -root "$Path\scripts" -exclude ('.html')

    Write-Host $LogPadding "!Removing 'styles' folder in $Path with exlude => .html"
    CleanFolder -root "$Path\styles" -exclude ('.html')
}

function CleanAdminFolder
{
    param(
        [string]$Path,
        [bool]$WithWebpack,
        [bool]$LeavePartialScripts,
        [string]$LogPadding
    )
    Write-Host $LogPadding "!Cleaning" $Path

    RemoveFile("$Path\.stylelintrc.json")
    RemoveFile("$Path\app.config")
    RemoveFile("$Path\job_scheduling_data_2_0.xsd")

    Write-Host $LogPadding "!Removing bundle_config, node_modules, node_scripts folders in $Path"
    RemoveFolder("$Path\bundle_config\");

    Write-Host $LogPadding "!Removing 'Content' folder in $Path with exlude => images, color-schemes, .html, .woff, .woff2"
    CleanFolder -root "$Path\Content" -exclude ('images', 'color-schemes', '.html', '.woff', '.woff2')
}

function CleanLandingFolder
{
    param(
        [string]$Path,
        [bool]$WithWebpack,
        [bool]$LeavePartialScripts,
        [string]$LogPadding
    )
    Write-Host $LogPadding "!Cleaning" $Path

    RemoveFile("$Path\app.config")

    Write-Host $LogPadding "!Removing bundle_config, node_modules, node_scripts folders in $Path"
    RemoveFolder("$Path\bundle_config\");

    Write-Host $LogPadding "!Removing 'Content' folder in $Path with exlude => images, .html"
    CleanFolder -root "$Path\frontend" -exclude ('images', '.html')
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

function CleanPartnersFolder
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

    Write-Host $LogPadding "!Removing 'scripts' folder in $Path with exlude => images, .html"
    CleanFolder -root "$Path\Content" -exclude ('images', '.html')
}

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

try
{
    Write-Host "!Removing .pdb files from bin folder"
    RemoveFilesWithExtension -path "$SitePath\bin\" -extension ".pdb"

    Write-Host "!Removing all .DotSettings files"
    RemoveFilesWithExtension -path "$SitePath" -extension ".DotSettings"

    Write-Host "!Removing all .scss files"
    RemoveFilesWithExtension -path "$SitePath" -extension ".scss"

    Write-Host "!Removing all .nuspec files"
    RemoveFilesWithExtension -path "$SitePath" -extension ".nuspec"
    
    Write-Host "!Removing all .ts files"
    RemoveFilesWithExtension -path "$SitePath" -extension ".ts"

    Write-Host "!Removing all .cjs files"
    RemoveFilesWithExtension -path "$SitePath" -extension ".cjs"

    Write-Host "!Cleaning root folder"
    CleanRootFolder -Path $SitePath -WithWebpack $True -LeavePartialScripts $True -LogPadding $PS_firstLevelLog

    Write-Host "!Cleaning Areas/Admin folder"
    $adminRoot = "$SitePath\Areas\Admin\"
    CleanAdminFolder -Path $adminRoot -WithWebpack $True -LeavePartialScripts $False -LogPadding $PS_firstLevelLog

    $templatesRoot = $adminRoot + '\Templates'
    if (Test-Path $templatesRoot)
    {
        $templates = Get-ChildItem $templatesRoot
        if ($templates.Count -gt 0)
        {
            Write-Host $PS_firstLevelLog "!Cleaning Templates folders"
            foreach ($templatesItem in $templates)
            {
                Write-Host $PS_firstLevelLog "Removing files for $templatesItem"
                $templeteRoot = "$templatesRoot\$templatesItem";
                CleanAdminFolder -Path $templeteRoot -WithWebpack $True -LeavePartialScripts $False -LogPadding $PS_secondLevelLog
            }
        }
        else
        {
            Write-Host $PS_firstLevelLog "!Not found templates for clean"
        }
    }

    Write-Host "!Cleaning Areas/Landing folder"
    $landingRoot = "$SitePath\Areas\Landing\"
    CleanLandingFolder -Path $landingRoot -WithWebpack $True -LeavePartialScripts $False -LogPadding $PS_firstLevelLog

    Write-Host "!Cleaning Areas/Mobile folder"
    $mobileRoot = "$SitePath\Areas\Mobile\"
    CleanMobileFolder -Path $mobileRoot -WithWebpack $True -LeavePartialScripts $False -LogPadding $PS_firstLevelLog

    Write-Host "!Cleaning Areas/Partners folder"
    $partnersRoot = "$SitePath\Areas\Partners\"
    CleanPartnersFolder -Path $partnersRoot -WithWebpack $True -LeavePartialScripts $False -LogPadding $PS_firstLevelLog

    $templatesRoot = $SitePath + '\Templates'
    if (Test-Path $templatesRoot)
    {
        RemoveFolder("$templatesRoot\.git\")
        RemoveFolder("$templatesRoot\.idea\")

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
            Write-Host $PS_firstLevelLog "!Not found templates for clean"
        }
    }
}
catch [Exception]
{
    FormatErrors $Error
    exit 1
}

Write-Host "!Done!"
