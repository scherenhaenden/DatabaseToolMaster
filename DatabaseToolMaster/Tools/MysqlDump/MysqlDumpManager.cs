using System.Diagnostics;
using System.IO.Pipes;
using System.Text;
using DatabaseToolMaster.Core.Consoles;
using DatabaseToolMaster.Core.MysqlDump;
using DatabaseToolMaster.Tools.Consoles;

namespace DatabaseToolMaster.Tools.MysqlDump;

public class MysqlDumpManager : IMysqlDumpManager
{
    private readonly DetectMysql _detectMysql = new DetectMysql();

    public bool IsInstalled()
    {
        return _detectMysql.IsInstalled();
    }

    public void CreateDumpDirectDump(string databaseName, string outputFilePath)
    {
        string fileName = "backup.sql";
        string dbName = "mydatabase";
        string dbUser = "root";
        string dbPass = "mypassword";
        
        var processCreator = new ProcessCreator();
        var process = processCreator.CreateProcess("mysqldump", $"--user={dbUser} --password={dbPass} --databases {dbName} > {fileName}");
     
        process.Start();
        process.WaitForExit();

        if (process.ExitCode == 0)
        {
            Console.WriteLine("Backup created successfully.");
        }
        else
        {
            Console.WriteLine("Backup creation failed.");
        }
    }

    public void CreateDumpDirectDump(string databaseName, string outputFilePath, string username, string password)
    {
        var processCreator = new ProcessCreator();
        var process = processCreator.CreateProcess("mysqldump", $"--user={username} --password={password} --databases {databaseName} > {outputFilePath}");

        process.Start();
        process.WaitForExit();
        var output = process.StandardOutput.ReadToEnd();
        if(process.ExitCode != 0)
        {
            throw new Exception("Failed to create dump");
        }
    }

    public string GetRawOfDump(string databaseName, string username, string password, string host,  bool createDatabase = true)
    {
        var stream = GetStreamOfDump(databaseName, username, password, host, createDatabase);
        var reader = new StreamReader(stream);
        var output = reader.ReadToEnd();
        
        return output;
    }

    public Stream GetStreamOfDump(string databaseName, string username, string password, string host, bool createDatabase = true)
    {
        var consoleInput = $"--user={username} --password={password} --host={host} --databases {databaseName}";
        
        if(!createDatabase)
        {
            consoleInput += " --no-create-db";
        }
        var processCreator = new ProcessCreator();
        
        var process = processCreator.CreateProcess("mysqldump", consoleInput);
        process.Start();

        var stream = new MemoryStream();
        process.StandardOutput.BaseStream.CopyTo(stream);
        stream.Position = 0;

        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new Exception("Failed to create dump");
        }

