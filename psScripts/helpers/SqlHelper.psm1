#--------------------------------------------------
# Project: AdvantShop.NET
# Web site: https:\\www.advantshop.net
#--------------------------------------------------

Function  SqlFileExec
{
    param
    (
        [string]$SqlServerInstance,
        [string]$SqlDatabaseName,
        [string]$Path
    )

    #Load the required assemlies SMO and SmoExtended.
    [System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SMO") | Out-Null
    [System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SmoExtended") | Out-Null
    [System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.BatchParserClient") | Out-Null

    $sqlServer = New-Object ('Microsoft.SqlServer.Management.Smo.Server') $SqlServerInstance
    $db = $sqlServer.Databases.Item($SqlDatabaseName);
    $reader = New-Object System.IO.StreamReader($Path)
    $script = $reader.ReadToEnd()
    $db.ExecuteNonQuery($script)
}

Function SqlCommandExec
{
    param
    (
        [string]$SqlServerInstance,
        [string]$SqlDatabaseName,
        [string]$Command
    )
    #Load the required assemlies SMO and SmoExtended.
    [System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SMO") | Out-Null
    [System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SmoExtended") | Out-Null

    $sqlServer = New-Object ('Microsoft.SqlServer.Management.Smo.Server') $SqlServerInstance
    $db = $sqlServer.Databases.Item($SqlDatabaseName);
    $db.ExecuteNonQuery($Command)
}

function CreateDb
{
    param(
        [string] $SqlName,
        [string] $SqlDatabaseName,
        [string] $PathToDb
    )
    #Load the required assemlies SMO and SmoExtended.
    [System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SMO") | Out-Null
    [System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SmoExtended") | Out-Null

    $sqlServer = New-Object ('Microsoft.SqlServer.Management.Smo.Server') $SqlName

    $dbExist = $sqlServer.Databases[$SqlDatabaseName]
    if ($null -ne $dbExist)
    {
        return
    }

    $db = New-Object ('Microsoft.SqlServer.Management.Smo.Database') ($SqlName, $SqlDatabaseName)
    $fg = New-Object ('Microsoft.SqlServer.Management.Smo.FileGroup') ($db, "PRIMARY")
    $dataFile = New-Object ('Microsoft.SqlServer.Management.Smo.DataFile') ($fg, $SqlDatabaseName)
    $dataFile.FileName = $PathToDb + "\" + $SqlDatabaseName + ".mdf"
    $dataFile.GrowthType = [Microsoft.SqlServer.Management.Smo.FileGrowthType]::KB
    $dataFile.Growth = 10000
    $dataFile.IsPrimaryFile = $true
    $fg.Files.Add($dataFile)

    $logname = $SqlDatabaseName + "_log"

    $logFile = New-Object ('Microsoft.SqlServer.Management.Smo.LogFile') ($db, $logname)
    $logFile.FileName = $PathToDb + "\" + $SqlDatabaseName + "_log" + ".ldf"
    $logFile.GrowthType = [Microsoft.SqlServer.Management.Smo.FileGrowthType]::Percent
    $logFile.Growth = 10
    $db.LogFiles.Add($logFile)

    $db.FileGroups.Add($fg);
    $db.RecoveryModel = [Microsoft.SqlServer.Management.Smo.RecoveryModel]::Simple;
    $db.Create();
    $sqlServer.Refresh();
}

function RestoreDb
{
    param (
        [string] $SqlName,
        [string] $SqlDatabaseName,
        [string] $PathToDb,
        [string] $BackupFile
    )
    #Load the required assemlies SMO and SmoExtended.
    [System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SMO") | Out-Null
    [System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SmoExtended") | Out-Null

    CreateDb $SqlName $SqlDatabaseName $PathToDb
    $sqlServer = New-Object ('Microsoft.SqlServer.Management.Smo.Server') $SqlName
    $sqlServer.KillAllProcesses($SqlDatabaseName)

    $dbRestore = new-object ("Microsoft.SqlServer.Management.Smo.Restore")
    $dbRestore.Action = [Microsoft.SqlServer.Management.Smo.RestoreActionType]::Database
    $dbRestore.Database = $SqlDatabaseName
    $dbRestore.NoRecovery = $false
    $backupDevice = New-Object("Microsoft.SqlServer.Management.Smo.BackupDeviceItem") ($BackupFile, "File")
    $dbRestore.Devices.Add($backupDevice)

    $fl = $dbRestore.ReadFileList($sqlServer);
    foreach ($row in $fl)
    {
        $relocatFile = New-Object("Microsoft.SqlServer.Management.Smo.RelocateFile")
        $relocatFile.LogicalFileName = $row["LogicalName"]
        $fileType = $row["Type"].ToUpper()
        if ( $fileType.Equals("D"))
        {
            $relocatFile.PhysicalFileName = $PathToDb + "\" + $SqlDatabaseName + ".mdf"
        }
        else
        {
            $relocatFile.PhysicalFileName = $PathToDb + "\" + $SqlDatabaseName + "_log" + ".ldf"
        }
        $dbRestore.RelocateFiles.Add($relocatFile);
    }
    $dbRestore.ReplaceDatabase = $true;
    $dbRestore.SqlRestore($sqlServer);
}

function BackupDatabase
{
    param (
        [string] $SqlName,
        [string] $SqlDatabaseName,
        [string] $BackupFile
    )

    #Load the required assemlies SMO and SmoExtended.
    [System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SMO") | Out-Null
    [System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SmoExtended") | Out-Null

    # Connect SQL Server.
    $sqlServer = New-Object ('Microsoft.SqlServer.Management.Smo.Server') $SqlName

    #Create SMO Backup object instance with the Microsoft.SqlServer.Management.Smo.Backup
    $dbBackup = New-Object ("Microsoft.SqlServer.Management.Smo.Backup")

    $dbBackup.Database = $SqlDatabaseName
    $dbBackup.Action = [Microsoft.SqlServer.Management.Smo.BackupActionType]::Database

    $bkpDevice = New-Object ('Microsoft.SqlServer.Management.Smo.BackupDeviceItem') ($BackupFile, [Microsoft.SqlServer.Management.Smo.DeviceType]::File);

    #Add the backup file to the Devices
    $dbBackup.Devices.Add($bkpDevice)

    #Call the SqlBackup method to complete backup 
    $dbBackup.SqlBackup($sqlServer)
}

Export-ModuleMember -Function  *
