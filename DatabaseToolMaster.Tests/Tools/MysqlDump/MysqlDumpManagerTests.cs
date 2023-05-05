using DatabaseToolMaster.Core.MysqlDump;
using DatabaseToolMaster.Tests.Data_For_Tests;
using DatabaseToolMaster.Tools.Consoles;
using DatabaseToolMaster.Tools.MysqlDump;

namespace DatabaseToolMaster.Tests.Tools.MysqlDump;

[TestFixture]
public class MysqlDumpManagerTests
{
    private string _backup = ""; 

    public MysqlDumpManagerTests()
    {
        _backup = RawDataTest.GetRawDataExample();
    }

    [Test]
    public async Task TestDump()
    {
        var dbName = "library";
        var dbUser = "root";
        var dbPass = "mysql_test";
        var dbHost = "localhost";
        
        // Create process
        var processCreator = new ProcessCreator();
        
        // Arrange
        var mysqlDumpManager = new MysqlDumpManager(processCreator);
        
        await mysqlDumpManager.LoadBackupToDatabase(dbHost, dbName, dbUser, dbPass, _backup);
    }
    
   
    
}