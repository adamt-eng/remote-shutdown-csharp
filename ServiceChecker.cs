using System.Diagnostics;

namespace Remote_Shutdown;

internal static class ServiceChecker
{
    internal static bool IsRunningAsService()
    {
        using var process = Process.GetCurrentProcess();
        return GetParentProcess(process) is { ProcessName: "services" };
    }
    internal static Process GetParentProcess(Process process)
    {
        try
        {
            int parentId;
            using (var mo = new System.Management.ManagementObject($"win32_process.handle='{process.Id}'"))
            {
                mo.Get();
                parentId = Convert.ToInt32(mo["ParentProcessId"]);
            }
            return Process.GetProcessById(parentId);
        }
        catch
        {
            return null;
        }
    }
}