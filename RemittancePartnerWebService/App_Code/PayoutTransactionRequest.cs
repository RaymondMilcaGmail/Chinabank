﻿//#define TEST

using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Diagnostics;
using ChinaBankWebReference;

/// <summary>
/// Represents an instance of a PayoutTransactionRequest class
/// </summary>
public class PayoutTransactionRequest
{
    #region Enums

    private enum TransactionStatuses
    {
        Outstanding = 19,
        PaidOut = 20,
        PayoutPending = 36
    }

    #endregion

    #region Constructors

    public PayoutTransactionRequest()
    {
    }

    #endregion

    #region Fields

    private CebuanaBranchInformation _cebuanaBranchInformation;

    private string _transactionNumber;
    private decimal _payoutAmount;
    private string _sendingCurrency;
    private string _payoutCurrency;
    private decimal _currencyConversionRate;
    private string _payoutCountry;
    private string _senderLastName;
    private string _senderFirstName;

    private string _receiverCustomerNumber;
    private string _receiverLastName;
    private string _receiverFirstName;
    private string _receiverIDCode;
    private string _receiverIDType;
    private string _receiverIDDetails;
    private string _receiverCity;
    private string _receiverCountry;

    //private string _idType;
    private string _sessionID;

    #endregion

    #region Properties

    public CebuanaBranchInformation CebuanaBranchInformation
    {
        get { return _cebuanaBranchInformation; }
        set { _cebuanaBranchInformation = value; }
    }

    public string TransactionNumber
    {
        get { return _transactionNumber; }
        set { _transactionNumber = value; }
    }

    public decimal PayoutAmount
    {
        get { return _payoutAmount; }
        set { _payoutAmount = value; }
    }

    public string SendingCurrency
    {
        get { return _sendingCurrency; }
        set { _sendingCurrency = value; }
    }

    public string PayoutCurrency
    {
        get { return _payoutCurrency; }
        set { _payoutCurrency = value; }
    }

