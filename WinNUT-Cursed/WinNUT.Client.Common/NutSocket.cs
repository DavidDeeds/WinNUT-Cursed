using System.IO;
using System.Net.Sockets;
using System.Runtime.ExceptionServices;

namespace WinNUT_Client_Common;

/// <summary>
/// Manages low-level interaction with an endpoint communicating in the NUT protocol (upsd).
/// </summary>
public class NutSocket
{
    private const int TimeoutMs = 5000;
    private static readonly System.Text.Encoding NutCharEncoding = System.Text.Encoding.ASCII;

    public bool ConnectionStatus => _client is { Connected: true };

    private bool _isLoggedIn;
    public bool IsLoggedIn => _isLoggedIn;

    public string NUTVersion { get; private set; } = "";
    public string NetVersion { get; private set; } = "";

    private readonly Logger _logFile;
    private readonly NutParameter _nutConfig;

    private TcpClient? _client;
    private NetworkStream? _nutStream;
    private StreamReader? _readerStream;
    private StreamWriter? _writerStream;

    private bool _streamInUse;

    public event Action? Socket_Broken;

    public NutSocket(NutParameter nutConfig, Logger logger)
    {
        _logFile = logger;
        _nutConfig = nutConfig;
    }

    public void Connect()
    {
        var host = _nutConfig.Host;
        var port = _nutConfig.Port;

        if (string.IsNullOrEmpty(host) || port == 0)
        {
            throw new InvalidOperationException("Host and Port must be specified to connect.");
        }

        _logFile.LogTracing(string.Format("Attempting TCP socket connection to {0}:{1}...", host, port), LogLvl.LOG_NOTICE, this);

        try
        {
            _client = new TcpClient(host, port)
            {
                SendTimeout = TimeoutMs,
                ReceiveTimeout = TimeoutMs
            };

            _nutStream = _client.GetStream();
            _readerStream = new StreamReader(_nutStream, NutCharEncoding);
            _writerStream = new StreamWriter(_nutStream, NutCharEncoding);

            _logFile.LogTracing("Connection established and streams ready.", LogLvl.LOG_NOTICE, this);
            _logFile.LogTracing("Gathering basic info about the NUT server...", LogLvl.LOG_DEBUG, this);

            try
            {
                var nutQuery = Query_Data("VER");
                NUTVersion = nutQuery.RawResponse;
                _logFile.LogTracing("Server version: " + NUTVersion, LogLvl.LOG_NOTICE, this);
            }
            catch (NutException nutEx)
            {
                _logFile.LogTracing("Error retrieving server version.", LogLvl.LOG_WARNING, this);
                _logFile.LogException(nutEx, this);
            }

            try
            {
                var nutQuery = Query_Data("NETVER");
                NetVersion = nutQuery.RawResponse;
                _logFile.LogTracing("Protocol version: " + NetVersion, LogLvl.LOG_NOTICE, this);
            }
            catch (NutException nutEx)
            {
                _logFile.LogTracing("Error retrieving protocol version.", LogLvl.LOG_WARNING, this);
                _logFile.LogException(nutEx, this);
            }
        }
        catch (Exception ex)
        {
            _logFile.LogTracing("Error connecting socket.", LogLvl.LOG_DEBUG, this);
            _logFile.LogException(ex, this);
            Disconnect(true);
            throw;
        }

        _logFile.LogTracing("Completed gathering basic info about NUT server.", LogLvl.LOG_DEBUG, this);
    }

    public void Login()
    {
        if (_isLoggedIn)
        {
            throw new InvalidOperationException("Attempted to login when already logged in.");
        }

        _logFile.LogTracing(
            string.Format("Logging in to UPS [{0}] as user [{1}] ({2})...",
                _nutConfig.UPSName,
                _nutConfig.Login,
                string.IsNullOrEmpty(_nutConfig.Password) ? "NO Password" : "Password provided"),
            LogLvl.LOG_NOTICE,
            this);

        if (!string.IsNullOrEmpty(_nutConfig.Login))
        {
            Query_Data("USERNAME " + _nutConfig.Login);

            if (!string.IsNullOrEmpty(_nutConfig.Password))
            {
                Query_Data("PASSWORD " + _nutConfig.Password);
            }
        }

        Query_Data("LOGIN " + _nutConfig.UPSName);
        _isLoggedIn = true;
        _logFile.LogTracing("Authenticated successfully.", LogLvl.LOG_NOTICE, this);
    }

    /// <param name="skipLogout">Do not send the LOGOUT command to the NUT server.</param>
    public void Disconnect(bool skipLogout = false)
    {
        if (IsLoggedIn && !skipLogout)
        {
            Query_Data("LOGOUT");
        }

        _isLoggedIn = false;

        _writerStream?.Dispose();
        _writerStream = null;

        _readerStream?.Dispose();
        _readerStream = null;

        _client?.Close();
        _client = null;
    }

    private void OnSocketBroken(Exception? ex)
    {
        _logFile.LogTracing("Socket breaking.", LogLvl.LOG_DEBUG, this);
        Disconnect(true);
        Socket_Broken?.Invoke();
        if (ex != null)
        {
            _logFile.LogException(ex, this);
            ExceptionDispatchInfo.Capture(ex).Throw();
        }
    }

