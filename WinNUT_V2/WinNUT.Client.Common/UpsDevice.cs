using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;

namespace WinNUT_Client_Common
{
    /// <summary>
    /// Represents a UPS device on a NUT protocol server (upsd). Is the highest-level object for operations in the
    /// NUT protocol. Will not raise exceptions, only events.
    /// </summary>
    public class UpsDevice
    {
        private static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;
        private const double PowerFactor = 0.8;

        // How many milliseconds to wait before the Reconnect routine tries again.
        private const double DefaultReconnectWaitMs = 5000;

        private const int MaxVarRetries = 3;

        public string Name =>
            Nut_Config != null ? Nut_Config.UPSName : "null";

        public bool IsConnected => _nutSocket.ConnectionStatus;

        public bool IsReconnecting => _reconnectNut.Enabled;

        public bool IsLoggedIn => _nutSocket.IsLoggedIn;

        /// <summary>
        /// How often UPS data is updated, in milliseconds.
        /// </summary>
        public int PollingInterval => _updateData.Interval;

        public bool IsUpdatingData
        {
            get => _updateData.Enabled;
            set
            {
                LogFile.LogTracing("UPS device updating status is now [" + value + "]", LogLvl.LOG_NOTICE, this);
                _updateData.Enabled = value;
            }
        }

        private UPSData _upsData;

        public UPSData UPS_Datas
        {
            get => _upsData;
            private set => _upsData = value;
        }

        private PowerMethod _powerCalculationMethod;

        public PowerMethod PowerCalculationMethod => _powerCalculationMethod;

        public event Action DataUpdated;
        public event Action<UpsDevice> Connected;

        /// <summary>Notify that the connection was closed gracefully.</summary>
        public event Action Disconnected;

        /// <summary>Notify of an unexpectedly lost connection.</summary>
        public event Action Lost_Connect;

        /// <summary>Error encountered when trying to connect.</summary>
        public event Action<UpsDevice, Exception> ConnectionError;

        /// <summary>
        /// Raised when the NUT server returns an error during normal communication and is deemed important for the client
        /// application to know.
        /// </summary>
        /// <param name="sender">The device object that has received the error.</param>
        /// <param name="nutEx">An exception detailing the error and cirucmstances surrounding it.</param>
        public event Action<UpsDevice, NutException> EncounteredNUTException;

        /// <summary>
        /// Raise an event when a status code is added to the UPS that wasn't there before.
        /// </summary>
        /// <param name="newStatuses">The bitmask of status flags that are currently set on the UPS.</param>
        public event Action<UpsDevice, UPS_States> StatusesChanged;

        private readonly System.Windows.Forms.Timer _updateData = new System.Windows.Forms.Timer();
        private readonly System.Windows.Forms.Timer _reconnectNut = new System.Windows.Forms.Timer();
        private readonly NutSocket _nutSocket;

        private double _freqFallback;
        public NutParameter Nut_Config { get; set; }
        private readonly Logger LogFile;

        public UpsDevice(NutParameter nutConfig, Logger logFile, int pollInterval, int defaultFrequency)
        {
            LogFile = logFile;
            Nut_Config = nutConfig;
            _freqFallback = defaultFrequency;
            _nutSocket = new NutSocket(Nut_Config, LogFile);
            _nutSocket.Socket_Broken += OnNutSocketSocketBroken;

            _reconnectNut.Interval = (int)DefaultReconnectWaitMs;
            _reconnectNut.Enabled = false;
            _reconnectNut.Tick += AttemptReconnect;

            _updateData.Interval = pollInterval;
            _updateData.Enabled = false;
            _updateData.Tick += Retrieve_UPS_Datas;
        }

        public void Connect_UPS(bool retryOnConnFailure = false)
        {
            LogFile.LogTracing("Beginning connection: " + Nut_Config, LogLvl.LOG_DEBUG, this);

            try
            {
                _nutSocket.Connect();
                UPS_Datas = GetUPSProductInfo();

                Connected?.Invoke(this);

                if (!string.IsNullOrEmpty(Nut_Config.Login))
                {
                    Login();
                }

                // Have UPS data available right away.
                Retrieve_UPS_Datas(this, EventArgs.Empty);
                _updateData.Start();
            }
            catch (NutException ex)
            {
                // This is how we determine if we have a valid UPS name entered, among other errors.
                EncounteredNUTException?.Invoke(this, ex);
            }
            catch (Exception ex)
            {
                ConnectionError?.Invoke(this, ex);

                if (retryOnConnFailure && !IsReconnecting)
                {
                    LogFile.LogTracing("Reconnection Process Started", LogLvl.LOG_NOTICE, this);
                    _reconnectNut.Start();
                }
            }
        }

