namespace DatabaseToolMaster.Core.Platform;

public interface IPlatformDetection
{
    OSPlatform DetectOS();
}