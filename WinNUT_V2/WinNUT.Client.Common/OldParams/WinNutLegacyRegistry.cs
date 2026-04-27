using System.Diagnostics;
using System.Globalization;
using Microsoft.Win32;

namespace WinNUT_Client_Common.OldParams;

/// <summary>Previous WinNUT settings in the user registry (VB: WinNUT_Params).</summary>
public class WinNUT_Params
{
    private const string RegKeyRoot = @"SOFTWARE\WinNUT\";

    private static readonly Dictionary<string, Dictionary<string, object>> DefaultParamStructure = new()
    {
        ["Connexion"] = new Dictionary<string, object>
        {
            ["ServerAddress"] = "nutserver host",
            ["Port"] = 3493,
            ["UPSName"] = "UPSName",
            ["Delay"] = 1000,
            ["NutLogin"] = new SerializedProtectedString(string.Empty),
            ["NutPassword"] = new SerializedProtectedString(string.Empty),
            ["AutoReconnect"] = false
        },
        ["Appareance"] = new Dictionary<string, object>
        {
            ["MinimizeToTray"] = false,
            ["MinimizeOnStart"] = false,
            ["CloseToTray"] = false,
            ["StartWithWindows"] = false
        },
        ["Calibration"] = new Dictionary<string, object>
        {
            ["MinInputVoltage"] = 210,
            ["MaxInputVoltage"] = 270,
            ["FrequencySupply"] = 0,
            ["MinInputFrequency"] = 40,
            ["MaxInputFrequency"] = 60,
            ["MinOutputVoltage"] = 210,
            ["MaxOutputVoltage"] = 250,
            ["MinBattVoltage"] = 6,
            ["MaxBattVoltage"] = 18
        },
        ["Power"] = new Dictionary<string, object>
        {
            ["ShutdownLimitBatteryCharge"] = 30,
            ["ShutdownLimitUPSRemainTime"] = 120,
            ["ImmediateStopAction"] = false,
            ["Follow_FSD"] = false,
            ["TypeOfStop"] = 0,
            ["DelayToShutdown"] = 15,
            ["AllowExtendedShutdownDelay"] = false,
            ["ExtendedShutdownDelay"] = 15
        },
        ["Logging"] = new Dictionary<string, object>
        {
            ["UseLogFile"] = false,
            ["Log Level"] = 0
        },
        ["Update"] = new Dictionary<string, object>
        {
            ["VerifyUpdate"] = false,
            ["VerifyUpdateAtStart"] = false,
            ["DelayBetweenEachVerification"] = 2,
            ["StableOrDevBranch"] = 0,
            ["LastDateVerification"] = DateTime.MinValue
        }
    };

    public Dictionary<string, Dictionary<string, object>> Parameters { get; protected set; } =
        new Dictionary<string, Dictionary<string, object>>();

    public static RegistryKey? RegistryKeyRoot => Registry.CurrentUser.OpenSubKey(RegKeyRoot, true);

    public static bool ParamsExist => RegistryKeyRoot != null;

    protected static Dictionary<string, Dictionary<string, object>> LoadParams(object callingObj)
    {
        var newParams = new Dictionary<string, Dictionary<string, object>>();
        var root = RegistryKeyRoot;
        if (root == null)
        {
            WinNutGlobals.LogFile.LogTracing("Failed to open the root WinNUT key.", LogLvl.LOG_ERROR, callingObj);
            return newParams;
        }

        foreach (var paramStructFolder in DefaultParamStructure)
        {
            using var regKeyFolder = root.OpenSubKey(paramStructFolder.Key);
            if (regKeyFolder != null)
            {
                var paramFolder = new Dictionary<string, object>();
                foreach (var paramItem in paramStructFolder.Value)
                {
                    try
                    {
                        var raw = regKeyFolder.GetValue(paramItem.Key);
                        var converted = CoerceRegistryValue(raw, paramItem.Value);
                        if (converted != null)
                        {
                            paramFolder.Add(paramItem.Key, converted);
                        }

                        WinNutGlobals.LogFile.LogTracing("Loaded parameter " + paramItem.Key, LogLvl.LOG_NOTICE, callingObj);
                    }
                    catch (Exception ex)
                    {
                        WinNutGlobals.LogFile.LogTracing(
                            string.Format("Failed to load value from Registry [{0}\\{1}]:{2}{3}",
                                regKeyFolder.Name, paramItem.Key, Environment.NewLine, ex.Message),
                            LogLvl.LOG_WARNING, callingObj);
                    }
                }

                newParams.Add(paramStructFolder.Key, paramFolder);
            }
            else
            {
                WinNutGlobals.LogFile.LogTracing("Failed to open or create Registry Key for " + paramStructFolder.Key,
                    LogLvl.LOG_WARNING, callingObj);
            }
        }

        return newParams;
    }

    private static object? CoerceRegistryValue(object? raw, object template)
    {
        if (raw == null)
        {
            return null;
        }

        if (template is SerializedProtectedString)
        {
            return (SerializedProtectedString)(raw.ToString() ?? string.Empty);
        }

        var targetType = template.GetType();
        if (targetType == typeof(DateTime) && raw is string s)
        {
            return DateTime.Parse(s, CultureInfo.InvariantCulture);
        }

        return Convert.ChangeType(raw, targetType, CultureInfo.InvariantCulture);
    }

    public static void ExportParams(string destinationPath)
    {
        if (File.Exists(destinationPath))
        {
            File.Delete(destinationPath);
        }

        var root = RegistryKeyRoot ?? throw new InvalidOperationException("Registry root not available.");
        var regExpProcStartInfo = new ProcessStartInfo
        {
            FileName = "reg.exe",
            UseShellExecute = false,
            RedirectStandardError = true,
            CreateNoWindow = true,
            Arguments = "export \"" + root.Name + "\" \"" + destinationPath + "\""
        };

        using var proc = Process.Start(regExpProcStartInfo)!;
        proc.WaitForExit();
        if (proc.ExitCode == 1)
        {
            throw new InvalidOperationException("reg.exe encountered an error: " + proc.StandardError.ReadToEnd());
        }
    }

    public static void DeleteParams()
    {
        RegistryKeyRoot?.DeleteSubKeyTree(string.Empty);
    }
}
