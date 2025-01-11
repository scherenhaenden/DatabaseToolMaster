using DatabaseToolMaster.Core.Platform;

namespace DatabaseToolMaster.Tools.Platform;

public class PlatformDetection : IPlatformDetection
{
    public OSPlatform DetectOS()
    {
        var os = System.Runtime.InteropServices.RuntimeInformation.OSDescription;
        if (os.Contains("Windows"))
        {
            return OSPlatform.Windows;
        }
        else if (os.Contains("Linux"))
        {
            return OSPlatform.Linux;
        }
        else if (os.Contains("MacOS"))
        {
            return OSPlatform.MacOS;
        }
        else if (os.Contains("Android"))
        {
            return OSPlatform.Android;
        }
        else if (os.Contains("iOS"))
        {
            return OSPlatform.iOS;
        }
        else if (os.Contains("FreeBSD"))
        {
            return OSPlatform.FreeBSD;
        }
        else
        {
            return OSPlatform.Other;
        }
    }
}