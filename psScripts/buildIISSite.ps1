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

#region Build settings
# Site mode
    [String]$SiteMode = "Lic",
# Setup settings StoreActive
    [String]$ActiveStore = "false",
# ADVANTSHOP lic key
    [String]$LicKey = "",
# ADVANTSHOP client code
    [String]$ClientCode = "",
# Copy default content (pictures and etc)
    [String]$CopyContent = "false",
# Copy default content (pictures and etc)
    [String[]]$AdditionalPicturesFolders = "mobileapp",
# Create DB Bak file
    [String]$CreateDBBakFile = "false",
# Add date to web.config
    [String]$AddDateToWebConfig = "false",
# TechDomain for client side
    [String]$TechDomainStoreUrl = "",
# TechDomain for admin side
    [String]$TechDomainAdminPanelUrl = ""
#endregion Build settings
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
Write-Host $PS_secondLevelLog "Build settings"
Write-Host $PS_secondLevelLog "SiteMode " $SiteMode
Write-Host $PS_secondLevelLog "ActiveStore " $ActiveStore
Write-Host $PS_secondLevelLog "LicKey " $LicKey
Write-Host $PS_secondLevelLog "ClientCode " $ClientCode
Write-Host $PS_secondLevelLog "CopyContent " $CopyContent
Write-Host $PS_secondLevelLog "AdditionalPicturesFolders " $AdditionalPicturesFolders
Write-Host $PS_secondLevelLog "CreateDBBakFile " $CreateDBBakFile
Write-Host $PS_secondLevelLog "AddDateToWebConfig " $AddDateToWebConfig
Write-Host $PS_secondLevelLog "TechDomainStoreUrl " $TechDomainStoreUrl
Write-Host $PS_secondLevelLog "TechDomainAdminPanelUrl " $TechDomainAdminPanelUrl
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
if ($SiteMode -eq $null -or $SiteMode -eq "")
{
    Write-Host $PS_firstLevelLog "SiteMode param is empty"
    exit 1
}
Write-Host "!Params checked"
#endregion Param checks

# Converting params because in case of TeamCity params converts to strings
#region Converting
try
{
    $StoreActiveBool = [System.Convert]::ToBoolean($ActiveStore)
}
catch [FormatException]
{
    Write-Host "!Error on convertion ActiveStore param to boolean - settings will be false"
    $StoreActiveBool = $false
}
try
{
    $ShrinkDatabaseBool = [System.Convert]::ToBoolean($ShrinkDatabase)
}
catch [FormatException]
{
    Write-Host "!Error on convertion ShrinkDatabase param to boolean - settings will be false"
    $ShrinkDatabaseBool = $false
}
try
{
    $CopyContentBool = [System.Convert]::ToBoolean($CopyContent)
}
catch [FormatException]
{
    Write-Host "!Error on convertion CopyContent param to boolean - settings will be false"
    $CopyContentBool = $false
}
try
{
    $CreateDBBakFileBool = [System.Convert]::ToBoolean($CreateDBBakFile)
}
catch [FormatException]
{
    Write-Host "!Error on convertion CreateDBBakFile param to boolean - settings will be false"
    $CreateDBBakFileBool = $false
}
try
{
    $AddDateToWebConfigBool = [System.Convert]::ToBoolean($AddDateToWebConfig)
}
catch [FormatException]
{
    Write-Host "!Error on convertion AddDateToWebConfig param to boolean - settings will be false"
    $AddDateToWebConfigBool = $false
}
#endregion Converting

