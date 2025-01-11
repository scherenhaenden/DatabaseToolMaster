using System.Diagnostics;

namespace DatabaseToolMaster.Core.Consoles;

public interface IProcessCreator
{
    Process CreateProcess(string fileName, string arguments);
}