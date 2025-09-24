#--------------------------------------------------
# Project: AdvantShop.NET
# Web site: https:\\www.advantshop.net
#--------------------------------------------------

function RemoveFile($Path)
{
    if (Test-Path $Path)
    {
        Remove-Item $Path
    }
}

function RemoveFilesWithExtension
{
    param(
        [string]$Path,
        [string]$Extension
    )

    if (Test-Path $Path)
    {
        Get-ChildItem $Path -recurse | Where-Object { $_.Extension -eq $Extension } | Remove-Item -Force -Recurse -Confirm:$false
    }
}

function RemoveFolder($Path)
{
    if (Test-Path $Path)
    {
        Get-ChildItem -Path $Path -Recurse | Remove-Item -Force -Recurse -Confirm:$false
        Remove-Item $Path -Force -Recurse
    }
}

function RemoveFolderRetry($Path)
{
    if (Test-Path $Path)
    {
        $retryCount = 5
        For ($i = 0; $i -lt $retryCount; $i++) {
            try
            {
                Get-ChildItem -Path $Path -Recurse | Remove-Item -Force -Recurse -Confirm:$false
                Remove-Item $Path -Force -Recurse
                return
            }
            catch [Exception]
            {
                Write-Host $PS_firstLevelLog "copy err, trying again"
            }
        }
        throw $_.Exception
    }
}

function CleanFolder
{
    param(
        [string]$Root,
        [string[]]$Exclude
    )

    if (!(Test-Path $Root))
    {
        return
    }

    $Root = Resolve-Path $Root

    Push-Location $Root

    $filesToDel = Get-ChildItem $Root -Recurse -File
    $directoriesToDel = Get-ChildItem $Root -Recurse -Directory

    foreach ($exclusion in $Exclude)
    {
        $filesToDel = $filesToDel | Where-Object { $_.fullname -notmatch $exclusion }
        $directoriesToDel = $directoriesToDel | Where-Object { $_.fullname -notmatch $exclusion } 
    }

    foreach ($file in $filesToDel)
    {
        $file | Remove-Item
    }

    foreach ($dir in $directoriesToDel)
    {
        if (Test-Path $dir)
        {
            $filesInDirectoty = Get-ChildItem -Path $dir -Recurse -File;
            if ($null -eq $filesInDirectoty -or $filesInDirectoty.Count -eq 0)
            {
                $dir | Remove-Item -Recurse
            }
        }
    }

    Pop-Location

    if (@(Get-ChildItem $Root ).Count -eq 0)
    {
        $Root | Remove-Item
    }
}

function CopyFolder()
{
    param (
        [string] $from,
        [string] $to,
        $exclude = @("ttt.ttx"),
        $excludeMatch = @("qwqwqwq")
    )

    Write-Host $PS_firstLevelLog "copy " $from " to " $to
    [regex]$excludeMatchRegEx = '(?i)' + (($excludeMatch |ForEach-Object { [regex]::escape($_) }) -join "|") + ''
    Get-ChildItem -Path $from -Recurse -Exclude $exclude |
            Where-Object { $null -eq $excludeMatch -or $_.FullName.Replace($from, "") -notmatch $excludeMatchRegEx } |
            Copy-Item -Destination {
                $len = $from.length
                if ($_.PSIsContainer)
                {
                    Join-Path $to $_.Parent.FullName.Substring($len)
                }
                else
                {
                    Join-Path $to $_.FullName.Substring($len)
                }
            } -Force -Exclude $exclude
}

function CopyFolderRetry()
{
    param (
        [string] $from,
        [string] $to,
        $exclude = @("ttt.ttx"),
        $excludeMatch = @("qwqwqwq")
    )

    CreateFolder $to
    
    $retryCount = 5
    For ($i = 0; $i -lt $retryCount; $i++) {
        try
        {
            CopyFolder $from $to $exclude $excludeMatch
            return
        }
        catch [Exception]
        {
            Write-Host $PS_firstLevelLog "copy err, trying again"
        }
    }
    throw $_.Exception
}