        return stream;
    }
    
    public string ReplacerForDataBaseCloning(string databaseNameOrigin, string databaseTarget, string backupData)
    {
        
        // find out which line has "CREATE DATABASE" statement
        var createDatabaseLine = backupData.Split("\n").FirstOrDefault(x => x.Contains("CREATE DATABASE"));
        var lines = backupData.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

        var result = lines.ToList().FirstOrDefault(x => x.ToLower().Contains("CREATE DATABASE".ToLower()));

        if (result != null)
        {
            // create if it doesn't exist
            backupData = backupData.Replace(result, $"CREATE DATABASE IF NOT EXISTS `{databaseTarget}`;");
            //backupData = backupData.Replace(result, $"CREATE DATABASE `{databaseTarget}`;");
        }
        //backupData = backupData.Replace($"CREATE DATABASE `{databaseNameOrigin}`;", $"CREATE DATABASE `{databaseTarget}`;");
        backupData = backupData.Replace($"USE `{databaseNameOrigin}`;", $"USE `{databaseTarget}`;");
        return backupData;
    }

    public Task LoadBackupToDatabaseCloning(string server, string databaseNameOrigin, string databaseNameTarget, string username,
        string password, string backupData)
    {
        backupData = ReplacerForDataBaseCloning(databaseNameOrigin, databaseNameTarget, backupData);
        return LoadBackupToDatabase(server, databaseNameTarget, username, password, backupData);
    }

    public void Test()
    {
        NamedPipeServerStream s = new NamedPipeServerStream("p", PipeDirection.In);
        
        Action<NamedPipeServerStream> a = callBack;
        a.BeginInvoke(s, ar => { }, null);
    }
    
    private void callBack(NamedPipeServerStream pipe)
    {
        while (true)
        {
            pipe.WaitForConnection();
            StreamReader sr = new StreamReader(pipe);
            Console.WriteLine(sr.ReadToEnd());
            pipe.Disconnect();
        }
    }
    
    


    public async Task LoadBackupToDatabase(string server, string databaseName, string username, string password,
        string backupData)
    {

        // Create the command-line arguments for the mysql command.
        var arguments = $"-h{server} -u{username} -p{password}";

        // Create a ProcessStartInfo object to configure the process.
        var startInfo = ProcessStartInfo(arguments);
        
        // Check if the database exists.
        
        var checkDatabaseCommand = $"SELECT COUNT(*) FROM `information_schema`.`SCHEMATA` WHERE `SCHEMA_NAME` = '{databaseName}';";
        var checkDatabaseResult = await ProcessRunnerMethod(arguments , checkDatabaseCommand);
        var databaseExists = checkDatabaseResult.Contains("1") == true;
        
        /*if (!databaseExists)
         {
             try
             { 
                 // Create the database.
                 var createDatabaseCommand = $"CREATE DATABASE `{databaseName}`;";
                 var isDatabaseCreated = await ProcessRunnerMethod(arguments , createDatabaseCommand);

              ;
                 
             }catch(Exception ex)
             {
                 Console.WriteLine(ex.Message);
             }
              
         }*/
        
        /*started =process.Start();
           // Use the database.
           var useDatabaseCommand = $"USE `{databaseName}`;";
           try
           {
               
               var result = await ExecuteCommand(useDatabaseCommand, process);
               Console.WriteLine(result);
           }
           catch (Exception ex)
           {
               Console.WriteLine(ex.Message);
           }*/

        try
        {
            var backupResults = await ProcessRunnerMethod(arguments, backupData);
            Console.WriteLine(backupResults);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private static ProcessStartInfo ProcessStartInfo(string arguments)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "mysql",
            Arguments = arguments,
            RedirectStandardInput = true,
            RedirectStandardOutput = true, // Add this line to be able to read the output
            RedirectStandardError = true, // Add this line to be able to read the error
            UseShellExecute = false
        };
        return startInfo;
    }

    public async Task<string> ProcessRunnerMethod(string arguments, string Commands)
    {
        string result = "";
        using (var process = new Process())
        {
            process.StartInfo = ProcessStartInfo(arguments);
            var started =process.Start();
            //process.WaitForExit();
            started =process.Start();
            
            
            try
            {
                result = await ExecuteCommand(Commands, process);
                Console.WriteLine(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                //throw new Exception("Failed to load backup data.");
                Console.WriteLine("Failed to load backup data.");
            }
        }

        return result;
    }
    
    public async Task<string> ExecuteCommand(string command, Process process)
    {
        int bufferSize = Math.Max(command.Length, 4096)*2;
        using (var writer = new StreamWriter(process.StandardInput.BaseStream,  Encoding.UTF8, bufferSize))
        {
            
            await writer.WriteLineAsync(command);
            writer.AutoFlush = true;
            //await writer.FlushAsync();
        }

        var outputBuilder = new StringBuilder();
        var outputTask = Task.Run(async () =>
        {
            while (!process.StandardOutput.EndOfStream)
            {
                var line = await process.StandardOutput.ReadLineAsync();
                if (line != null) 
                {
                    outputBuilder.AppendLine(line);
                }
            }
        });

        var errorBuilder = new StringBuilder();
        var errorTask = Task.Run(async () =>
        {
            while (!process.StandardError.EndOfStream)
            {
                var line = await process.StandardError.ReadLineAsync();
                if (line != null) 
                {
                    errorBuilder.AppendLine(line);
                }
            }
        });

        await Task.WhenAll(outputTask, errorTask);

        

        if (process.ExitCode != 0)
        {
            throw new Exception($"Failed to execute command. Output: {outputBuilder}, Error: {errorBuilder}");
        }
        

        return outputBuilder.ToString();
    }

    private static string ExecuteCommand_(string command, Process process)
    {
        using (var writer = new StreamWriter(process.StandardInput.BaseStream))
        {
            writer.WriteLine(command);
            writer.Flush();
            process.WaitForExit();
        }

        var output = "";
        while (!process.StandardOutput.EndOfStream)
        {
            var line = process.StandardOutput.ReadLine();
            if (line == null) continue;
            output += line;
        }

        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new Exception("Failed to execute command.");
        }

        return output;
    }


}