        /// <summary>
        /// Indicates to the NUT server that this client is dependant upon this UPS for power, and registers for a FSD event.
        /// Not usually necessary for normal opeartion (reading UPS variables.)
        /// </summary>
        /// <exception cref="Exception">Any exception raised from <see cref="NutSocket.Login"/></exception>
        public void Login()
        {
            if (!IsConnected || IsLoggedIn)
            {
                throw new InvalidOperationException("UPS is in an invalid state to login.");
            }

            if (!string.IsNullOrEmpty(Nut_Config.Login))
            {
                try
                {
                    _nutSocket.Login();
                }
                catch (NutException ex)
                {
                    LogFile.LogTracing("Error while attempting to log in.", LogLvl.LOG_ERROR, this);
                    EncounteredNUTException?.Invoke(this, ex);
                }
            }
        }

        /// <param name="cancelReconnect">When true, stops the reconnect timer if it is running.</param>
        /// <param name="forceful">When true, skips sending LOGOUT to the server (same as <see cref="NutSocket.Disconnect"/>'s skipLogout).</param>
        public void Disconnect(bool cancelReconnect = true, bool forceful = false)
        {
            LogFile.LogTracing("Processing request to disconnect...", LogLvl.LOG_DEBUG, this);

            _updateData.Stop();
            if (cancelReconnect && _reconnectNut.Enabled)
            {
                LogFile.LogTracing("Stopping Reconnect timer.", LogLvl.LOG_DEBUG, this);
                _reconnectNut.Stop();
            }

            try
            {
                _nutSocket.Disconnect(forceful);
            }
            catch (NutException nutEx)
            {
                EncounteredNUTException?.Invoke(this, nutEx);
            }
            catch (Exception ex)
            {
                LogFile.LogTracing("Unexpected exception while Disconnecting.", LogLvl.LOG_ERROR, this);
                LogFile.LogException(ex, this);
            }
            finally
            {
                Disconnected?.Invoke();
            }
        }

        private void OnNutSocketSocketBroken()
        {
            LogFile.LogTracing("Socket has reported a Broken event.", LogLvl.LOG_WARNING, this);
            _updateData.Stop();
            Lost_Connect?.Invoke();

            if (Nut_Config.AutoReconnect)
            {
                LogFile.LogTracing("Reconnection Process Started", LogLvl.LOG_NOTICE, this);
                _reconnectNut.Start();
            }
        }

        private void AttemptReconnect(object sender, EventArgs e)
        {
            LogFile.LogTracing("Attempting reconnection...", LogLvl.LOG_NOTICE, this);
            Connect_UPS();
            if (IsConnected)
            {
                LogFile.LogTracing("Nut Host Reconnected", LogLvl.LOG_NOTICE, this);
                _reconnectNut.Stop();
            }
        }

