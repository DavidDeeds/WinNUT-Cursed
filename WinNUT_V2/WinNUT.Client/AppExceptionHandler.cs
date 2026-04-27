using System.Configuration;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.VisualBasic.Devices;
using Newtonsoft.Json;
using WinNUT_Client.Properties;
using WinNUT_Client_Common;
using WinNUT_Client_Common.OldParams;

namespace WinNUT_Client;

/// <summary>Unhandled-exception UI and startup settings validation (VB ApplicationEvents.vb port).</summary>
internal static class AppExceptionHandler
{
    private static readonly CultureInfo DefCulture = CultureInfo.InvariantCulture;
    private static readonly HashSet<string> Sensitive = new(StringComparer.Ordinal)
    {
        "NUT_ServerAddress", "NUT_ServerPort", "NUT_UPSName", "NUT_Username", "NUT_Password"
    };

    private static Form? _crashForm;
    private static Label? _msgCrash;
    private static TextBox? _msgError;
    private static Button? _btnClose;
    private static Button? _btnGenerate;
    private static Exception? _caughtException;

    internal static void RegisterGlobalHandlers()
    {
        AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandledException;
        Application.ThreadException += OnThreadException;
    }

    internal static void RegisterSettingsHandlers()
    {
        ((ApplicationSettingsBase)Settings.Default).SettingsLoaded += OnSettingsFirstLoaded;
        Settings.Default.PropertyChanged += OnSettingsPropertyChanged;
    }

    internal static void RunFirstRunFlow()
    {
        if (!Settings.Default.IsFirstRun)
        {
            return;
        }

        try
        {
            Settings.Default.Upgrade();
            WinNutGlobals.LogFile.LogTracing("Settings upgrade completed without exception.", LogLvl.LOG_NOTICE, null);
        }
        catch (ConfigurationErrorsException ex)
        {
            WinNutGlobals.LogFile.LogTracing("Error encountered while trying to upgrade Settings:", LogLvl.LOG_ERROR, null);
            WinNutGlobals.LogFile.LogException(ex, null);
        }

        if (Settings.Default.IsFirstRun && WinNUT_Params.ParamsExist)
        {
            WinNutGlobals.LogFile.LogTracing("Previous preferences data detected in the Registry.", LogLvl.LOG_NOTICE, null,
                Properties.Resources.DetectedPreviousPrefsData);
            using (var dlg = new UpgradePrefsDialog())
            {
                dlg.ShowDialog();
            }
        }

        Settings.Default.IsFirstRun = false;
        Settings.Default.Save();
    }

    internal static void ApplyLoggingSettings()
    {
        WinNutGlobals.LogFile.IsWritingToFile = Settings.Default.LG_LogToFile;
        WinNutGlobals.LogFile.LogLevelValue = (LogLvl)Settings.Default.LG_LogLevel;
    }

    private static void OnSettingsFirstLoaded(object? sender, SettingsLoadedEventArgs e)
    {
        WinNutGlobals.LogFile.LogTracing("OnSettingsFirstLoaded event raised.", LogLvl.LOG_DEBUG, null);

        try
        {
            _ = Settings.Default.NUT_Username?.ToString();
        }
        catch (Exception ex)
        {
            WinNutGlobals.LogFile.LogTracing("Error attempting to decrypt encrypted data. Resetting to defaults.",
                LogLvl.LOG_ERROR, null, Properties.Resources.Log_Str_ErrorDecrypting);
            WinNutGlobals.LogFile.LogException(ex, null);
            Settings.Default.NUT_Username = new SerializedProtectedString();
            Settings.Default.NUT_Password = new SerializedProtectedString();
        }

        if (Settings.Default.NUT_PollIntervalMsec <= 0)
        {
            WinNutGlobals.LogFile.LogTracing("Incorrect value of " + Settings.Default.NUT_PollIntervalMsec +
                                              " for Poll Delay/Interval, resetting to default.", LogLvl.LOG_ERROR, null);
            Settings.Default.NUT_PollIntervalMsec = 1000;
        }
    }

