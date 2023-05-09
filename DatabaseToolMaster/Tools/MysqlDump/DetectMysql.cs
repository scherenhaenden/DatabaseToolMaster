using DatabaseToolMaster.Tools.Consoles;

namespace DatabaseToolMaster.Tools.MysqlDump;

public class DetectMysql
{
    public bool IsInstalled()
    {
        var processCreator = new ProcessCreator();
        var process = processCreator.CreateProcess("mysqldump", "--version");

        process.Start();
        process.WaitForExit();
        var output = process.StandardOutput.ReadToEnd();
        if(process.ExitCode != 0)
        {
            return false;
        }

        return true;
    }
}