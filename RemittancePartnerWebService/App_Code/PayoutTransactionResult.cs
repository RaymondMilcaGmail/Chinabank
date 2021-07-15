using System;
//using ChinaBankWebReference;
using Mockcarnelian;
/// <summary>
/// Represents an instance of a PayoutTransactionResult class
/// </summary>
/// 
public class PayoutTransactionResult
{
    #region Constructors

    public PayoutTransactionResult()
    { }

    public PayoutTransactionResult(transactionUpdateResult result, string transactionNumber)
    {
        switch (result.isError)
        {
            case 0:
                {
                    _resultCode = PayoutTransactionResultCode.Successful;
                    _messageToClient = "Payout transaction successful.";
                    _transactionNumber = transactionNumber;
                    _payoutDate = DateTime.Now;
                }
                break;
            default:
                {
                    string errorLogMessage = string.Format("Code: {1}{0}Message: {2}", Environment.NewLine, result.isError, result.errorMessage);
                    Utils.WriteToEventLog(errorLogMessage, System.Diagnostics.EventLogEntryType.Error);
                    _resultCode = PayoutTransactionResultCode.Unsuccessful;
                    _messageToClient = "An error has occurred while paying out this transaction. Please contact ICT Support Desk.";
                }
                break;
        }

        _messageToClient = string.Format("[{0}:{1}] {2}", RemittancePartnerConfiguration.ApplicationCode, result.isError, _messageToClient);
    }

    #endregion

    #region Fields

    private PayoutTransactionResultCode _resultCode = PayoutTransactionResultCode.UnrecognizedResponse;
    private string _messageToClient;
    private string _transactionNumber;
    private DateTime _payoutDate;

    #endregion

    #region Properties

    public PayoutTransactionResultCode ResultCode
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

    public DateTime PayoutDate
    {
        get { return _payoutDate; }
        set { _payoutDate = value; }
    }

    #endregion
}