        /// <summary>
        /// Convenient function to get data that never changes from the UPS.
        /// </summary>
        private UPSData GetUPSProductInfo()
        {
            LogFile.LogTracing("Retrieving basic UPS product information...", LogLvl.LOG_NOTICE, this);

            var freshData = new UPSData(
                GetUPSVar(new[] { "ups.mfr", "device.mfr" }, "Unknown").Trim(),
                GetUPSVar(new[] { "ups.model", "device.model" }, "Unknown").Trim(),
                GetUPSVar(new[] { "ups.serial", "device.serial" }, "Unknown").Trim(),
                GetUPSVar("ups.firmware", "Unknown").Trim());

            var uv = freshData.UPS_Value;
            LogFile.LogTracing("Initializing other well-known UPS variables...", LogLvl.LOG_DEBUG, this);
            try
            {
                var value = float.Parse(GetUPSVar("output.current"), InvariantCulture);
                uv.Output_Current = value;
                LogFile.LogTracing("output.current: " + value, LogLvl.LOG_DEBUG, this);
            }
            catch (Exception ex)
            {
                if (ex.GetType() != typeof(NutException))
                {
                    LogFile.LogException(ex, this);
                }
            }

            try
            {
                var value = float.Parse(GetUPSVar("output.voltage"), InvariantCulture);
                uv.Output_Voltage = value;
                LogFile.LogTracing("output.voltage: " + value, LogLvl.LOG_DEBUG, this);
            }
            catch (Exception ex)
            {
                if (ex.GetType() != typeof(NutException))
                {
                    LogFile.LogException(ex, this);
                }
            }

            try
            {
                var value = float.Parse(GetUPSVar("output.realpower"), InvariantCulture);
                uv.Output_Power = value;
                LogFile.LogTracing("output.power: " + value, LogLvl.LOG_DEBUG, this);
            }
            catch (Exception)
            {
                // intentionally empty
            }

            // Determine optimal method for measuring power output from the UPS.
            LogFile.LogTracing("Determining best method to calculate power usage...", LogLvl.LOG_NOTICE, this);
            // Start with directly reading a variable from the UPS.
            try
            {
                if (freshData.UPS_Value.Output_Power != 0)
                {
                    _powerCalculationMethod = PowerMethod.RealOutputPower;
                    LogFile.LogTracing("Using RealOutputPower method.", LogLvl.LOG_NOTICE, this);
                }
                else
                {
                    GetUPSVar("ups.realpower");
                    _powerCalculationMethod = PowerMethod.RealPower;
                    LogFile.LogTracing("Using RealPower method.", LogLvl.LOG_NOTICE, this);
                }
            }
            catch
            {
                try
                {
                    GetUPSVar("ups.realpower.nominal");
                    GetUPSVar("ups.load");
                    _powerCalculationMethod = PowerMethod.RPNomLoadPct;
                    LogFile.LogTracing("Using RPNomLoadPct method.", LogLvl.LOG_NOTICE, this);
                }
                catch
                {
                    try
                    {
                        GetUPSVar("input.current.nominal");
                        GetUPSVar("input.voltage.nominal");
                        GetUPSVar("ups.load");
                        _powerCalculationMethod = PowerMethod.InputNomVALoadPct;
                        LogFile.LogTracing("Using InputNomVALoadPct method.", LogLvl.LOG_NOTICE, this);
                    }
                    catch
                    {
                        if (freshData.UPS_Value.Output_Current != null &&
                            freshData.UPS_Value.Output_Voltage != 0)
                        {
                            _powerCalculationMethod = PowerMethod.OutputVACalc;
                            LogFile.LogTracing("Using OutputVACalc method.", LogLvl.LOG_NOTICE, this);
                        }
                        else
                        {
                            _powerCalculationMethod = PowerMethod.Unavailable;
                            LogFile.LogTracing("Unable to find a suitable method to calculate power usage.", LogLvl.LOG_WARNING, this);
                        }
                    }
                }
            }

            // Other constant values for UPS calibration.
            freshData.UPS_Value.Batt_Capacity = double.Parse(GetUPSVar("battery.capacity", -1), InvariantCulture);
            _freqFallback = double.Parse(GetUPSVar("output.frequency.nominal", _freqFallback), InvariantCulture);

            LogFile.LogTracing("Completed retrieval of basic UPS product information.", LogLvl.LOG_NOTICE, this);
            return freshData;
        }

        private int _oldStatusBitmask;

