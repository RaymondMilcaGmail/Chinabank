using System;
using System.Configuration;
using System.Net;
using System.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

/// <summary>
/// Represents an instance of a RemittancePartnerConfiguration class
/// </summary>
public class RemittancePartnerConfiguration
{
    public static string ApplicationName
    {
        get { return ConfigurationManager.AppSettings["ApplicationName"].ToString(); }
    }

    public static string ApplicationCode
    {
        get { return ConfigurationManager.AppSettings["ApplicationCode"].ToString(); }
    }

    //public static WebProxy WebProxy
    //{
    //    get
    //    {
    //        try
    //        {
    //            string proxyHost = ConfigurationManager.AppSettings["ProxyHost"];
    //            int proxyPort = Convert.ToInt32(ConfigurationManager.AppSettings["ProxyPort"]);
    //            string proxyUsername = ConfigurationManager.AppSettings["ProxyUsername"];
    //            string proxyPassword = ConfigurationManager.AppSettings["ProxyPassword"];
    //            string proxyDomain = ConfigurationManager.AppSettings["ProxyDomain"];

    //            if (proxyHost == null || proxyPort == 0)
    //            {
    //                return null;
    //            }

    //            WebProxy webProxy = new WebProxy(proxyHost, proxyPort);

    //            if (proxyUsername == null || proxyPassword == null)
    //            {
    //                return webProxy;
    //            }

    //            if (proxyDomain == null)
    //            {
    //                webProxy.Credentials = new NetworkCredential(proxyUsername, proxyPassword);
    //            }
    //            else
    //            {
    //                webProxy.Credentials = new NetworkCredential(proxyUsername, proxyPassword, proxyDomain);
    //            }

    //            return webProxy;
    //        }
    //        catch
    //        {
    //            return null;
    //        }
    //    }
    //}

    public static string StoredProcedureInsertPayoutTransaction
    {
        get { return ConfigurationManager.AppSettings["StoredProcedureInsertPayoutTransaction"].ToString(); }
    }

    public static string StoredProcedureUpdatePayoutTransaction
    {
        get { return ConfigurationManager.AppSettings["StoredProcedureUpdatePayoutTransaction"].ToString(); }
    }

    public static string ConnectionStringRemittanceDatabase
    {
        get { return ConfigurationManager.ConnectionStrings["RemittanceDBConnection"].ToString(); }
    }

    //public static bool AreNonPhpTransactionsAllowed
    //{
    //    get { return Convert.ToBoolean(ConfigurationManager.AppSettings["AreNonPhpTransactionsAllowed"]); }
    //}

    public static string MultiCurrencyPayoutCode
    {
        get { return ConfigurationManager.AppSettings["MultiCurrencyPayoutCode"]; }
    }

    public static string GetMultiCurrencyPayoutCode(string currencyCode)
    {
        string appSettingsKey = string.Format(@"PayoutCode{0}", currencyCode.ToUpper());
        return ConfigurationManager.AppSettings[appSettingsKey];
    }

    public static string GetIDType(string idType)
    {
        string partnerIDType;
        idType = idType.Trim().ToUpper();

        if (ConfigurationManager.AppSettings[idType] != null)
        {
            partnerIDType = ConfigurationManager.AppSettings[idType];
        }
        else if (ConfigurationManager.AppSettings["DEFAULT_ID"] != null)
        {
            partnerIDType = ConfigurationManager.AppSettings["DEFAULT_ID"];
        }
        else
        {
            partnerIDType = string.Empty;
        }

        return partnerIDType;
    }

    public static string GetIDMap(string idType)
    {
        string partnerIDDesc;
        idType = idType.Trim().ToUpper();

        if (ConfigurationManager.AppSettings["ID_" + idType] != null)
        {
            partnerIDDesc = ConfigurationManager.AppSettings[idType];
        }
        else
        {
            partnerIDDesc = string.Empty;
        }

        return partnerIDDesc;
    }
    public static string Username
    {
        get { return ConfigurationManager.AppSettings["Username"] ?? string.Empty; }
    }

    public static string Password
    {
        get { return ConfigurationManager.AppSettings["Password"] ?? string.Empty; }
    }

    public static string OutletCode
    {
        get { return ConfigurationManager.AppSettings["OutletCode"] ?? string.Empty; }
    }

    public static bool ValidateRemoteCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
    {
        if (Convert.ToBoolean(ConfigurationManager.AppSettings["IgnoreSslErrors"]))
        {
            return true;
        }
        else
        {
            return policyErrors == SslPolicyErrors.None;
        }
    }
}
