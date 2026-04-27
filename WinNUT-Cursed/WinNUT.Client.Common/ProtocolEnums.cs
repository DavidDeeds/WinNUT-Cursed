namespace WinNUT_Client_Common;

public enum AppIconIdx
{
    IDX_BATT_0 = 1,
    IDX_BATT_25 = 2,
    IDX_BATT_50 = 4,
    IDX_BATT_75 = 8,
    IDX_BATT_100 = 16,
    IDX_OL = 32,
    WIN_DARK = 64,
    IDX_ICO_OFFLINE = 128,
    IDX_ICO_RETRY = 256,
    IDX_ICO_VIEWLOG = 2001,
    IDX_ICO_DELETELOG = 2002,
    IDX_OFFSET = 1024
}

public enum LogLvl
{
    LOG_NOTICE,
    LOG_WARNING,
    LOG_ERROR,
    LOG_DEBUG
}

public enum AppResxStr
{
    STR_MAIN_RECONNECT,
    STR_MAIN_NOTCONN,
    STR_MAIN_CONN,
    STR_MAIN_OL,
    STR_MAIN_OB,
    STR_MAIN_LOWBAT,
    STR_MAIN_BATOK,
    STR_MAIN_UNKNOWN_UPS,
    STR_MAIN_LOSTCONNECT,
    STR_MAIN_INVALIDLOGIN,
    STR_MAIN_EXITSLEEP,
    STR_MAIN_GOTOSLEEP,
    STR_SHUT_STAT,
    STR_APP_SHUT,
    STR_LOG_PREFS,
    STR_LOG_CONNECTED,
    STR_LOG_CON_FAILED,
    STR_LOG_CON_RETRY,
    STR_LOG_LOGOFF,
    STR_LOG_SHUT_START,
    STR_LOG_SHUT_STOP,
    STR_LOG_NO_UPDATE,
    STR_LOG_UPDATE,
    STR_LOG_NUT_FSD
}

public enum NUTResponse
{
    UNRECOGNIZED,
    OK,
    VAR,
    ACCESSDENIED,
    UNKNOWNUPS,
    VARNOTSUPPORTED,
    CMDNOTSUPPORTED,
    INVALIDARGUMENT,
    INSTCMDFAILED,
    SETFAILED,
    READONLY,
    TOOLONG,
    FEATURENOTSUPPORTED,
    FEATURENOTCONFIGURED,
    ALREADYSSLMODE,
    DRIVERNOTCONNECTED,
    DATASTALE,
    ALREADYLOGGEDIN,
    INVALIDPASSWORD,
    ALREADYSETPASSWORD,
    INVALIDUSERNAME,
    ALREADYSETUSERNAME,
    USERNAMEREQUIRED,
    PASSWORDREQUIRED,
    UNKNOWNCOMMAND,
    INVALIDVALUE,
    BEGINLIST,
    ENDLIST
}

[Flags]
public enum UPS_States
{
    None = 0,
    OL = 1 << 0,
    OB = 1 << 1,
    LB = 1 << 2,
    HB = 1 << 3,
    CHRG = 1 << 4,
    DISCHRG = 1 << 5,
    FSD = 1 << 6,
    BYPASS = 1 << 7,
    CAL = 1 << 8,
    OFF = 1 << 9,
    OVER = 1 << 10,
    TRIM = 1 << 11,
    BOOST = 1 << 12,
    ALARM = 1 << 13,
    ECO = 1 << 14,
    RB = 1 << 15
}

public enum PowerMethod
{
    Unavailable,
    RealPower,
    RealOutputPower,
    RPNomLoadPct,
    InputNomVALoadPct,
    OutputVACalc
}
