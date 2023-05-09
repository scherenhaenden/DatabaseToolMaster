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
        var dbName = "classicmodels";
        var dbUser = "root";
        var dbPass = "blueberrywater4";
        var dbHost = "localhost";
        
        // Create process
        var processCreator = new ProcessCreator();
        
        // Arrange
        var mysqlDumpManager = new MysqlDumpManager();
        
        await mysqlDumpManager.LoadBackupToDatabase(dbHost, dbName, dbUser, dbPass, _backup);
    }
    
    [Test]
    public async Task CloneDump()
    {
        var dbName = "test";
        var dbUser = "root";
        var dbPass = "test";
        var dbHost = "localhost";
        
        

        // Create process
        
        
        // Arrange
        ;
        
        
        _backup = new MysqlDumpManager().GetRawOfDump(dbName, dbUser, dbPass, dbHost);
        await new MysqlDumpManager().LoadBackupToDatabaseCloning(dbHost, dbName, "test1",dbUser, dbPass, _backup);
        
        
        // create a loop to create 100 databases
        for(int i = 0; i < 500; i++)
        {
             await new MysqlDumpManager().LoadBackupToDatabaseCloning(dbHost, dbName, "test1" + (i+600),dbUser, dbPass, _backup);
        }

    }
    
   
    
}