    public Transaction Query_Data(string queryMsg)
    {
        if (!ConnectionStatus)
        {
            throw new InvalidOperationException("Attempted to send query " + queryMsg + " while disconnected.");
        }

        if (_streamInUse)
        {
            throw new InvalidOperationException("Attempted to send query " + queryMsg + " while stream is in use.");
        }

        _streamInUse = true;
        try
        {
            try
            {
                _writerStream!.WriteLine(queryMsg);
                _writerStream.Flush();
            }
            catch (Exception ex)
            {
                _logFile.LogTracing("Error writing to Stream.", LogLvl.LOG_ERROR, this);
                OnSocketBroken(ex);
            }

            string? response = null;
            try
            {
                response = _readerStream!.ReadLine();
            }
            catch (Exception ex)
            {
                _logFile.LogTracing("Error reading from Stream.", LogLvl.LOG_ERROR, this);
                OnSocketBroken(ex);
            }

            if (string.IsNullOrEmpty(response))
            {
                OnSocketBroken(new EndOfStreamException("Server terminated connection."));
            }

            var splitResponse = response!.Split(new[] { ' ' }, 4, StringSplitOptions.None);
            NUTResponse responseEnum;

            switch (splitResponse[0])
            {
                case "OK":
                case "VAR":
                case "DESC":
                case "UPS":
                    responseEnum = NUTResponse.OK;
                    break;
                case "BEGIN":
                    responseEnum = NUTResponse.BEGINLIST;
                    break;
                case "END":
                    responseEnum = NUTResponse.ENDLIST;
                    break;
                case "Network":
                case "1.0":
                case "1.1":
                case "1.2":
                case "1.3":
                    responseEnum = NUTResponse.OK;
                    break;
                case "ERR":
                    var errKind = NUTResponse.UNRECOGNIZED;
                    if (splitResponse.Length >= 2)
                    {
                        var errToken = splitResponse[1].Replace("-", string.Empty);
                        if (!Enum.TryParse(errToken, true, out errKind))
                        {
                            errKind = NUTResponse.UNRECOGNIZED;
                        }
                    }

                    _logFile.LogTracing($"Parsed error response: {errKind}", LogLvl.LOG_DEBUG, this);
                    throw new NutException(new Transaction(queryMsg, response, errKind, splitResponse));
                default:
                    _logFile.LogTracing($"Unrecognized response while parsing: {response}", LogLvl.LOG_ERROR, this);
                    throw new NutException(new Transaction(queryMsg, response, NUTResponse.UNRECOGNIZED, splitResponse));
            }

            return new Transaction(queryMsg, response, responseEnum, splitResponse);
        }
        finally
        {
            _streamInUse = false;
        }
    }

    public List<UPS_List_Datas> Query_List_Datas(string queryMsg)
    {
        var listDatas = new List<string>();
        var listResult = new List<UPS_List_Datas>();

        _ = Query_Data(queryMsg);
        _streamInUse = true;
        try
        {
            while (true)
            {
                var readLine = _readerStream!.ReadLine();
                if (string.IsNullOrEmpty(readLine) || readLine.StartsWith("END", StringComparison.Ordinal))
                {
                    break;
                }

                listDatas.Add(readLine);
            }
        }
        finally
        {
            _streamInUse = false;
        }

        foreach (var line in listDatas)
        {
            var splitString = line.Split(new[] { ' ' }, 4, StringSplitOptions.None);
            if (splitString.Length == 0)
            {
                continue;
            }

            switch (splitString[0])
            {
                case "BEGIN":
                    break;
                case "VAR":
                {
                    var key = splitString[2].Replace("\"", "");
                    var value = splitString[3].Replace("\"", "");
                    var varDesc = GetVarDescription(key);
                    var descParts = varDesc.Replace("\"", "").Split(new[] { ' ' }, 4, StringSplitOptions.None);
                    var desc = descParts.Length > 3 ? descParts[3] : string.Empty;
                    listResult.Add(new UPS_List_Datas { VarKey = key, VarValue = value.Trim(), VarDesc = desc });
                    break;
                }
                case "UPS":
                    listResult.Add(new UPS_List_Datas
                    {
                        VarKey = "UPSNAME",
                        VarValue = splitString[1],
                        VarDesc = splitString[2].Replace("\"", "")
                    });
                    break;
                case "RW":
                {
                    var key = splitString[2].Replace("\"", "");
                    var value = splitString[3].Replace("\"", "");
                    var varDescRw = GetVarDescription(key);
                    var descPartsRw = varDescRw.Replace("\"", "").Split(new[] { ' ' }, 4, StringSplitOptions.None);
                    var descRw = descPartsRw.Length > 3 ? descPartsRw[3] : string.Empty;
                    listResult.Add(new UPS_List_Datas { VarKey = key, VarValue = value.Trim(), VarDesc = descRw });
                    break;
                }
                case "ENUM":
                {
                    var key = splitString[2].Replace("\"", "");
                    var value = splitString[3].Replace("\"", "");
                    var upsName = splitString[1];
                    var varDescEnum = Query_Data("GET DESC " + upsName + " " + key);
                    if (varDescEnum.ResponseType == NUTResponse.OK)
                    {
                        var ep = varDescEnum.RawResponse.Replace("\"", "").Split(new[] { ' ' }, 4, StringSplitOptions.None);
                        var ed = ep.Length > 3 ? ep[3] : string.Empty;
                        listResult.Add(new UPS_List_Datas { VarKey = key, VarValue = value, VarDesc = ed });
                    }
                    else
                    {
                        throw new NutException(varDescEnum);
                    }

                    break;
                }
            }
        }

        return listResult;
    }

    public string GetVarDescription(string varName)
    {
        var nutQuery = Query_Data("GET DESC " + _nutConfig.UPSName + " " + varName);

        if (nutQuery.ResponseType == NUTResponse.OK)
        {
            return nutQuery.RawResponse;
        }

        throw new NutException(nutQuery);
    }
}
