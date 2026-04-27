namespace WinNUT_Client_Common.OldParams;

/// <summary>VB: UpgradableParams — migrates legacy registry keys into application settings.</summary>
public class UpgradableParams : WinNUT_Params
{
    private List<PrefSettingPair> _prefSettingsLookup = new()
    {
        new PrefSettingPair("ServerAddress", "NUT_ServerAddress"),
        new PrefSettingPair("Port", "NUT_ServerPort"),
        new PrefSettingPair("UPSName", "NUT_UPSName"),
        new PrefSettingPair("Delay", "NUT_PollIntervalMsec"),
        new PrefSettingPair("NutLogin", "NUT_Username"),
        new PrefSettingPair("NutPassword", "NUT_Password"),
        new PrefSettingPair("AutoReconnect", "NUT_AutoReconnect"),
        new PrefSettingPair("MinInputVoltage", "CAL_VoltInMin"),
        new PrefSettingPair("MaxInputVoltage", "CAL_VoltInMax"),
        new PrefSettingPair("FrequencySupply", "CAL_FreqInNom"),
        new PrefSettingPair("MinInputFrequency", "CAL_FreqInMin"),
        new PrefSettingPair("MaxInputFrequency", "CAL_FreqInMax"),
        new PrefSettingPair("MinOutputVoltage", "CAL_VoltOutMin"),
        new PrefSettingPair("MaxOutputVoltage", "CAL_VoltOutMax"),
        new PrefSettingPair("MinBattVoltage", "CAL_BattVMin"),
        new PrefSettingPair("MaxBattVoltage", "CAL_BattVMax"),
        new PrefSettingPair("MinimizeToTray", "MinimizeToTray"),
        new PrefSettingPair("MinimizeOnStart", "MinimizeOnStart"),
        new PrefSettingPair("CloseToTray", "CloseToTray"),
        new PrefSettingPair("StartWithWindows", "StartWithWindows"),
        new PrefSettingPair("UseLogFile", "LG_LogToFile"),
        new PrefSettingPair("Log Level", "LG_LogLevel"),
        new PrefSettingPair("ShutdownLimitBatteryCharge", "PW_BattChrgFloor"),
        new PrefSettingPair("ShutdownLimitUPSRemainTime", "PW_RuntimeFloor"),
        new PrefSettingPair("ImmediateStopAction", "PW_Immediate"),
        new PrefSettingPair("Follow_FSD", "PW_RespectFSD"),
        new PrefSettingPair("TypeOfStop", "PW_StopType"),
        new PrefSettingPair("DelayToShutdown", "PW_StopDelaySec"),
        new PrefSettingPair("AllowExtendedShutdownDelay", "PW_UserExtendStopTimer"),
        new PrefSettingPair("ExtendedShutdownDelay", "PW_ExtendDelaySec"),
        new PrefSettingPair("VerifyUpdate", "UP_AutoUpdate"),
        new PrefSettingPair("VerifyUpdateAtStart", "UP_CheckAtStart"),
        new PrefSettingPair("DelayBetweenEachVerification", "UP_AutoChkDelay"),
        new PrefSettingPair("StableOrDevBranch", "UP_Branch"),
        new PrefSettingPair("LastDateVerification", "UP_LastCheck")
    };

    public List<PrefSettingPair> PrefSettingsLookup
    {
        get => _prefSettingsLookup;
        set => _prefSettingsLookup = value;
    }

    private int _lastCount = -1;

    public int TotalPrefs
    {
        get
        {
            if (_lastCount != -1)
            {
                return _lastCount;
            }

            _lastCount = 0;
            foreach (var collection in Parameters)
            {
                _lastCount += collection.Value.Count;
            }

            return _lastCount;
        }
    }

    public UpgradableParams()
    {
        Parameters = LoadParams(this);
    }

    public class PrefSettingPair
    {
        public string OldPreferenceName { get; set; } = "";
        public string NewSettingsName { get; set; } = "";

        public PrefSettingPair(string oldPrefName, string newSettingName)
        {
            OldPreferenceName = oldPrefName;
            NewSettingsName = newSettingName;
        }
    }

    internal class UpgradeWorkerArguments
    {
        public bool DoImport { get; }
        public bool DoBackup { get; }
        public bool DoDelete { get; }

        public UpgradeWorkerArguments(bool doImport, bool doBackup, bool doDelete)
        {
            DoImport = doImport;
            DoBackup = doBackup;
            DoDelete = doDelete;
        }
    }

    internal class UpgradeWorkerProgressReport
    {
        public string LogOutput { get; set; } = "";
        public LogLvl LogLevel { get; set; }
        public object? Sender { get; set; }
        public string? LogResourceString { get; set; }

        public UpgradeWorkerProgressReport(string logOutput, LogLvl logLevel, object? sender, string? logRes = null)
        {
            LogOutput = logOutput;
            LogLevel = logLevel;
            Sender = sender;
            LogResourceString = logRes;
        }
    }
}
