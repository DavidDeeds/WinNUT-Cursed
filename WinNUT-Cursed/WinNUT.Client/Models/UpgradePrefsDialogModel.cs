#nullable enable
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using WinNUT_Client.Properties;
using WinNUT_Client_Common;
using WinNUT_Client_Common.OldParams;

namespace WinNUT_Client.Models;

public class UpgradePrefsDialogModel : INotifyPropertyChanged
{
    private readonly BackgroundWorker _upgradeWorker = new()
    {
        WorkerReportsProgress = true,
        WorkerSupportsCancellation = true
    };

    private UpgradableParams? _oldPrefs;
    private readonly Form _parentForm;

    private bool _importPreviousSettings;
    private bool _backupPreviousSettings;
    private bool _deletePreviousSettings;
    private bool _okButtonEnabled;
    private bool _formEnabled;
    private bool _formUseWaitCursor;
    private int _progressPercent;
    private Icon? _icon;

    /// <summary>VB typo preserved for data binding path.</summary>
    public bool ImportPreviousSettigns
    {
        get => _importPreviousSettings;
        set
        {
            _importPreviousSettings = value;
            NotifyPropertyChanged();
        }
    }

    public bool BackupPreviousSettings
    {
        get => _backupPreviousSettings;
        set
        {
            if (_backupPreviousSettings == value)
            {
                return;
            }

            _backupPreviousSettings = value;
            NotifyPropertyChanged();
        }
    }

    public bool DeletePreviousSettings
    {
        get => _deletePreviousSettings;
        set
        {
            _deletePreviousSettings = value;
            NotifyPropertyChanged();
        }
    }

    public bool OKButtonEnabled
    {
        get => _okButtonEnabled;
        set
        {
            if (_okButtonEnabled == value)
            {
                return;
            }

            _okButtonEnabled = value;
            NotifyPropertyChanged();
        }
    }

    public bool FormEnabled
    {
        get => _formEnabled;
        set
        {
            if (_formEnabled == value)
            {
                return;
            }

            _formEnabled = value;
            NotifyPropertyChanged();
        }
    }

    public bool FormUseWaitCursor
    {
        get => _formUseWaitCursor;
        set
        {
            if (_formUseWaitCursor == value)
            {
                return;
            }

            _formUseWaitCursor = value;
            NotifyPropertyChanged();
        }
    }

