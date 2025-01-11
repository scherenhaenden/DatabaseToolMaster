using System.Diagnostics;
using System.Text;
using DatabaseToolMaster.Core.MysqlDump;

namespace DatabaseToolMaster.Tools.MysqlDump;

public class MysqlDumpManagerFluent: IMysqlDumpManagerFluent
{
    private string _user;
    private string _password;
    private string _database;
    private List<string> _tables = new List<string>();
    private bool _addDropDatabase;
    private bool _addDropTable;
    private bool _addCreateDatabase;
    private bool _addCreateTable;
    private bool _noData;
    private bool _compact;
    private bool _extendedInsert;
    private bool _includeData;
    private bool _includeDropDatabase;
    private bool _includeDropTable;
    private bool _includeAddDropTable;
    private bool _includeAddDropDatabase;
    private bool _includeDefaultCharacterSet;
    private bool _includeCharset;
    private bool _orderByName;
    private bool _orderByPrimaryKey;
    private bool _includeComments;
    private bool _includeCreateDatabase;
    private bool _includeLockTables;
    private bool _useExtendedInsert;
    private bool _noCreateDb;
    private bool _includeRoutines;
    private bool _includeEvents;
    private bool _includeTriggers;
    private string _charset;
    private string _defaultCharacterSet;
    private bool _useSingleTransaction;
    private bool _useQuick;
    private int _maxAllowedPacket;
    
    public IMysqlDumpManagerFluent WithUser(string user)
    {
        _user = user;
        return this;
    }

    public IMysqlDumpManagerFluent WithPassword(string password)
    {
        _password = password;
        return this;
    }

    public IMysqlDumpManagerFluent WithDatabase(string database)
    {
        _database = database;
        return this;
    }

    public IMysqlDumpManagerFluent WithTables(params string[] tableNames)
    {
        _tables.AddRange(tableNames);
        return this;
    }

    public IMysqlDumpManagerFluent WithoutTables(params string[] tableNames)
    {
        foreach (var tableName in tableNames)
            _tables.Remove(tableName);
        return this;
    }


    public IMysqlDumpManagerFluent WithData(bool includeData)
    {
        _noData = !includeData;
        return this;
    }

    public IMysqlDumpManagerFluent WithDropDatabase(bool includeDropDatabase)
    {
        _addDropDatabase = includeDropDatabase;
        return this;
    }

    public IMysqlDumpManagerFluent WithDropTable(bool includeDropTable)
    {
        _addDropTable = includeDropTable;
        return this;
    }

    public IMysqlDumpManagerFluent WithAddDropTable(bool includeAddDropTable)
    {
        _addDropTable = includeAddDropTable;
        return this;
    }

    public IMysqlDumpManagerFluent WithAddDropDatabase(bool includeAddDropDatabase)
    {
        _addDropDatabase = includeAddDropDatabase;
        return this;
    }


    public IMysqlDumpManagerFluent WithComments(bool includeComments)
    {
        _includeComments = includeComments;
        return this;
    }

    public IMysqlDumpManagerFluent WithCreateDatabase(bool includeCreateDatabase)
    {
        _includeCreateDatabase = includeCreateDatabase;
        return this;
    }

    public IMysqlDumpManagerFluent WithLockTables(bool includeLockTables)
    {
        _includeLockTables = includeLockTables;
        return this;
    }

    public IMysqlDumpManagerFluent WithExtendedInsert(bool useExtendedInsert)
    {
        _useExtendedInsert = useExtendedInsert;
        return this;
    }

    public IMysqlDumpManagerFluent WithNoCreateDb(bool noCreateDb)
    {
        _noCreateDb = noCreateDb;
        return this;
    }

    public IMysqlDumpManagerFluent WithRoutines(bool includeRoutines)
    {
        _includeRoutines = includeRoutines;
        return this;
    }

    public IMysqlDumpManagerFluent WithEvents(bool includeEvents)
    {
        _includeEvents = includeEvents;
        return this;
    }

    public IMysqlDumpManagerFluent WithTriggers(bool includeTriggers)
    {
        _includeTriggers = includeTriggers;
        return this;
    }


    public IMysqlDumpManagerFluent WithCharset(string charset)
    {
        _charset = charset;
        return this;
    }

    public IMysqlDumpManagerFluent WithDefaultCharacterSet(string defaultCharacterSet)
    {
        _defaultCharacterSet = defaultCharacterSet;
        return this;
    }

    public IMysqlDumpManagerFluent WithSingleTransaction(bool useSingleTransaction)
    {
        _useSingleTransaction = useSingleTransaction;
        return this;
    }

    public IMysqlDumpManagerFluent WithQuick(bool useQuick)
    {
        _useQuick = useQuick;
        return this;
    }

    public IMysqlDumpManagerFluent WithMaxAllowedPacket(int maxAllowedPacket)
    {
        _maxAllowedPacket = maxAllowedPacket;
        return this;
    }


    public IMysqlDumpManagerFluent WithOrderByName(bool orderByName)
    {
        _orderByName = orderByName;
        return this;
    }

