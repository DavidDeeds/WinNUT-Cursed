using System.Globalization;
using System.Threading;
using WinNUT_Client.Properties;
using WinNUT_Client_Common;

namespace WinNUT_Client;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        var enUs = CultureInfo.GetCultureInfo("en-US");
        CultureInfo.DefaultThreadCurrentCulture = enUs;
        CultureInfo.DefaultThreadCurrentUICulture = enUs;
        Thread.CurrentThread.CurrentCulture = enUs;
        Thread.CurrentThread.CurrentUICulture = enUs;

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.ApplicationExit += (_, _) => Settings.Default.Save();

        const string mutexName = "WinNUT-Client-SingleInstance";
        using var mutex = new Mutex(true, mutexName, out var createdNew);
        if (!createdNew)
        {
            return;
        }

        AppExceptionHandler.RegisterGlobalHandlers();
        AppExceptionHandler.RegisterSettingsHandlers();

        _ = WinNutGlobals.LogFile;
        WinNutGlobals.LogFile.LogTracing($"{WinNutGlobals.ProgramName} v{WinNutGlobals.ProgramVersion} starting up.",
            LogLvl.LOG_NOTICE, null);
        WinNutGlobals.LogFile.LogTracing("Data storage path: " + WinNutGlobals.DataDirectory, LogLvl.LOG_NOTICE, null);
        WinNutGlobals.LogFile.LogTracing("Event handlers configured.", LogLvl.LOG_DEBUG, null);

        AppExceptionHandler.ApplyLoggingSettings();
        AppExceptionHandler.RunFirstRunFlow();

        Application.Run(new WinNUT());
    }
}