    public int ProgressPercent
    {
        get => _progressPercent;
        set
        {
            if (value is < 0 or > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(value),
                    "Progress must be reported as a percentage inbetween 0 and 100.");
            }

            _progressPercent = value;
            NotifyPropertyChanged();
        }
    }

    public Icon? Icon
    {
        get => _icon;
        private set
        {
            if (_icon == value)
            {
                return;
            }

            _icon = value;
            NotifyPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public UpgradePrefsDialogModel(Form parentForm)
    {
        _parentForm = parentForm;
        _upgradeWorker.DoWork += ProcessUpgradeWork;
        _upgradeWorker.ProgressChanged += UpgradeProgressChanged;
        _upgradeWorker.RunWorkerCompleted += UpgradeWorkComplete;
        PropertyChanged += CalculateOKButtonState;
    }

    private void NotifyPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public void InitializeProperties()
    {
        FormEnabled = true;
        FormUseWaitCursor = false;
        ImportPreviousSettigns = true;
        BackupPreviousSettings = true;
        Icon = Resources.WinNut;
        CalculateOKButtonState(this, new PropertyChangedEventArgs(null));
    }

    public void BeginUpgradeWorkAsync()
    {
        var workerArgs = new UpgradeWorkerArguments();

        if (BackupPreviousSettings)
        {
            using var saveFileDialog = new SaveFileDialog
            {
                FileName = "WinNUT-Prefs-Export",
                Filter = "Windows Registry files (*.reg)|*.reg|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Title = Resources.UpgradePrefsDialog_BackupLocationTitle
            };

            if (saveFileDialog.ShowDialog() == DialogResult.Cancel)
            {
                WinNutGlobals.LogFile.LogTracing("User cancelled upgrade prefs process while selecting backup target.",
                    LogLvl.LOG_NOTICE, this);
                return;
            }

            workerArgs.BackupPath = saveFileDialog.FileName;
        }

        FormEnabled = false;
        FormUseWaitCursor = true;
        _upgradeWorker.RunWorkerAsync(workerArgs);
    }

    public void CancelButtonClicked()
    {
        WinNutGlobals.LogFile.LogTracing("Handling Cancel button click...", LogLvl.LOG_DEBUG, this);

        if (_upgradeWorker.IsBusy)
        {
            WinNutGlobals.LogFile.LogTracing("Requesting cancellation of upgradeWorker.", LogLvl.LOG_NOTICE, this);
            _upgradeWorker.CancelAsync();
        }
        else
        {
            WinNutGlobals.LogFile.LogTracing("Exiting out of upgrade dialog.", LogLvl.LOG_NOTICE, this,
                Resources.UpgradePrefsDialog_Cancelled);
            _parentForm.DialogResult = DialogResult.Cancel;
            _parentForm.Close();
        }
    }

    private void CalculateOKButtonState(object? sender, PropertyChangedEventArgs? args)
    {
        if (FormEnabled && ImportPreviousSettigns || BackupPreviousSettings || DeletePreviousSettings)
        {
            OKButtonEnabled = true;
        }
        else
        {
            OKButtonEnabled = false;
        }
    }

    private void ProcessUpgradeWork(object? sender, DoWorkEventArgs e)
    {
        var worker = (BackgroundWorker)sender!;
        var args = (UpgradeWorkerArguments)e.Argument!;

        if (!worker.CancellationPending && ImportPreviousSettigns)
        {
            DoImportWork(worker, e);
        }

        if (!worker.CancellationPending && BackupPreviousSettings)
        {
            DoBackupWork(args.BackupPath ?? "");
        }

        if (!worker.CancellationPending && DeletePreviousSettings)
        {
            DoDeleteWork();
        }
    }

    private void UpgradeWorkComplete(object? sender, RunWorkerCompletedEventArgs e)
    {
        _oldPrefs = null;

        FormEnabled = true;
        FormUseWaitCursor = false;

        if (e.Error != null)
        {
            ProgressPercent = 0;
            var localError = string.Format(Resources.UpgradePrefsDialog_ErrorEncountered, e.Error.Message);
            WinNutGlobals.LogFile.LogTracing("UpgradeWorkComplete with error: " + Environment.NewLine + e.Error,
                LogLvl.LOG_ERROR, this, localError);
            MessageBox.Show(localError);
            return;
        }

        if (e.Cancelled)
        {
            ProgressPercent = 0;
            WinNutGlobals.LogFile.LogTracing("Upgrade work was cancelled.", LogLvl.LOG_WARNING, this);
            return;
        }

        ProgressPercent = 100;
        _parentForm.Close();
    }

    private void ReportProgress(BackgroundWorker worker, int percentComplete, string logMsg, LogLvl logLevel,
        object? senderObj, string? logRes = null)
    {
        worker.ReportProgress(percentComplete,
            new UpgradeWorkerProgressReport(logMsg, logLevel, senderObj, logRes));
    }

    private void UpgradeProgressChanged(object? sender, ProgressChangedEventArgs e)
    {
        var progReport = (UpgradeWorkerProgressReport)e.UserState!;
        WinNutGlobals.LogFile.LogTracing(progReport.LogOutput, progReport.LogLevel, progReport.Sender,
            progReport.LogResourceString);
        ProgressPercent = e.ProgressPercentage;
    }

    private void DoImportWork(BackgroundWorker worker, DoWorkEventArgs e)
    {
        ReportProgress(worker, 0, "Import operation beginning.", LogLvl.LOG_NOTICE, this);
        _oldPrefs = new UpgradableParams();
        ReportProgress(worker, 100, "Old parameters loaded.", LogLvl.LOG_NOTICE, this);

        var progress = 0;
        foreach (var section in _oldPrefs.Parameters)
        {
            foreach (var oldPref in section.Value)
            {
                var percentComplete = _oldPrefs.TotalPrefs > 0
                    ? (int)(progress / (double)_oldPrefs.TotalPrefs * 100)
                    : 0;
                try
                {
                    var pairLookupRes = _oldPrefs.PrefSettingsLookup.First(p =>
                        p.OldPreferenceName.Equals(oldPref.Key, StringComparison.Ordinal));

                    var oldValue = oldPref.Value;
                    if (oldPref.Key == "FrequencySupply")
                    {
                        oldValue = Convert.ToInt32(oldValue) * 10 + 50;
                    }

                    try
                    {
                        Settings.Default[pairLookupRes.NewSettingsName] = oldValue!;
                    }
                    catch (SettingsPropertyNotFoundException)
                    {
                        ReportProgress(worker, percentComplete,
                            string.Format("Skipping unknown setting key {0}", pairLookupRes.NewSettingsName),
                            LogLvl.LOG_WARNING, this);
                        continue;
                    }

                    ReportProgress(worker, percentComplete,
                        "Imported " + oldPref.Key + " into " + pairLookupRes.NewSettingsName, LogLvl.LOG_NOTICE, this);

                    _oldPrefs.PrefSettingsLookup.Remove(pairLookupRes);
                    progress++;
                }
                catch (Exception ex)
                {
                    ReportProgress(worker, percentComplete,
                        string.Format("Error importing {0}:{2}{1}", oldPref.Key, Environment.NewLine, ex),
                        LogLvl.LOG_ERROR, this);
                }
            }
        }

        var failedSettingsPairs = _oldPrefs.PrefSettingsLookup;

        if (failedSettingsPairs.Count > 0)
        {
            var unmatchedPairsList = string.Join("",
                failedSettingsPairs.Select(failedPair =>
                    string.Format("[{0}, {1}]", failedPair.OldPreferenceName, failedPair.NewSettingsName)));

            ReportProgress(worker, 95,
                string.Format("{0} unmatched settings pairs: {1}", failedSettingsPairs.Count, unmatchedPairsList),
                LogLvl.LOG_ERROR, this,
                string.Format(Resources.UpgradePrefsDialog_UnmatchedPairs, failedSettingsPairs.Count));
        }

        ReportProgress(worker, 100, "Import procedure complete.", LogLvl.LOG_NOTICE, this,
            Resources.UpgradePrefsDialog_ImportProcedureCompleted);
        Settings.Default.Save();
    }

    private void DoBackupWork(string targetPath)
    {
        ReportProgress(_upgradeWorker, 0, "Beginning reg export process to " + targetPath, LogLvl.LOG_NOTICE, this);
        WinNUT_Params.ExportParams(targetPath);
        ReportProgress(_upgradeWorker, 100, "reg export process exited. Backup complete.", LogLvl.LOG_NOTICE, this,
            string.Format(Resources.UpgradePrefsDialog_BackupProcedureCompleted, targetPath));
    }

    private void DoDeleteWork()
    {
        ReportProgress(_upgradeWorker, 0, "Starting delete procedure.", LogLvl.LOG_NOTICE, this);
        WinNUT_Params.DeleteParams();
        ReportProgress(_upgradeWorker, 100, "Delete procedure completed successfully.", LogLvl.LOG_NOTICE, this,
            Resources.UpgradePrefsDialog_DeleteProcedureComplete);
    }

    private sealed class UpgradeWorkerProgressReport
    {
        public string LogOutput { get; }
        public LogLvl LogLevel { get; }
        public object? Sender { get; }
        public string? LogResourceString { get; }

        public UpgradeWorkerProgressReport(string logOutput, LogLvl logLevel, object? sender, string? logRes = null)
        {
            LogOutput = logOutput;
            LogLevel = logLevel;
            Sender = sender;
            LogResourceString = logRes;
        }
    }

    private sealed class UpgradeWorkerArguments
    {
        public string? BackupPath { get; set; }
    }
}