        private void Retrieve_UPS_Datas(object sender, EventArgs e)
        {
            LogFile.LogTracing("Enter Retrieve_UPS_Datas", LogLvl.LOG_DEBUG, this);

            try
            {
                string upsRtStatus;

                if (IsConnected)
                {
                    var uv = UPS_Datas.UPS_Value;
                    uv.Batt_Charge = double.Parse(GetUPSVar("battery.charge", -1), InvariantCulture);
                    uv.Batt_Voltage = double.Parse(GetUPSVar("battery.voltage", -1), InvariantCulture);
                    uv.Batt_Runtime = double.Parse(GetUPSVar("battery.runtime", -1), InvariantCulture);
                    uv.Power_Frequency = double.Parse(GetUPSVar("input.frequency", _freqFallback), InvariantCulture);
                    uv.Input_Voltage = double.Parse(GetUPSVar("input.voltage", -1), InvariantCulture);
                    uv.Output_Voltage = double.Parse(GetUPSVar("output.voltage", -1), InvariantCulture);
                    uv.Load = double.Parse(GetUPSVar("ups.load", 0), InvariantCulture);

                    // Retrieve and/or calculate output power if possible.
                    if (_powerCalculationMethod != PowerMethod.Unavailable)
                    {
                        double parsedValue = 0;

                        try
                        {
                            switch (_powerCalculationMethod)
                            {
                                case PowerMethod.RealPower:
                                    parsedValue = double.Parse(GetUPSVar("ups.realpower"), InvariantCulture);
                                    break;

                                case PowerMethod.RealOutputPower:
                                    parsedValue = float.Parse(GetUPSVar("output.realpower"), InvariantCulture);
                                    break;

                                case PowerMethod.RPNomLoadPct:
                                    parsedValue = double.Parse(GetUPSVar("ups.realpower.nominal"), InvariantCulture);
                                    parsedValue *= UPS_Datas.UPS_Value.Load / 100.0;
                                    break;

                                case PowerMethod.InputNomVALoadPct:
                                    var nomCurrent = double.Parse(GetUPSVar("input.current.nominal"), InvariantCulture);
                                    var nomVoltage = double.Parse(GetUPSVar("input.voltage.nominal"), InvariantCulture);

                                    parsedValue = nomCurrent * nomVoltage * PowerFactor;
                                    parsedValue *= UPS_Datas.UPS_Value.Load / 100.0;
                                    break;

                                case PowerMethod.OutputVACalc:
                                    uv.Output_Current = float.Parse(GetUPSVar("output.current"), InvariantCulture);
                                    parsedValue = uv.Output_Current.Value * uv.Output_Voltage * PowerFactor;
                                    break;

                                default:
                                    // Should not trigger - something has gone wrong.
                                    throw new InvalidOperationException(
                                        "Reached Else case when attempting to get power output for method " + _powerCalculationMethod);
                            }
                        }
                        catch (FormatException ex)
                        {
                            LogFile.LogTracing("Unexpected format trying to parse value from UPS. Exception:", LogLvl.LOG_ERROR, this);
                            LogFile.LogTracing(ex.ToString(), LogLvl.LOG_ERROR, this);
                            LogFile.LogTracing("parsedValue: " + parsedValue, LogLvl.LOG_ERROR, this);
                        }
                        catch (Exception ex)
                        {
                            LogFile.LogException(ex, this);
                        }

                        // Apply rounding to this number since calculations have extended to three decimal places.
                        // TODO: Remove this round function once gauges can handle decimal places better.
                        uv.Output_Power = Math.Round(parsedValue, 1);
                    }

                    // Handle out-of-range battery charge
                    if (uv.Batt_Charge < 0 || uv.Batt_Charge > 100)
                    {
                        if (uv.Batt_Voltage > 0)
                        {
                            var nBatt = Math.Floor(uv.Batt_Voltage / 12);
                            uv.Batt_Charge = Math.Floor((uv.Batt_Voltage - 11.6 * nBatt) / (0.02 * nBatt));
                        }
                        else
                        {
                            LogFile.LogTracing(
                                "Unable to calculate UPS Batt_Charge: Batt_Voltage (" + uv.Batt_Voltage + ") out of range.",
                                LogLvl.LOG_WARNING, this);
                        }
                    }

                    // Attempt to calculate battery runtime if not given by the UPS.
                    if (uv.Batt_Runtime == -1)
                    {
                        if (uv.Output_Voltage == -1 || uv.Batt_Voltage == -1 || uv.Batt_Capacity == -1 || uv.Batt_Charge == -1)
                        {
                            LogFile.LogTracing("Unable to calculate battery runtime, missing UPS variables.", LogLvl.LOG_WARNING, this);
                            LogFile.LogTracing(
                                string.Format(
                                    "Output_Voltage: {0}, Batt_Voltage: {1}, Batt_Capacity: {2}, Batt_Charge: {3}",
                                    uv.Output_Voltage, uv.Batt_Voltage, uv.Batt_Capacity, uv.Batt_Charge),
                                LogLvl.LOG_WARNING, this);
                        }
                        else
                        {
                            var powerDivider = 0.5;
                            var load = uv.Load;
                            if (load >= 76 && load <= 100)
                            {
                                powerDivider = 0.4;
                            }
                            else if (load >= 51 && load <= 75)
                            {
                                powerDivider = 0.3;
                            }

                            uv.Load = load != 0 ? load : 0.1;
                            var battInstantCurrent = uv.Output_Voltage * uv.Load / (uv.Batt_Voltage * 100);
                            uv.Batt_Runtime = Math.Floor(
                                uv.Batt_Capacity * 0.6 * uv.Batt_Charge * (1 - powerDivider) * 3600 / (battInstantCurrent * 100));
                        }
                    }

                    upsRtStatus = GetUPSVar("ups.status", UPS_States.None);
                    // Prepare the status string for Enum parsing by replacing spaces with commas.
                    upsRtStatus = upsRtStatus.Replace(" ", ",");
                    try
                    {
                        uv.UPS_Status = (UPS_States)Enum.Parse(typeof(UPS_States), upsRtStatus, true);
                    }
                    catch (ArgumentException ex)
                    {
                        LogFile.LogTracing(
                            "Likely encountered an unknown/invalid UPS status. Using previous status." +
                            Environment.NewLine + ex.Message, LogLvl.LOG_ERROR, this);
                    }

                    // Get the difference between the old and new statuses, and filter only for active ones.
                    var statusDiff = (UPS_States)(((int)_oldStatusBitmask ^ (int)uv.UPS_Status) & (int)uv.UPS_Status);

                    if (statusDiff == 0)
                    {
                        LogFile.LogTracing("UPS statuses have not changed since last update, skipping.", LogLvl.LOG_DEBUG, this);
                    }
                    else
                    {
                        LogFile.LogTracing("UPS statuses have CHANGED...", LogLvl.LOG_NOTICE, this);
                        LogFile.LogTracing("Current statuses: " + upsRtStatus, LogLvl.LOG_NOTICE, this);
                        _oldStatusBitmask = (int)uv.UPS_Status;
                        StatusesChanged?.Invoke(this, statusDiff);
                    }

                    DataUpdated?.Invoke();
                }
            }
            catch (Exception excep)
            {
                LogFile.LogTracing("Something went wrong in Retrieve_UPS_Datas:", LogLvl.LOG_ERROR, this);
                LogFile.LogException(excep, this);
            }
        }

