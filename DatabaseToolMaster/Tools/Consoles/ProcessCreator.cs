using System.Diagnostics;
using DatabaseToolMaster.Core.Consoles;

namespace DatabaseToolMaster.Tools.Consoles;

public class ProcessCreator: IProcessCreator
{
    public Process CreateProcess(string fileName, string arguments)
    {
        return new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardInput = true,
                RedirectStandardOutput = true, // Add this line to be able to read the output
                RedirectStandardError = true, // Add this line to be able to read the error
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
    }
}