    private static void OnSettingsPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        WinNutGlobals.LogFile.LogTracing("Handling OnPropertyChanged for " + e.PropertyName, LogLvl.LOG_DEBUG, null);
        if (e.PropertyName is "LG_LogToFile" or "LG_LogLevel")
        {
            WinNutGlobals.LogFile.LogTracing("Settings property changed for logging subsystem, updating...", LogLvl.LOG_DEBUG, null);
            ApplyLoggingSettings();
        }
    }

    private static void OnAppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        WinNutGlobals.LogFile.LogTracing("AppDomainUnhandledException", LogLvl.LOG_ERROR, null);
        var exObj = e.ExceptionObject;
        var wrapped = exObj as Exception;
        if (wrapped == null)
        {
            var desc = exObj == null ? "Nothing" : exObj.ToString();
            wrapped = new Exception("Non-Exception unhandled error: " + desc);
        }

        ShowCrashUi(wrapped);
    }

    private static void OnThreadException(object sender, ThreadExceptionEventArgs e)
    {
        WinNutGlobals.LogFile.LogTracing("ThreadException", LogLvl.LOG_ERROR, null);
        ShowCrashUi(e.Exception);
    }

    private static void ShowCrashUi(Exception ex)
    {
        var main = Application.OpenForms.OfType<WinNUT>().FirstOrDefault();
        if (main != null)
        {
            main.HasCrashed = true;
            main.Hide();
        }

        _caughtException = ex;

        _msgCrash = new Label
        {
            Location = new Point(6, 6),
            Text = "WinNUT has encountered a critical error and will close soon." + Environment.NewLine +
                   "You can :" + Environment.NewLine +
                   "- generate a crash report which will contain most of the configured parameters (without sensitive" + Environment.NewLine +
                   "  information such as your connection information to your NUT server), the last 50 events logged" + Environment.NewLine +
                   "  and the error message displayed below." + Environment.NewLine +
                   "  This information will Then be copied To your clipboard For easy reporting." + Environment.NewLine +
                   "- simply close WinNUT without generating a report.",
            Size = new Size(470, 100)
        };

        _msgError = new TextBox
        {
            Location = new Point(6, 110),
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            ReadOnly = true,
            Text = ex.ToString(),
            Size = new Size(470, 300)
        };

        _btnClose = new Button
        {
            Location = new Point(370, 425),
            TextAlign = ContentAlignment.MiddleCenter,
            Text = "Close WinNUT",
            Size = new Size(100, 25)
        };

        _btnGenerate = new Button
        {
            Location = new Point(160, 425),
            TextAlign = ContentAlignment.MiddleCenter,
            Text = "Generate Report and Close WinNUT",
            Size = new Size(200, 25)
        };

        _crashForm = new Form
        {
            Icon = Properties.Resources.WinNut,
            Size = new Size(500, 500),
            FormBorderStyle = FormBorderStyle.Sizable,
            MaximizeBox = false,
            MinimizeBox = false,
            StartPosition = FormStartPosition.CenterScreen,
            Text = "Critical Error Occurred in WinNUT"
        };
        _crashForm.Controls.Add(_msgCrash);
        _crashForm.Controls.Add(_msgError);
        _crashForm.Controls.Add(_btnClose);
        _crashForm.Controls.Add(_btnGenerate);

        _btnClose!.Click += (_, _) => _crashForm!.Close();
        _btnGenerate!.Click += GenerateButton_Click;

        _crashForm.Show();
        _crashForm.BringToFront();
    }

    private static void GenerateButton_Click(object? sender, EventArgs e)
    {
        var logFileName = "CrashReport_" + DateTime.Now.ToString("s").Replace(":", ".") + ".txt";
        var generatedReport = GenerateCrashReport();

        new Computer().Clipboard.SetText(generatedReport);

        using (var w = new StreamWriter(Path.Combine(WinNutGlobals.DataDirectory, logFileName)))
        {
            w.WriteLine(generatedReport);
        }

        Process.Start("explorer.exe", WinNutGlobals.DataDirectory);
        Environment.Exit(0);
    }

    private static string GenerateCrashReport()
    {
        var jsonSerializerSettings = new JsonSerializerSettings
        {
            Culture = DefCulture,
            Formatting = Formatting.Indented
        };

        var reportStream = new StringWriter(DefCulture);
        reportStream.WriteLine("WinNUT Bug Report");
        reportStream.WriteLine("Generated at " + DateTime.UtcNow.ToString("F", DefCulture));
        reportStream.WriteLine();
        reportStream.WriteLine("OS Version: " + Environment.OSVersion);
        reportStream.WriteLine("WinNUT Version: " + WinNutGlobals.ProgramVersion);

        reportStream.WriteLine();
        reportStream.WriteLine("==== Settings ====");
        reportStream.WriteLine();

        var sensitiveLeft = new HashSet<string>(Sensitive, StringComparer.Ordinal);
        foreach (SettingsProperty setProp in Settings.Default.Properties)
        {
            string setVal;
            if (sensitiveLeft.Contains(setProp.Name))
            {
                setVal = "{Removed}";
                sensitiveLeft.Remove(setProp.Name);
            }
            else
            {
                setVal = Settings.Default[setProp.Name]?.ToString() ?? "";
            }

            reportStream.WriteLine(setProp.Name + ": " + setVal + " (" + setProp.DefaultValue + ")");
        }

        reportStream.WriteLine("==== Exception ====");
        reportStream.WriteLine();
        reportStream.WriteLine(Regex.Unescape(JsonConvert.SerializeObject(_caughtException, jsonSerializerSettings)));
        reportStream.WriteLine();

        reportStream.WriteLine("==== Last Events ====");

        var lastEventsSnapshot = new List<object>(WinNutGlobals.LogFile.LastEvents);
        lastEventsSnapshot.Reverse();
        reportStream.WriteLine();
        reportStream.WriteLine(Regex.Unescape(JsonConvert.SerializeObject(lastEventsSnapshot, jsonSerializerSettings)));

        return reportStream.ToString();
    }
}
