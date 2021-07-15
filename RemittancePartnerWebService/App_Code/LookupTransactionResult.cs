using System;
//using ChinaBankWebReference;
using Mockcarnelian;
/// <summary>
/// Represents an instance of a LookupTransactionResult class
/// </summary>
public class LookupTransactionResult
{
    #region Constructors

    public LookupTransactionResult()
    { }

    public LookupTransactionResult(requestTransactionResult result)
    {
        remittanceResponse resultItem = result.remittanceResponseList[0];

        switch (result.isError)
        {
            case 0:
                {
                    string multiCurrencyPayoutCode = RemittancePartnerConfiguration.GetMultiCurrencyPayoutCode(resultItem.transactionCurrency);
                    if (resultItem.transactionCurrency.ToUpper() != "PHP"
                        && string.IsNullOrEmpty(multiCurrencyPayoutCode) == true
                        )
                    {
                        _resultCode = LookupTransactionResultCode.Unsuccessful;
                        _messageToClient = "Transaction is a non-PHP transaction. Processing of this transaction is not yet allowed for this partner.";
                    }
                    else
                    {
                        _resultCode = LookupTransactionResultCode.Successful;
                        _messageToClient = "Lookup transaction successful.";

                        _transactionNumber = resultItem.transactionRefNo;
                        _transactionStatus = TransactionStatus.ForPayout;
                        _transactionDate = DateTime.Now;

                        _payoutAmount = Convert.ToDecimal(resultItem.transactionAmount);
                        _payoutCountry = "PH";
                        _payoutCurrency = resultItem.transactionCurrency;

                        _senderLastName = resultItem.remitterName == null ? string.Empty : resultItem.remitterName.Trim();
                        _senderFirstName = string.Empty;

                        if (_senderLastName.EndsWith(","))
                            _senderLastName = _senderLastName.Substring(0, _senderLastName.Length - 1);

                        _beneficiaryLastName = resultItem.beneficiaryName == null ? string.Empty : resultItem.beneficiaryName.Trim();
                        _beneficiaryFirstName = string.Empty;

                        if (_beneficiaryLastName.EndsWith(","))
                            _beneficiaryLastName = _beneficiaryLastName.Substring(0, _beneficiaryLastName.Length - 1);

                        _multiCurrencyPayoutCode = multiCurrencyPayoutCode;

                        _message = resultItem.messageToRecipient;
                    }
                }
                break;
            default:
                {
                    string errorLogMessage = string.Format("Code: {1}{0}Message: {2}", Environment.NewLine, result.isError, result.errorMessage);
                    Utils.WriteToEventLog(errorLogMessage, System.Diagnostics.EventLogEntryType.Error);
                    _resultCode = LookupTransactionResultCode.Unsuccessful;
                    _messageToClient = "An error has occurred while looking up this transaction. Please contact ICT Support Desk.";
                }
                break;
        }

        _messageToClient = string.Format("[{0}:{1}] {2}", RemittancePartnerConfiguration.ApplicationCode, result.isError, _messageToClient);
    }

    #endregion

    #region Fields

    private LookupTransactionResultCode _resultCode = LookupTransactionResultCode.UnrecognizedResponse;
    private string _messageToClient;

    private string _transactionNumber;
    private TransactionStatus _transactionStatus = TransactionStatus.UnrecognizedStatus;
    private DateTime _transactionDate;
    private decimal _payoutAmount;
    private string _payoutCountry;
    private string _payoutCurrency;

    private string _senderLastName;
    private string _senderFirstName;

    private string _beneficiaryLastName;
    private string _beneficiaryFirstName;

    private string _multiCurrencyPayoutCode;

    private string _sessionID;
    private string _message;
    

    #endregion

    #region Properties

    public LookupTransactionResultCode ResultCode
    {
        get { return _resultCode; }
        set { _resultCode = value; }
    }

    public string MessageToClient
    {
        get { return _messageToClient; }
        set { _messageToClient = value; }
    }



    public string TransactionNumber
    {
        get { return _transactionNumber; }
        set { _transactionNumber = value; }
    }

    public TransactionStatus TransactionStatus
    {
        get { return _transactionStatus; }
        set { _transactionStatus = value; }
    }

    public DateTime TransactionDate
    {
        get { return _transactionDate; }
        set { _transactionDate = value; }
    }

    public decimal PayoutAmount
    {
        get { return _payoutAmount; }
        set { _payoutAmount = value; }
    }

    public string PayoutCurrency
    {
        get { return _payoutCurrency; }
        set { _payoutCurrency = value; }
    }

    public string PayoutCountry
    {
        get { return _payoutCountry; }
        set { _payoutCountry = value; }
    }


    public string SenderLastName
    {
        get { return _senderLastName; }
        set { _senderLastName = value; }
    }

    public string SenderFirstName
    {
        get { return _senderFirstName; }
        set { _senderFirstName = value; }
    }


    public string BeneficiaryLastName
    {
        get { return _beneficiaryLastName; }
        set { _beneficiaryLastName = value; }
    }

    public string BeneficiaryFirstName
    {
        get { return _beneficiaryFirstName; }
        set { _beneficiaryFirstName = value; }
    }

    public string MultiCurrencyPayoutCode
    {
        get { return _multiCurrencyPayoutCode; }
        set { _multiCurrencyPayoutCode = value; }
    }

    public string SessionID
    {
        get { return _sessionID; }
        set { _sessionID = value; }
    }

    #endregion
}
