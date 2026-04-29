using System.Reflection;
using System.Windows.Forms;

namespace WinNUT_Client_Common;

/// <summary>Application-wide services and paths (replaces VB WinNUT_Globals module).</summary>
public static class WinNutGlobals
{
    private const string PersistDataInStartupPathArg = "-PersistDataInStartupPath";
    private static readonly string PreferredDataDirectory = Application.LocalUserAppDataPath;

    public static string ProgramName { get; }
    public static string ProgramVersion { get; }
    public static string ShortProgramVersion { get; }
    public static string GitHubUrl { get; }
    public static string Copyright { get; }
    public static string DataDirectory { get; }

    public static Logger LogFile { get; }
    public static Updater.UpdateUtil UpdateController { get; }
    public static List<string> StrLog { get; } = new List<string>();

    static WinNutGlobals()
    {
        var entry = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
        ProgramName = entry.GetCustomAttribute<AssemblyProductAttribute>()?.Product ?? "WinNUT-Cursed";
        ProgramVersion = entry.GetName().Version?.ToString() ?? "0.0.0.0";
        var firstDot = ProgramVersion.IndexOf('.');
        var secondDot = firstDot >= 0 ? ProgramVersion.IndexOf('.', firstDot + 1) : -1;
        ShortProgramVersion = secondDot > 0 ? ProgramVersion.Substring(0, secondDot) : ProgramVersion;
        GitHubUrl = entry.GetCustomAttribute<AssemblyTrademarkAttribute>()?.Trademark ??
                    "https://github.com/DavidDeeds/WinNUT-Cursed";
        Copyright = entry.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright ?? "";

        LogFile = new Logger(LogLvl.LOG_DEBUG);

        if (Environment.GetCommandLineArgs().Contains(PersistDataInStartupPathArg, StringComparer.OrdinalIgnoreCase))
        {
            LogFile.LogTracing("Detected CommandLineArg to store persistent data to StartupPath.", LogLvl.LOG_DEBUG, null);

            if (IsPathWritable(Application.StartupPath))
            {
                LogFile.LogTracing("Confirmed StartupPath as chosen path.", LogLvl.LOG_DEBUG, null);
                DataDirectory = Application.StartupPath;
            }
            else
            {
                LogFile.LogTracing("Log to StartupPath requested, but path is not writable.", LogLvl.LOG_ERROR, null);
                DataDirectory = PreferredDataDirectory;
            }
        }
        else
        {
            LogFile.LogTracing("Setting DataDirectory to preferred location.", LogLvl.LOG_DEBUG, null);
            DataDirectory = PreferredDataDirectory;
        }

        UpdateController = new Updater.UpdateUtil();
    }

    private static bool IsPathWritable(string path)
    {
        LogFile.LogTracing($"Checking path {path} for writability...", LogLvl.LOG_DEBUG, null);
        try
        {
            var p = Path.Combine(path, Path.GetRandomFileName());
            using (var fs = File.Create(p, 1, FileOptions.DeleteOnClose))
            {
            }

            LogFile.LogTracing("Path is writable.", LogLvl.LOG_DEBUG, null);
            return true;
        }
        catch (Exception)
        {
            LogFile.LogTracing("Path is not writable.", LogLvl.LOG_DEBUG, null);
            return false;
        }
    }
}