        /// <summary>
        /// Attempts to retrieve the value of a UPS variable using the `GET VAR` NUT API.
        /// </summary>
        /// <param name="varNames">One or more UPS variable name strings to query the server with.</param>
        /// <param name="fallbackValue">Gaurantee a returned value in the event of error. All encountered exceptions during
        /// variable query are suppressed.</param>
        /// <param name="recursing">Special parameter used during DATASTALE error handling.</param>
        /// <exception cref="InvalidOperationException">Attempted to query UPS variable while not <see cref="IsConnected"/>.</exception>
        /// <exception cref="InvalidOperationException">Attempted query with no varNames provided.</exception>
        /// <exception cref="Exception">Any exception raised by <see cref="NutSocket.Query_Data"/>, unless <paramref name="fallbackValue"/> is given.</exception>
        /// <returns>Example: VAR dummy ups.status "OB HB"</returns>
        public string GetUPSVar(string[] varNames, object fallbackValue = null, bool recursing = false)
        {
            if (varNames == null || varNames.Length == 0)
            {
                throw new InvalidOperationException("Attempted GetUPSVar is no names provided.");
            }

            LogFile.LogTracing($"Attempting to get UPS variable with {varNames.Length - 1} alternatives.", LogLvl.LOG_DEBUG, this);

            if (!IsConnected)
            {
                throw new InvalidOperationException("Tried to GetUPSVar while disconnected.");
            }

            Transaction nutQuery = null;
            Exception lastException = null;

            // Try each variable in the array sequentially
            foreach (var varName in varNames)
            {
                var exitVarLoop = false;
                try
                {
                    LogFile.LogTracing("Trying variable: " + varName, LogLvl.LOG_DEBUG, this);
                    nutQuery = _nutSocket.Query_Data($"GET VAR {Name} {varName}");

                    if (nutQuery.SplitResponse != null && nutQuery.SplitResponse.Length == 4)
                    {
                        // Extract the variable value from the response.
                        var response = nutQuery.SplitResponse[3].Trim('"');
                        LogFile.LogTracing("Returning good response: " + response, LogLvl.LOG_DEBUG, this);
                        return response;
                    }

                    throw new Exception("Received unexpected response, but no exception was thrown.");
                }
                catch (Exception ex)
                {
                    lastException = ex;

                    var nutEx = ex as NutException;
                    if (nutEx != null)
                    {
                        switch (nutEx.LastTransaction.ResponseType)
                        {
                            case NUTResponse.VARNOTSUPPORTED:
                                LogFile.LogTracing(varName + " is not supported by server, trying next", LogLvl.LOG_WARNING, this);
                                // Continue to next variable
                                continue;

                            case NUTResponse.DATASTALE:
                                LogFile.LogTracing(
                                    "DATA-STALE Error Result On Retrieving " + varName + " : " + nutEx.LastTransaction.RawResponse,
                                    LogLvl.LOG_ERROR, this);
                                if (recursing)
                                {
                                    // Continue to next variable instead of returning Nothing
                                    continue;
                                }

                                var retryNum = 1;
                                string returnString = null;
                                while (returnString == null && retryNum <= MaxVarRetries)
                                {
                                    LogFile.LogTracing(
                                        "Attempting retry " + retryNum + " to get variable " + varName, LogLvl.LOG_NOTICE, this);
                                    returnString = GetUPSVar(new[] { varName }, fallbackValue, true);
                                    retryNum++;
                                }

                                if (returnString != null)
                                {
                                    return returnString;
                                }

                                // Retry failed, continue to next variable
                                continue;

                            default:
                                LogFile.LogTracing(
                                    "Unexpected NUT error response when retrieving variable: " + ex.Message, LogLvl.LOG_ERROR, this);
                                exitVarLoop = true;
                                break;
                        }
                    }
                    else
                    {
                        LogFile.LogTracing(
                            "Socket or other unexpected error encountered. Expect a SocketBroken event to follow.",
                            LogLvl.LOG_ERROR, this);
                        exitVarLoop = true;
                    }

                    if (exitVarLoop)
                    {
                        break;
                    }
                }
            }


            LogFile.LogTracing("Unable to get any UPS variable.", LogLvl.LOG_ERROR, this);

            if (lastException == null)
            {
                LogFile.LogTracing("!! No exceptions were recorded.", LogLvl.LOG_ERROR, this);
                lastException = new InvalidOperationException("No exceptions were recorded by the end of GetUPSVar.");
            }
            else if (lastException as NutException == null)
            {
                // Print exception info for anyting other than NUT errors.
                LogFile.LogTracing("Last exception recorded:", LogLvl.LOG_ERROR, this);
                LogFile.LogException(lastException, this);
            }

            if (IsFallbackProvided(fallbackValue))
            {
                LogFile.LogTracing("Returning fallback value.", LogLvl.LOG_NOTICE, this);
                return FallbackToString(fallbackValue);
            }

            LogFile.LogTracing("No fallback provided. Throwing last exception.", LogLvl.LOG_ERROR, this);
            throw lastException;
        }

