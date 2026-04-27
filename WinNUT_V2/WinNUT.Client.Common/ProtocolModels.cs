namespace WinNUT_Client_Common;

public class UPS_Var_Node
{
    public string VarKey = "";
    public string VarValue = "";
    public string VarDesc = "";
}

public class UPS_List_Datas
{
    public string VarKey = "";
    public string VarValue = "";
    public string VarDesc = "";
}

public class UPS_Values
{
    public double Batt_Charge;
    public double Batt_Voltage;
    public double Batt_Runtime;
    public double Input_Voltage;
    public double Output_Voltage;
    public float? Output_Current;
    public double Power_Frequency;
    public double Load;
    public double Output_Power;
    public double Batt_Capacity;
    public UPS_States UPS_Status;
}

public class UPSData
{
    public string Mfr { get; }
    public string Model { get; }
    public string Serial { get; }
    public string Firmware { get; }
    public UPS_Values UPS_Value { get; } = new UPS_Values();

    public UPSData(string mfr, string model, string serial, string firmware)
    {
        Mfr = mfr;
        Model = model;
        Serial = serial;
        Firmware = firmware;
    }
}

public class Transaction
{
    public string Query { get; }
    public NUTResponse ResponseType { get; }
    public string RawResponse { get; }
    public string[]? SplitResponse { get; }

    public Transaction(string query, string response, NUTResponse responseType, string[]? splitResponse = null)
    {
        Query = query;
        RawResponse = response;
        ResponseType = responseType;
        SplitResponse = splitResponse;
    }
}

public class NutException : ApplicationException
{
    public Transaction LastTransaction { get; }

    public NutException(Transaction transaction)
        : base(string.Format("{0} ({1})\r\nQuery: {2}", transaction.ResponseType, transaction.RawResponse, transaction.Query))
    {
        LastTransaction = transaction;
    }
}

public class NutParameter
{
    public string Host = "";
    public int Port;
    public string Login = "";
    public string Password = "";
    public string UPSName = "";
    public bool AutoReconnect;

    public NutParameter(string host, int port, string login, string password, string upsName, bool autoReconnect = false)
    {
        Host = host;
        Port = port;
        Login = login;
        Password = password;
        UPSName = upsName;
        AutoReconnect = autoReconnect;
    }

    public override string ToString()
    {
        return string.Format("{0}@{1}:{2}, Name: {3}{4}", Login, Host, Port, UPSName, AutoReconnect ? " [AutoReconnect]" : "");
    }
}
