using System.Security.Cryptography;
using System.Text;

namespace WinNUT_Client_Common;

public class SerializedProtectedString
{
    private string _protectedString = "";

    public string ProtectedValue
    {
        get => _protectedString;
        set => _protectedString = value;
    }

    private string UnprotectedValue
    {
        get => Unprotect(_protectedString);
        set => _protectedString = Convert.ToBase64String(ProtectedData.Protect(Encoding.Unicode.GetBytes(value),
            null!, DataProtectionScope.CurrentUser));
    }

    public SerializedProtectedString(string data, bool alreadyEncrypted = false)
    {
        if (alreadyEncrypted)
        {
            _protectedString = data;
        }
        else
        {
            UnprotectedValue = data;
        }
    }

    public SerializedProtectedString() : this(string.Empty)
    {
    }

    private static string TryUnprotect(string unknownString)
    {
        try
        {
            return Unprotect(unknownString);
        }
        catch (Exception)
        {
            return unknownString;
        }
    }

    private static string Unprotect(string protectedStr)
    {
        return Encoding.Unicode.GetString(ProtectedData.Unprotect(Convert.FromBase64String(protectedStr),
            null!, DataProtectionScope.CurrentUser));
    }

    public override string ToString() => UnprotectedValue;

    public static implicit operator SerializedProtectedString(string unkStr) => new SerializedProtectedString(TryUnprotect(unkStr));

    public static implicit operator string?(SerializedProtectedString? protStr) => protStr?.ToString();
}