function CreateFolder($outPath)
{
    If (!(Test-Path $outPath)) {
        New-Item -Path $outPath -ItemType Directory
    }
}

function RenameFolder($from , $to)
{
    $temp=$from.Replace('current',$to)
    RemoveFolderRetry($temp)
    Write-Host "rename from " $from " to " $to
    Rename-Item $from $to
}

function ZipWith7Zip($destination, $source)
{
    if (-not (test-path "$env:ProgramFiles\7-Zip\7z.exe")) {throw "$env:ProgramFiles\7-Zip\7z.exe needed"}
    set-alias sz "$env:ProgramFiles\7-Zip\7z.exe"

    sz a -tzip $destination $source
}

function ZipFolder($zipfilename, $sourcedir)
{
    Add-Type -Assembly System.IO.Compression.FileSystem
    $compressionLevel = [System.IO.Compression.CompressionLevel]::Optimal
    [System.IO.Compression.ZipFile]::CreateFromDirectory($sourcedir, $zipfilename, $compressionLevel, $false)
}

function ZipSingle($source, $destination)
{
    [System.Reflection.Assembly]::LoadWithPartialName("System.IO.Compression.FileSystem") | Out-Null
    $zipEntry = "$source" | Split-Path -Leaf
    $zipFile = [System.IO.Compression.ZipFile]::Open($destination, 'Update')
    $compressionLevel = [System.IO.Compression.CompressionLevel]::Optimal
    [System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zipfile, $Source, $zipEntry, $compressionLevel)
    Write-Verbose "Created archive $destination."
}


