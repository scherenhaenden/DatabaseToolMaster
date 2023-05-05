using System.Diagnostics;
using System.Text;
using DatabaseToolMaster.Core.Consoles;
using DatabaseToolMaster.Core.MysqlDump;

namespace DatabaseToolMaster.Tools.MysqlDump;

public class MysqlDumpManager : IMysqlDumpManager
{
    private readonly IProcessCreator _processCreator;

    public MysqlDumpManager(IProcessCreator processCreator)
    {
        _processCreator = processCreator;
    }
    
    public bool IsInstalled()
    {
        var process = _processCreator.CreateProcess("mysqldump", "--version");

        process.Start();
        process.WaitForExit();
        var output = process.StandardOutput.ReadToEnd();
        if(process.ExitCode != 0)
        {
            return false;
        }

        return true;
    }

    public void CreateDumpDirectDump(string databaseName, string outputFilePath)
    {
        string fileName = "backup.sql";
        string dbName = "mydatabase";
        string dbUser = "root";
        string dbPass = "mypassword";

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "mysqldump",
                Arguments = $"--user={dbUser} --password={dbPass} --databases {dbName} > {fileName}",
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

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
        var process = _processCreator.CreateProcess("mysqldump", $"--user={username} --password={password} --databases {databaseName} > {outputFilePath}");

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
        
        
        var process = _processCreator.CreateProcess("mysqldump", consoleInput);
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
        backupData = backupData.Replace($"CREATE DATABASE `{databaseNameOrigin}`;", $"CREATE DATABASE `{databaseTarget}`;");
        backupData = backupData.Replace($"USE `{databaseNameOrigin}`;", $"USE `{databaseTarget}`");
        return backupData;
    }

    public Task LoadBackupToDatabaseCloning(string server, string databaseNameOrigin, string databaseNameTarget, string username,
        string password, string backupData)
    {
        backupData = ReplacerForDataBaseCloning(databaseNameOrigin, databaseNameTarget, backupData);
        return LoadBackupToDatabase(server, databaseNameTarget, username, password, backupData);
    }


    public async Task LoadBackupToDatabase(string server, string databaseName, string username, string password,
        string backupData)
    {

        // Create the command-line arguments for the mysql command.
        var arguments = $"-h{server} -u{username} -p{password}";

        // Create a ProcessStartInfo object to configure the process.
        var startInfo = new ProcessStartInfo
        {
            FileName = "mysql",
            Arguments = arguments,
            RedirectStandardInput = true,
            RedirectStandardOutput = true, // Add this line to be able to read the output
            RedirectStandardError = true, // Add this line to be able to read the error
            UseShellExecute = false
        };

        // Create a new Process object to execute the mysql command.
        using (var process = new Process())
        {
            process.StartInfo = startInfo;

            var started =process.Start();

            // Check if the database exists.
            var checkDatabaseCommand = $"SELECT COUNT(*) FROM `information_schema`.`SCHEMATA` WHERE `SCHEMA_NAME` = '{databaseName}';";
            var checkDatabaseResult = await ExecuteCommand(checkDatabaseCommand, process);
            var databaseExists = checkDatabaseResult.Trim() == "1";

            if (!databaseExists)
            {
                try
                {   started =process.Start();
                    // Create the database.
                    var createDatabaseCommand = $"CREATE DATABASE `{databaseName}`;";
                    await ExecuteCommand(createDatabaseCommand, process);
                    
                }catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                 
            }

            started =process.Start();
            // Use the database.
            var useDatabaseCommand = $"USE `{databaseName}`;";
            await ExecuteCommand(useDatabaseCommand, process);

            started =process.Start();
            // Load the backup data.
            /*using (var writer = process.StandardInput)
            {
                writer.Write(backupData);
            }*/
            
            await ExecuteCommand(backupData, process);

            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new Exception("Failed to load backup data.");
            }
        }
    }
    
    public async Task<string> ExecuteCommand(string command, Process process)
    {
        using (var writer = new StreamWriter(process.StandardInput.BaseStream))
        {
            await writer.WriteLineAsync(command);
            await writer.FlushAsync();
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

        process.WaitForExit();

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