        private static bool IsFallbackProvided(object fallbackValue)
        {
            if (fallbackValue == null)
            {
                return false;
            }

            if (fallbackValue is string s)
            {
                return !string.IsNullOrEmpty(s);
            }

            return true;
        }

        private static string FallbackToString(object fallbackValue)
        {
            if (fallbackValue is string str)
            {
                return str;
            }

            return Convert.ToString(fallbackValue, CultureInfo.InvariantCulture);
        }

        // Overload for backward compatibility with existing code
        public string GetUPSVar(string varName, object fallbackValue = null, bool recursing = false)
        {
            return GetUPSVar(new[] { varName }, fallbackValue, recursing);
        }

        public List<UPS_List_Datas> GetUPS_ListVar()
        {
            var response = new List<UPS_List_Datas>();
            var query = "LIST VAR " + Nut_Config.UPSName;
            LogFile.LogTracing("Enter GetUPS_ListVar", LogLvl.LOG_DEBUG, this);
            if (!IsConnected)
            {
                throw new InvalidOperationException("Attempted to list vars while disconnected.");
            }

            var listVar = _nutSocket.Query_List_Datas(query);

            if (listVar != null)
            {
                response = listVar;
            }

            return response;
        }

        private static string ExtractData(string varData)
        {
            string sanitisedVar;
            string[] stringArray = Array.Empty<string>();
            try
            {
                sanitisedVar = varData.Replace("\"", string.Empty);
                stringArray = sanitisedVar.Split(new[] { ' ' }, 4);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return stringArray[stringArray.Length - 1];
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
