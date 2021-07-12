using System;
using System.Net;
using System.Web.Services.Protocols;
using ChinaBankWebReference;

/// <summary>
/// Represents an instance of a LookupTransactionRequest class
/// </summary>
public class LookupTransactionRequest
{
    #region Constructors

    public LookupTransactionRequest()
    {
    }

    #endregion

    #region Fields

    private CebuanaBranchInformation _cebuanaBranchInformation;
    private string _transactionNumber;

    #endregion

    #region Properties

    public string TransactionNumber
    {
        get { return _transactionNumber; }
        set { _transactionNumber = value; }
    }

    public CebuanaBranchInformation CebuanaBranchInformation
    {
        get { return _cebuanaBranchInformation; }
        set { _cebuanaBranchInformation = value; }
    }

    #endregion

    #region Methods

    public LookupTransactionResult LookupTransaction()
    {
        requestHeader header = new requestHeader();
        header.username = RemittancePartnerConfiguration.Username;
        header.password = RemittancePartnerConfiguration.Password;

        CarnelianWebServiceImplService service = new CarnelianWebServiceImplService();
        //service.Proxy = RemittancePartnerConfiguration.WebProxy;

        requestTransactionResult result = service.requestTransaction(header, RemittancePartnerConfiguration.OutletCode, _transactionNumber);

        LookupTransactionResult lookupTransactionResult;

        if (result.remittanceResponseList == null || result.remittanceResponseList.Length == 0)
        {
            lookupTransactionResult = new LookupTransactionResult();
            lookupTransactionResult.ResultCode = LookupTransactionResultCode.Unsuccessful;
            lookupTransactionResult.MessageToClient =
                //"Transaction is either invalid or already paid out. Make sure transaction number is correct.";
                string.Format("[{0}] NOT FOUND. Please contact CLSC if control number is correct.", _transactionNumber);
            return lookupTransactionResult;
        }

        if (result.remittanceResponseList.Length > 1)
        {
            lookupTransactionResult = new LookupTransactionResult();
            lookupTransactionResult.ResultCode = LookupTransactionResultCode.PartnerError;
            lookupTransactionResult.MessageToClient = "Partner returned too many results for this transaction number. Please contact ICT Support Desk.";
            return lookupTransactionResult;
        }

        lookupTransactionResult = new LookupTransactionResult(result);

        return lookupTransactionResult;
    }

    #endregion
}