try
{
    Write-Host "!Restore database"
    RestoreDb $SqlName $SqlDatabaseName $PathToDb $SqlBackupFile

    Write-Host "!Apply sql patches"
    foreach ($f in Get-ChildItem -path $SqlPatchesPath -Filter *.sql | sort-object { [regex]::Replace($_.Name, '\d+', { $args[0].Value.PadLeft(20) }) })
    {
        Write-Host $PS_firstLevelLog "aplly file " $f.fullname $SqlDatabaseName
        SqlFileExec $SqlName $SqlDatabaseName $f.fullname
    }

    if ($CustomSqlScriptPath -ne "")
    {
        Write-Host "!Apply custom sql patches"
        if (Test-path $CustomSqlScriptPath)
        {
            Write-Host $PS_firstLevelLog "aplly file " $CustomSqlScriptPath $SqlDatabaseName
            SqlFileExec $SqlName $SqlDatabaseName $CustomSqlScriptPath
        }
    }

    Write-Host "!Updating [Settings].[Settings]"
    Write-Host $PS_firstLevelLog "!Updating ShopUrl"
    SqlCommandExec $SqlName $SqlDatabaseName "UPDATE Settings.Settings set value = '$IISUrl' where name='ShopUrl'"

    Write-Host $PS_firstLevelLog "!Updating StoreActive"
    if ($StoreActiveBool)
    {
        SqlCommandExec $SqlName $SqlDatabaseName "update [Settings].[Settings] set Value = 'True' where Name = 'StoreActive'"
    }
    else
    {
        SqlCommandExec $SqlName $SqlDatabaseName "update [Settings].[Settings] set Value = 'False' where Name = 'StoreActive'"
    }
	
	if ($SiteMode -ne "Lic")
	{
		Write-Host $PS_firstLevelLog "!Updating photos"
		SqlCommandExec $SqlName $SqlDatabaseName "update [Catalog].[Photo] set PhotoName = 'https://cs71.advantshop.net/' + PhotoName where type = 'product'"
	}
	
    if ($ShrinkDatabaseBool)
    {
        Write-Host "!Shrinking Database"
        $sqlShrinkCommand = "ALTER DATABASE [" + $SqlDatabaseName + "] SET RECOVERY SIMPLE; DBCC SHRINKDATABASE ('" + $SqlDatabaseName + "', 10);"
        Write-Host $sqlShrinkCommand
        SqlCommandExec $SqlName $SqlDatabaseName $sqlShrinkCommand
    }
    if ($CreateDBBakFileBool)
    {
        Write-Host "!Backuping Database"
        $dbBackupFile = $SitePath + "\" + $SiteMode + ".bak"
        BackupDatabase $SqlName $SqlDatabaseName $dbBackupFile
    }

    if ($LicKey -ne "")
    {
        Write-Host $PS_firstLevelLog "!Updating lickey"
        SqlCommandExec $SqlName $SqlDatabaseName "UPDATE Settings.Settings set value = '$LicKey' where name='lickey'; UPDATE Settings.Settings set value = 'True' where name='ActiveLic'"
    }
    if ($ClientCode -ne "")
    {
        Write-Host $PS_firstLevelLog "!Updating ClientCode"
        SqlCommandExec $SqlName $SqlDatabaseName "insert Settings.Settings (name, value) values ('ClientCode','$ClientCode')"
    }
    if ($TechDomainStoreUrl -ne "")
    {
        Write-Host $PS_firstLevelLog "!Updating TechDomainStoreUrl"
        SqlCommandExec $SqlName $SqlDatabaseName "insert Settings.Settings (name, value) values ('TechDomainStore','$TechDomainStoreUrl')"
    }
    if ($TechDomainAdminPanelUrl -ne "")
    {
        Write-Host $PS_firstLevelLog "!Updating TechDomainAdminPanelUrl"
        SqlCommandExec $SqlName $SqlDatabaseName "insert Settings.Settings (name, value) values ('TechDomainAdminPanel','$TechDomainAdminPanelUrl')"
    }
	
	

    if ($CopyContentBool)
    {
        Write-Host "!Copy content"
        Write-Host $PS_firstLevelLog "!pictures folder"
        $fromPath = $CheckoutPath + "\AdvantShop.Web\pictures";
        $toPath = $SitePath + "\pictures";
        $exclude = @("ttt.ttx");
        $excludeMatch = @("qwqwqwq");

        if ($SiteMode -ne "Lic")
        {
            $excludeMatch = @('\product\', '\category\');
        }

        CopyFolderRetry $fromPath $toPath $exclude $excludeMatch

        Write-Host $PS_firstLevelLog "!userfiles folder"
        $fromPath = $CheckoutPath + "\AdvantShop.Web\userfiles";
        $toPath = $SitePath + "\userfiles";
        CopyFolderRetry  $fromPath $toPath

        Write-Host $PS_firstLevelLog "!design folder"
        $fromPath = $CheckoutPath + "\AdvantShop.Web\design";
        $toPath = $SitePath + "\design";
        CopyFolderRetry $fromPath $toPath
		

		Write-Host $PS_firstLevelLog "!template logo"
		$fromPath = $CheckoutPath + "\AdvantShop.Web\templates\modern\images\logo.svg";
		$toPath = $SitePath + "\pictures\logo.svg";
	
		If (Test-Path $fromPath) 
		{
			Copy-Item $fromPath -Destination $toPath
		}
    }
    else
    {
        Write-Host "!Copy content"
        Write-Host $PS_firstLevelLog "!pictures folder"
        $fromPath = $CheckoutPath + "\AdvantShop.Web\pictures";
        $toPath = $SitePath + "\pictures";
        $exclude = @("ttt.ttx");
        $excludeMatch = @("qwqwqwq");
        
        if ($AdditionalPicturesFolders -ne $null)
        {
            foreach ($folder in $AdditionalPicturesFolders)
            {
                $localFromPath = $fromPath + "\" + $folder
                $localToPath = $toPath + "\" + $folder
                Write-Host $PS_secondLevelLog "!copy" $folder
                CopyFolderRetry $localFromPath $localToPath $exclude $excludeMatch
            }
        }
    }

    Write-Host "!Set site mode " $SiteMode
    SetSiteMode $SitePath $SiteMode
    
    Write-Host "!Set connection"
    SetConnectionString $SitePath $SqlName $SqlDatabaseName $SqlUserName $SqlPassword
    Write-Host "!Set public version"
    SetPublicVersion -SitePath $SitePath -AddDate $AddDateToWebConfigBool

    Write-Host "!Create IIS site"
    CreateSite $IISSiteName $SitePath $IISPoolName

    Write-Host "!Removing app_offline.htm"
    if (Test-Path "app_offline.html")
    {
        Remove-Item "app_offline.html"
    }
    if (Test-Path "app_offline.htm")
    {
        Rename-Item "app_offline.htm" "app_offline.html"
    }

    Write-Host "!Ping to site $IISUrl"
    DoRequestRetry "$IISUrl"

    Write-Host "!Ping to site mobile version $IISUrl"
    DoRequestRetry "$IISUrl/?deviceMode=mobile"

    Write-Host "!Ping to admin panel $IISUrl/admin"
    DoRequestRetry "$IISUrl/admin"
}
catch [Exception]
{
    FormatErrors $Error
    exit 1
}