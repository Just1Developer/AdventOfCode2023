namespace ConsoleApp1;

public class Path
{
    protected const string HOME_PC_PATH =
        "E:\\Coding\\AdventOfCode\\one\\ConsoleApp1\\ConsoleApp1\\input\\";
    protected const string MAC_PATH =
        "/Users/windfury/RiderProjects/AdventOfCode2023/ConsoleApp1/input/";

    // From Upperbound
    #region OS Check

    /// <summary>
    /// 
    /// Gets if the current Operating System is Windows.
    /// 
    /// </summary>
    /// 
    /// <returns>
    ///  Returns true if the OS is Windows. <br/>
    ///  Returns false if the OS is not Windows.
    /// </returns>
    protected static bool IsWindows() => System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);

    #endregion
    
    protected static string PATH
    {
        get => IsWindows() ? HOME_PC_PATH : MAC_PATH;   // add other eventually, but these are my two options so they're all im putting in
    }
}