    public decimal CurrencyConversionRate
    {
        get { return _currencyConversionRate; }
        set { _currencyConversionRate = value; }
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

    public string ReceiverCustomerNumber
    {
        get { return _receiverCustomerNumber; }
        set { _receiverCustomerNumber = value; }
    }

    public string ReceiverLastName
    {
        get { return _receiverLastName; }
        set { _receiverLastName = value; }
    }    

    public string ReceiverFirstName
    {
        get { return _receiverFirstName; }
        set { _receiverFirstName = value; }
    }

    public string ReceiverIDCode
    {
        get { return _receiverIDCode; }
        set { _receiverIDCode = value; }
    }

    public string ReceiverIDType
    {
        get { return _receiverIDType; }
        set { _receiverIDType = value; }
    }

    public string ReceiverIDNumber
    {
        get { return _receiverIDDetails; }
        set { _receiverIDDetails = value; }
    }

    public string ReceiverCity
    {
        get { return _receiverCity; }
        set { _receiverCity = value; }
    }

    public string ReceiverCountry
    {
        get { return _receiverCountry; }
        set { _receiverCountry = value; }
    }

    public string SessionID
    {
        get { return _sessionID; }
        set { _sessionID = value; }
    }

    #endregion

    #region Methods

    private string GetTransactionStatusDescription(TransactionStatuses transactionStatus)
    {
        switch (transactionStatus)
        {
            case TransactionStatuses.PaidOut:
                return "TIE UP PAID";
            case TransactionStatuses.PayoutPending:
                return "TIE UP PAYOUTPENDING";
            case TransactionStatuses.Outstanding:
                return "TIE UP OUTSTANDING";
            default:
                return "UNKNOWN STATUS";
        }
    }

    public PayoutTransactionResult PayoutTransaction()
    {
        //_idType = RemittancePartnerConfiguration.GetIDType(_receiverIDCode);
        //if (string.IsNullOrEmpty(_idType))
        //{
        //    PayoutTransactionResult payoutValidationResult = new PayoutTransactionResult();
        //    payoutValidationResult.ResultCode = PayoutTransactionResultCode.Unsuccessful;
        //    payoutValidationResult.MessageToClient = "ID type submitted is not a recognized ID for this transaction. Please use a different ID type.";
        //    return payoutValidationResult;
        //}

        long transactionID = InsertTransactionToDatabase(TransactionStatuses.PayoutPending);

        requestHeader header = new requestHeader();
        header.username = RemittancePartnerConfiguration.Username;
        header.password = RemittancePartnerConfiguration.Password;

        CarnelianWebServiceImplService service = new CarnelianWebServiceImplService();

        transactionUpdateResult result = service.updateTransactionStatus(
            header,
            2, // 2 = Paid out
            true, DateTime.Now.ToString("MMddyyyyhhmmss"),
            string.Empty,
            _transactionNumber,
            RemittancePartnerConfiguration.OutletCode);

        PayoutTransactionResult payoutTransactionResult = new PayoutTransactionResult(result, _transactionNumber);

        if (payoutTransactionResult.ResultCode == PayoutTransactionResultCode.Successful)
        {
            try
            {
                UpdateTransaction(transactionID, TransactionStatuses.PaidOut);
            }
            catch (Exception error)
            {
                Utils.WriteToEventLog(string.Format("UpdateTransaction\n{0}", error.Message), EventLogEntryType.Error);
            }
        }

        return payoutTransactionResult;
    }

    private long InsertTransactionToDatabase(TransactionStatuses transactionStatus)
    {
        return InsertTransactionToDatabase(transactionStatus, string.Empty, string.Empty);
    }

    private long InsertTransactionToDatabase(TransactionStatuses transactionStatus, string partnerInternalReferenceNumber, string partnerInternalReferenceNumber2)
    {
        long transactionID = long.MinValue;

        using (SqlConnection sqlConnection = new SqlConnection())
        {
            sqlConnection.ConnectionString = RemittancePartnerConfiguration.ConnectionStringRemittanceDatabase;
            SqlCommand sqlCommand = new SqlCommand(RemittancePartnerConfiguration.StoredProcedureInsertPayoutTransaction, sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.AddWithValue("@PartnerCode", RemittancePartnerConfiguration.ApplicationCode);
            sqlCommand.Parameters.AddWithValue("@TransactionID", transactionID);
            sqlCommand.Parameters["@TransactionID"].Direction = ParameterDirection.Output;
            sqlCommand.Parameters.AddWithValue("@ControlNumber", _transactionNumber);
            sqlCommand.Parameters.AddWithValue("@TransactionStatusID", (int)transactionStatus);
            sqlCommand.Parameters.AddWithValue("@TransactionStatusDescription", GetTransactionStatusDescription(transactionStatus));
            sqlCommand.Parameters.AddWithValue("@PayoutAmount", _payoutAmount);
            sqlCommand.Parameters.AddWithValue("@SendingCurrency", _sendingCurrency);
            sqlCommand.Parameters.AddWithValue("@PayoutCurrency", _payoutCurrency);
            sqlCommand.Parameters.Add("@CurrencyConversionRate", SqlDbType.Decimal);
            sqlCommand.Parameters["@CurrencyConversionRate"].Precision = 18;
            sqlCommand.Parameters["@CurrencyConversionRate"].Scale = 2;
            sqlCommand.Parameters["@CurrencyConversionRate"].Value = _currencyConversionRate;
            sqlCommand.Parameters.AddWithValue("@PayoutCountry", _payoutCountry);

            sqlCommand.Parameters.AddWithValue("@SenderLastName", _senderLastName);
            sqlCommand.Parameters.AddWithValue("@SenderFirstName", _senderFirstName);

            sqlCommand.Parameters.AddWithValue("@BeneficiaryCustomerNumber", Convert.ToInt64(_receiverCustomerNumber));
            sqlCommand.Parameters.AddWithValue("@BeneficiaryLastName", _receiverLastName);
            sqlCommand.Parameters.AddWithValue("@BeneficiaryFirstName", _receiverFirstName);
            sqlCommand.Parameters.AddWithValue("@BeneficiaryIDType", _receiverIDType);
            sqlCommand.Parameters.AddWithValue("@BeneficiaryIDDetails", _receiverIDDetails);

            sqlCommand.Parameters.AddWithValue("@ReceiverBranchUserID", _cebuanaBranchInformation.BranchUserID);
            sqlCommand.Parameters.AddWithValue("@PayoutBranchCode", _cebuanaBranchInformation.BranchCode);

            sqlCommand.Parameters.AddWithValue("@AuditTracker",
                RemittanceAuditTrail.GetAuditTrailString(
                    _cebuanaBranchInformation.BranchCode,
                    _cebuanaBranchInformation.BranchUserID,
                    "1",
                    "1",
                    HttpContext.Current.Request.UserHostAddress,
                    _cebuanaBranchInformation.ClientMacAddress)
                );

            sqlCommand.Parameters.AddWithValue("@PartnerInternalReferenceNumber", partnerInternalReferenceNumber);
            sqlCommand.Parameters.AddWithValue("@PartnerInternalReferenceNumber2", partnerInternalReferenceNumber2);

            sqlConnection.Open();
            sqlCommand.ExecuteNonQuery();

            transactionID = Convert.ToInt64(sqlCommand.Parameters["@TransactionID"].Value);
            return transactionID;
        }
    }

    private void UpdateTransaction(long transactionID, TransactionStatuses transactionStatus)
    {
        UpdateTransaction(transactionID, transactionStatus, string.Empty, string.Empty);
    }

    private void UpdateTransaction(long transactionID, TransactionStatuses transactionStatus, string partnerInternalReferenceNumber, string partnerInternalReferenceNumber2)
    {
        using (SqlConnection sqlConnection = new SqlConnection())
        {
            sqlConnection.ConnectionString = RemittancePartnerConfiguration.ConnectionStringRemittanceDatabase;
            SqlCommand sqlCommand = new SqlCommand(RemittancePartnerConfiguration.StoredProcedureUpdatePayoutTransaction, sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.AddWithValue("@PartnerCode", RemittancePartnerConfiguration.ApplicationCode);
            sqlCommand.Parameters.AddWithValue("@TransactionID", transactionID);
            sqlCommand.Parameters.AddWithValue("@ControlNumber", _transactionNumber);
            sqlCommand.Parameters.AddWithValue("@TransactionStatusID", (int)transactionStatus);
            sqlCommand.Parameters.AddWithValue("@TransactionStatusDescription", GetTransactionStatusDescription(transactionStatus));
            sqlCommand.Parameters.AddWithValue("@PayoutAmount", _payoutAmount);
            sqlCommand.Parameters.AddWithValue("@SendingCurrency", _sendingCurrency);
            sqlCommand.Parameters.AddWithValue("@PayoutCurrency", _payoutCurrency);
            sqlCommand.Parameters.Add("@CurrencyConversionRate", SqlDbType.Decimal);
            sqlCommand.Parameters["@CurrencyConversionRate"].Precision = 18;
            sqlCommand.Parameters["@CurrencyConversionRate"].Scale = 2;
            sqlCommand.Parameters["@CurrencyConversionRate"].Value = _currencyConversionRate;
            sqlCommand.Parameters.AddWithValue("@PayoutCountry", _payoutCountry);

            sqlCommand.Parameters.AddWithValue("@SenderLastName", _senderLastName);
            sqlCommand.Parameters.AddWithValue("@SenderFirstName", _senderFirstName);

            sqlCommand.Parameters.AddWithValue("@BeneficiaryCustomerNumber", Convert.ToInt64(_receiverCustomerNumber));
            sqlCommand.Parameters.AddWithValue("@BeneficiaryLastName", _receiverLastName);
            sqlCommand.Parameters.AddWithValue("@BeneficiaryFirstName", _receiverFirstName);
            sqlCommand.Parameters.AddWithValue("@BeneficiaryIDType", _receiverIDType);
            sqlCommand.Parameters.AddWithValue("@BeneficiaryIDDetails", _receiverIDDetails);

            sqlCommand.Parameters.AddWithValue("@ReceiverBranchUserID", _cebuanaBranchInformation.BranchUserID);
            sqlCommand.Parameters.AddWithValue("@PayoutBranchCode", _cebuanaBranchInformation.BranchCode);

            sqlCommand.Parameters.AddWithValue("@AuditTracker",
                RemittanceAuditTrail.GetAuditTrailString(
                    _cebuanaBranchInformation.BranchCode,
                    _cebuanaBranchInformation.BranchUserID,
                    "1",
                    "1",
                    HttpContext.Current.Request.UserHostAddress,
                    _cebuanaBranchInformation.ClientMacAddress)
                );

            sqlCommand.Parameters.AddWithValue("@PartnerInternalReferenceNumber", partnerInternalReferenceNumber);
            sqlCommand.Parameters.AddWithValue("@PartnerInternalReferenceNumber2", partnerInternalReferenceNumber2);

            sqlConnection.Open();
            sqlCommand.ExecuteNonQuery();
        }
    }

    #endregion
}