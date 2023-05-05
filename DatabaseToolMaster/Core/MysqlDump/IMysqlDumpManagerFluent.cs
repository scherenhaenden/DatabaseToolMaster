namespace DatabaseToolMaster.Core.MysqlDump;

public interface IMysqlDumpManagerFluent
{
    IMysqlDumpManagerFluent WithUser(string user);
    IMysqlDumpManagerFluent WithPassword(string password);
    IMysqlDumpManagerFluent WithTable(string table);

    IMysqlDumpManagerFluent WithDropDatabase();
    IMysqlDumpManagerFluent WithDatabase(string databaseName);
    IMysqlDumpManagerFluent WithTables(params string[] tableNames);
    IMysqlDumpManagerFluent WithoutTables(params string[] tableNames);
    IMysqlDumpManagerFluent WithData(bool includeData);
    IMysqlDumpManagerFluent WithDropDatabase(bool includeDropDatabase);
    IMysqlDumpManagerFluent WithDropTable(bool includeDropTable);
    IMysqlDumpManagerFluent WithAddDropTable(bool includeAddDropTable);
    IMysqlDumpManagerFluent WithAddDropDatabase(bool includeAddDropDatabase);
    IMysqlDumpManagerFluent WithComments(bool includeComments);
    IMysqlDumpManagerFluent WithCreateDatabase(bool includeCreateDatabase);
    IMysqlDumpManagerFluent WithLockTables(bool includeLockTables);
    IMysqlDumpManagerFluent WithExtendedInsert(bool useExtendedInsert);
    IMysqlDumpManagerFluent WithNoCreateDb(bool noCreateDb);
    IMysqlDumpManagerFluent WithRoutines(bool includeRoutines);
    IMysqlDumpManagerFluent WithEvents(bool includeEvents);
    IMysqlDumpManagerFluent WithTriggers(bool includeTriggers);
    IMysqlDumpManagerFluent WithCharset(string charset);
    IMysqlDumpManagerFluent WithDefaultCharacterSet(string defaultCharacterSet);
    IMysqlDumpManagerFluent WithSingleTransaction(bool useSingleTransaction);
    IMysqlDumpManagerFluent WithQuick(bool useQuick);
    IMysqlDumpManagerFluent WithMaxAllowedPacket(int maxAllowedPacket);
    IMysqlDumpManagerFluent WithOrderByName(bool orderByName);
    IMysqlDumpManagerFluent WithOrderByPrimaryKey(bool orderByPrimaryKey);
    IMysqlDumpManagerFluent WithDropTable();
    IMysqlDumpManagerFluent WithCreateDatabase();
    IMysqlDumpManagerFluent WithCreateTable();
    IMysqlDumpManagerFluent WithNoData();
    IMysqlDumpManagerFluent WithCompact();
    IMysqlDumpManagerFluent WithExtendedInsert();
    string Dump(string username, string password, string host, string outputPath);
    Stream GetDumpStream(string username, string password, string host);
    string GetDumpText(string username, string password, string host);
}