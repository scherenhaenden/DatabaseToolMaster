namespace DatabaseToolMaster.Core.MysqlDump;

public interface IMysqlDumpManager
{
    // Detect if mysql dump is installed
    public bool IsInstalled();
    
    // Create a dump of a database
    public void CreateDumpDirectDump(string databaseName, string outputFilePath);
    
    // Create a dump of a database V2
    public void CreateDumpDirectDump(string databaseName, string outputFilePath, string username, string password);
    
    
    public string GetRawOfDump(string databaseName, string username, string password, string host,  bool createDatabase = true);
    
    
    // Create a dump of a database V3 Getstream
    public Stream GetStreamOfDump(string databaseName, string username, string password, string host,  bool createDatabase = true);

    Task LoadBackupToDatabase(string server, string databaseName, string username, string password, string backupData);
    string ReplacerForDataBaseCloning(string databaseNameOrigin, string databaseTarget, string backupData);
    
    Task LoadBackupToDatabaseCloning(string server, string databaseNameOrigin, string databaseNameTarget, string username, string password, string backupData);

}