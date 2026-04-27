using System.Diagnostics;
using System.Globalization;
using System.Text;
using Microsoft.VisualBasic.Logging;

namespace WinNUT_Client_Common;

public class Logger
{
    private const LogFileCreationScheduleOption LogFileCreationSchedule = LogFileCreationScheduleOption.Daily;
    private const string Subdirectory = "Logs";
    public const int MaxDisplayedLogs = 50;

#if DEBUG
    private static readonly DateTimeFormatInfo DefaultDateTimeFormat = DateTimeFormatInfo.InvariantInfo;
#else
    private static readonly DateTimeFormatInfo DefaultDateTimeFormat = DateTimeFormatInfo.CurrentInfo;
#endif

    private readonly TraceEventCache _eventCache = new TraceEventCache();

    private FileLogTraceListener? _logFile;
    private readonly List<object> _lastEventsList = new List<object>();
    private readonly Queue<string> _displayedLogs = new Queue<string>(MaxDisplayedLogs);
    private int _displayedLogsCounter;
    private DateTimeFormatInfo _dateTimeFormatInfo = DefaultDateTimeFormat;

    public LogLvl LogLevelValue;

    public event Action<string>? DisplayedLogsLineAdded;
    public event Action<string>? DisplayedLogsTrimmed;

    private int _maxEvents = 200;

    public int MaxEvents
    {
        get => _maxEvents;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Maximum number of events cannot be negative.");
            }

            _maxEvents = value;
        }
    }

    public Queue<string> DisplayedLogs => _displayedLogs;

    public List<object> LastEvents => _lastEventsList;

    public bool IsWritingToFile
    {
        get => _logFile != null;
        set
        {
            if (value != IsWritingToFile)
            {
                if (value)
                {
                    InitializeLogFile();
                }
                else
                {
                    TerminateLogFile();
                }
            }
        }
    }

    public string LogFilePath =>
        IsWritingToFile ? _logFile!.FullLogFileName : throw new InvalidOperationException("Log file has not been created.");

    public DateTimeFormatInfo DateTimeFormatInfo
    {
        get => _dateTimeFormatInfo;
        set => _dateTimeFormatInfo = value;
    }

    public Logger(LogLvl logLevel)
    {
        LogLevelValue = logLevel;
    }

    private void InitializeLogFile()
    {
        LogTracing("InitializeLogFile called...", LogLvl.LOG_DEBUG, this);

        try
        {
            _logFile = new FileLogTraceListener
            {
                TraceOutputOptions = TraceOptions.DateTime | TraceOptions.ProcessId,
                Append = true,
                AutoFlush = true,
                LogFileCreationSchedule = LogFileCreationSchedule,
                CustomLocation = Path.Combine(WinNutGlobals.DataDirectory, Subdirectory),
                Location = LogFileLocation.Custom
            };

            LogTracing($"Attempt init log file: {LogFilePath}", LogLvl.LOG_NOTICE, this);
        }
        catch (Exception ex)
        {
            TerminateLogFile();
            LogException(ex, this);
        }

        if (!IsWritingToFile)
        {
            return;
        }

        if (_lastEventsList.Count > 0)
        {
            _logFile!.WriteLine("==== History of " + _lastEventsList.Count + " previous events ====");
            for (var index = 0; index < _lastEventsList.Count; index++)
            {
                _logFile.WriteLine(string.Format("[{0}] {1}", index + 1, _lastEventsList[index]));
            }
        }

        _logFile!.WriteLine("==== Begin Live Log ====" + Environment.NewLine);
    }

    public void TerminateLogFile()
    {
        if (IsWritingToFile)
        {
            LogTracing("Terminating log file.", LogLvl.LOG_NOTICE, this);
            _logFile!.Close();
            _logFile.Dispose();
            _logFile = null;
        }
        else
        {
            var inv = new InvalidOperationException("Unable to terminate log file - already disabled.");
            LogException(inv, this);
            throw inv;
        }
    }

    public void DeleteLogFile()
    {
        if (IsWritingToFile)
        {
            var fileLocation = _logFile!.FullLogFileName;
            TerminateLogFile();
            File.Delete(fileLocation);
            LogTracing("Log file has been deleted.", LogLvl.LOG_NOTICE, this);
        }
        else
        {
            var inv = new InvalidOperationException("File logging is disabled, unable to delete log file.");
            LogException(inv, this);
            throw inv;
        }
    }

    public void LogTracing(string message, LogLvl lvlError, object? sender, string? logToDisplay = null)
    {
        var finalMsg = FormatLogLine(message, lvlError, sender);

        Trace.WriteLine(finalMsg);

        if (_lastEventsList.Count >= MaxEvents)
        {
            _lastEventsList.RemoveAt(0);
        }

        _lastEventsList.Add(finalMsg);

        if (IsWritingToFile && LogLevelValue >= lvlError)
        {
            _logFile!.WriteLine(finalMsg);
        }

        if (logToDisplay != null)
        {
            if (_displayedLogs.Count >= MaxDisplayedLogs)
            {
                var removedLog = _displayedLogs.Dequeue();
                LogTracing($"Removed log from displayed logs collection: {removedLog}", LogLvl.LOG_DEBUG, this);
                DisplayedLogsTrimmed?.Invoke(removedLog);
            }

            _displayedLogsCounter++;
            var newLogLine = string.Format("[{0}][{1}] {2}", _displayedLogsCounter,
                string.Format(CultureInfo.CurrentCulture, "{0}", DateTime.Now), logToDisplay);
            _displayedLogs.Enqueue(newLogLine);
            LogTracing("Added new line to displayed logs collection: " + newLogLine, LogLvl.LOG_DEBUG, this);
            DisplayedLogsLineAdded?.Invoke(newLogLine);
        }
    }

    public void LogException(Exception ex, object? sender)
    {
        var sb = new StringBuilder();
        sb.AppendLine(ex.GetType() + " thrown in " + ex.Source);
        sb.AppendLine("Message: " + ex.Message);
        sb.AppendLine(ex.StackTrace);

        LogTracing(sb.ToString(), LogLvl.LOG_ERROR, sender);

        if (ex.InnerException != null)
        {
            LogTracing("Inner exception present:", LogLvl.LOG_ERROR, sender);
            LogException(ex.InnerException, ex);
        }

        LogTracing("Exception report complete.", LogLvl.LOG_NOTICE, this);
    }

    private string FormatLogLine(string message, LogLvl logLvl, object? sender = null)
    {
        var pid = _eventCache.ProcessId;
        var senderName = sender != null ? sender.GetType().Name : "Nothing";

        return string.Format("{0} [{1}, {2}]: {3}", DateTime.Now.ToString(_dateTimeFormatInfo), pid, senderName, message);
    }
}