function RemoveCommonFiles
{
    param (
        [string]$rootPath,
        [bool]$isTemplateWithWebpack
    )
    
    RemoveFile($rootPath +"\Web.ConnectionString.config.etalon")
    RemoveFile($rootPath +"\package.json.etalon")
    RemoveFile($rootPath +"\karma.conf.js")
    RemoveFile($rootPath +"\job_scheduling_data_2_0.xsd")
    RemoveFile($rootPath +"\gulpfile.js")
    RemoveFile($rootPath +"\AdvantShop.Web.Site.csproj.teamcity.msbuild.tcargs")
    RemoveFile($rootPath +"\AdvantShop.Web.Site.csproj.teamcity")
    RemoveFile($rootPath +"\.babelrc")
    RemoveFile($rootPath +"\.browserslistrc")
    RemoveFile($rootPath +"\.npmrc")
    RemoveFile($rootPath +"\postcss.config.js")
    RemoveFile($rootPath +"\webpack.config.js")
    RemoveFile($rootPath +"\webpack.config.dev.js")
    RemoveFile($rootPath +"\webpack.config.prod.js")
    RemoveFile($rootPath +"\webpack.config.rules.js")
    RemoveFile($rootPath +"\package.json")
    RemoveFile($rootPath +"\package-lock.json")
    RemoveFile($rootPath +"\AdvantShop.Web.Site.nuspec")

    RemoveFolder($rootPath +"\bundle_config\");
    RemoveFolder($rootPath +"\node_modules\");
    RemoveFolder($rootPath +"\node_scripts\");

    $pathStyles = "$rootPath\styles\"
    $pathDesign = "$rootPath\design\"
    $pathScripts = "$rootPath\scripts\"

    CleanFolder -root $pathDesign -exclude ('\.css', 'images', '\.config')

    if($isTemplateWithWebpack){
        CleanFolder -root $pathStyles
        CleanFolder -root $pathScripts -exclude ('_common', '_partials', '_mobile', '\.html')
    }else{
        CleanFolder -root $pathStyles -exclude ('\.css')
        CleanFolder -root $pathScripts -exclude ('_common', '_partials', '_mobile\\full-height-mobile', '_mobile\\mobileOverlap\.html', '\.css', 'extendScripts', '\.html')
    }
}

function CreateCodeMaskFile($sourcePath, $codeMasksFile)
{
    #$lengthCopy = $sourcePath.Length
    #$lastIndexOf = $sourcePath.LastIndexOf("\")
    #$lengthCopy=$lengthCopy- $lastIndexOf

    #$fileCodeMask = $codeMasksDirectory + $sourcePath.Substring($lastIndexOf, $lengthCopy ) + ".txt";	
    $fileCodeMask = $codeMasksFile
    $allFiles = [System.IO.Directory]::GetFiles($sourcePath, "*.*",  [System.IO.SearchOption]::AllDirectories);
    $ExclusionFoldersAndFiles =
    @(
        ".svn\",
        "exports\",
        "Export\\",
        "pictures\\",
        "pictures_elbuz\\",
        "pictures_default\\",
        "pictures_deleted\\",
        "pictures_extra\\",
        "price_download\\",
        "price_temp\\",
        "upload_images\\",
        "combine\\",
        "userfiles\\",
        "App_Data\\Lucene\\",
        "App_Data\\errlog\\",
        "App_Data\\notepad\\",
        "App_Data\\publishprofiles",
        "_rev\\",
        "_SQL\\",
        "design\\",
        "info\\",
        "Documentation\\",

        "App_Data\\shopBaseMaskFile.txt",
        "App_Data\\shopCodeMaskFile.txt",
        "App_Data\\bak.sql",
        "App_Data\\dak_code.zip",
        "App_Data\\LogTempData.txt",
        "App_Data\\template.config",
        "scripts\\_localization\\",
        "css\\extra.css",

        "Web.ConnectionString.config",
        "Web.ModeSettings.config",
        "template.config",
        "Yamarket.xml",
        "robots.txt",
        "sitemap.html",
        "sitemap.xml",
        "combined_",
        "install.txt",
        "website.publishproj",
        "app_offline.html",
        "app_offline.htm"
    )

    usingClose ($outputFile = New-Object System.IO.StreamWriter($fileCodeMask, $false, [System.Text.Encoding]::UTF8)) {
        foreach ($advFileName in $allFiles){
            $fileName = $advFileName.Replace($sourcePath + "\", "");
            $any= $false;
            foreach($exclude in $ExclusionFoldersAndFiles)
            {
                if ($fileName.Contains($exclude))
                {
                    $any= $true;
                    break;
                }
            }
            if ($any -eq $true){
                Write-Host "skip " $fileName
                continue
            }

            #skip some
            UsingClose($hashAlg = New-Object System.Security.Cryptography.SHA1Managed) {
                UsingClose($file=New-Object System.IO.FileStream($advFileName,[System.IO.FileMode]::Open, [System.IO.FileAccess]::Read)) {
                    $hash = $hashAlg.ComputeHash($file)
                    $outputFile.WriteLine($fileName+";"+[System.BitConverter]::ToString($hash))
                }
            }
        }
    }
}

function UsingClose
{
    param (
        [System.IDisposable] $inputObject = $( throw "The parameter -inputObject is required." ),
        [ScriptBlock] $scriptBlock = $( throw "The parameter -scriptBlock is required." )
    )

    Try
    {
        &$scriptBlock
    }
    Finally
    {
        if ($inputObject -ne $null)
        {
            if ($null -eq $inputObject.psbase)
            {
                $inputObject.Dispose()
            }
            else
            {
                $inputObject.psbase.Dispose()
            }
        }
    }
}

function GetVersion($sitepath)
{
    Write-Host "read config" ($sitepath + "\web.config")
    $configurationAppSettingXmlPath = "//configuration/appSettings"
    [xml]$configurationDocument = Get-Content -Path ($sitepath + "\web.config")
    $nodeVersion = $configurationDocument.SelectSingleNode($configurationAppSettingXmlPath+"/add[@key='DB_Version']")

    $version = $nodeVersion.value
    return $version
}
Export-ModuleMember -Function  *