    public IMysqlDumpManagerFluent WithOrderByPrimaryKey(bool orderByPrimaryKey)
    {
        _orderByPrimaryKey = orderByPrimaryKey;
        return this;
    }

    public string Dump(string username, string password, string host, string outputPath)
    {
        // Build the command-line arguments.
        var arguments = BuildArguments();

        if (_orderByName)
            arguments += " --order-by-name";

        if (_orderByPrimaryKey)
            arguments += " --order-by-primary";

        // Create a ProcessStartInfo object to configure the process.
        var startInfo = new ProcessStartInfo
        {
            FileName = "mysqldump",
            Arguments = arguments,
            RedirectStandardInput = true,
            RedirectStandardOutput = true, // Add this line to be able to read the output
            RedirectStandardError = true, // Add this line to be able to read the error
            UseShellExecute = false,
            CreateNoWindow = true
        };

        // Create a new Process object to execute the mysqldump command.
        using (var process = new Process())
        {
            process.StartInfo = startInfo;

            // Redirect the output of the process to a file.
            using (var output = new StreamWriter(outputPath))
            {
                process.OutputDataReceived += (sender, e) =>
                {
                    output.WriteLine(e.Data);
                };

                process.Start();
                process.BeginOutputReadLine();
                process.WaitForExit();
            }

            if (process.ExitCode != 0)
            {
                throw new Exception("Failed to create backup.");
            }
        }

        return outputPath;
    }

    public Stream GetDumpStream(string username, string password, string host)
    {
        var arguments = BuildArguments();
        
        if (_orderByName)
            arguments += " --order-by-name";

        if (_orderByPrimaryKey)
            arguments += " --order-by-primary";

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "mysqldump",
                Arguments = arguments,
                RedirectStandardInput = true,
                RedirectStandardOutput = true, // Add this line to be able to read the output
                RedirectStandardError = true, // Add this line to be able to read the error
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

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

    public string GetDumpText(string username, string password, string host)
    {
        using (var stream = GetDumpStream(username, password, host))
        {
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }

    public IMysqlDumpManagerFluent WithTable(string table)
    {
        _tables.Add(table);
        return this;
    }

    public IMysqlDumpManagerFluent WithDropDatabase()
    {
        _addDropDatabase = true;
        return this;
    }

    public IMysqlDumpManagerFluent WithDropTable()
    {
        _addDropTable = true;
        return this;
    }

    public IMysqlDumpManagerFluent WithCreateDatabase()
    {
        _addCreateDatabase = true;
        return this;
    }

    public IMysqlDumpManagerFluent WithCreateTable()
    {
        _addCreateTable = true;
        return this;
    }

    public IMysqlDumpManagerFluent WithNoData()
    {
        _noData = true;
        return this;
    }

    public IMysqlDumpManagerFluent WithCompact()
    {
        _compact = true;
        return this;
    }

    public IMysqlDumpManagerFluent WithExtendedInsert()
    {
        _extendedInsert = true;
        return this;
    }

    private string BuildArguments()
    {
        var arguments = new StringBuilder();

        if (!string.IsNullOrEmpty(_user))
            arguments.Append($"-u{_user} ");

        if (!string.IsNullOrEmpty(_password))
            arguments.Append($"-p{_password} ");

        if (!string.IsNullOrEmpty(_database))
            arguments.Append($"{_database} ");

        foreach (var table in _tables)
            arguments.Append($"{table} ");

        if (_addDropDatabase)
            arguments.Append("--add-drop-database ");

        if (_addDropTable)
            arguments.Append("--add-drop-table ");

        if (_addCreateDatabase)
            arguments.Append("--add-create-database ");

        if (_addCreateTable)
            arguments.Append("--add-create-table ");

        if (_noData)
            arguments.Append("--no-data ");

        if (_compact)
            arguments.Append("--compact ");

        if (_extendedInsert)
            arguments.Append("--extended-insert ");

        if (_includeComments)
            arguments.Append("--comments ");

        if (_includeCreateDatabase)
            arguments.Append("--create-database ");

        if (_includeLockTables)
            arguments.Append("--lock-tables ");

        if (_useExtendedInsert)
            arguments.Append("--extended-insert ");

        if (_noCreateDb)
            arguments.Append("--no-create-db ");

        if (_includeRoutines)
            arguments.Append("--routines ");

        if (_includeEvents)
            arguments.Append("--events ");

        if (_includeTriggers)
            arguments.Append("--triggers ");

        if (!string.IsNullOrEmpty(_charset))
            arguments.Append($"--default-character-set={_charset} ");

        if (!string.IsNullOrEmpty(_defaultCharacterSet))
            arguments.Append($"--default-character-set={_defaultCharacterSet} ");

        if (_useSingleTransaction)
            arguments.Append("--single-transaction ");

        if (_useQuick)
            arguments.Append("--quick ");

        if (_maxAllowedPacket > 0)
            arguments.Append($"--max_allowed_packet={_maxAllowedPacket} ");

        return arguments.ToString().TrimEnd();
    }


}