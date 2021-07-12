using System;
using System.Web.Services;
using System.Diagnostics;
using System.Web.Services.Protocols;
using System.Net;

/// <summary>
/// Represents a RemittancePartnerWebService instance. Acts as a wrapper utilizing Partner's API methods.
/// </summary>
[WebService(Namespace = "http://pjlhuillier.org/remittance/partner/nybp")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class RemittancePartnerWebService : System.Web.Services.WebService
{
    #region Contructors

    public RemittancePartnerWebService()
    {
        //Uncomment the following line if using designed components 
        //InitializeComponent();
    }

    #endregion

    #region Fields

    #endregion

    #region WebMethods

    [WebMethod(Description = "Method to expose proxy classes. Does not do anything but allow the clients to use the class.")]
    public void ExposeProxyClasses(CebuanaCustomerInformation cebuanaCustomerInformation, CebuanaCustomerID cebuanaCustomerID)
    { }

    [WebMethod(Description = "Partner reference number lookup method.")]
    public LookupTransactionResult RemittancePartnerLookup(LookupTransactionRequest lookupTransactionRequest)
    {
        switch (lookupTransactionRequest.CebuanaBranchInformation.ClientApplicationVersion)
        {
            default:
                return RemittancePartnerLookup_01(lookupTransactionRequest);
        }
    }

    [WebMethod(Description = "Partner reference number payout method.")]
    public PayoutTransactionResult RemittancePartnerPayout(PayoutTransactionRequest payoutTransactionRequest)
    {
        switch (payoutTransactionRequest.CebuanaBranchInformation.ClientApplicationVersion)
        {
            default:
                return RemittancePartnerPayout_01(payoutTransactionRequest);
        }
    }

    #endregion

    #region Methods

    private LookupTransactionResult RemittancePartnerLookup_01(LookupTransactionRequest lookupTransactionRequest)
    {
        try
        {
            LookupTransactionResult lookupTransactionResult = lookupTransactionRequest.LookupTransaction();
            return lookupTransactionResult;
        }
        catch (Exception error)
        {
            string concatenatedErrorMessage = error.InnerException != null ? string.Format("{0}\n{1}", error.Message, error.InnerException.Message) : error.Message;

            Utils.WriteToEventLog(string.Format("RemittancePartnerLookup_01\n{0}", concatenatedErrorMessage), EventLogEntryType.Error);

            LookupTransactionResult errorResponse = new LookupTransactionResult();
            errorResponse.ResultCode = LookupTransactionResultCode.ServerError;

            if (error is RemittanceException)
            {
                errorResponse.MessageToClient = error.Message;
            }
            else if (error is SoapException)
            {
                errorResponse.MessageToClient = "An error in the partner's web service has occurred while looking up the transaction.";
            }
            else if (error is WebException)
            {
                errorResponse.MessageToClient = "An error in the connection to the partner's web service has occurred while looking up the transaction. Please try again later.";
            }
            else
            {
                errorResponse.MessageToClient = "An error has occurred while retrieving the transaction details from the partner. Please contact ICT Support Desk.";
            }
            
            return errorResponse;
        }
    }

    private PayoutTransactionResult RemittancePartnerPayout_01(PayoutTransactionRequest payoutTransactionRequest)
    {
        try
        {
            PayoutTransactionResult payoutTransactionResult = payoutTransactionRequest.PayoutTransaction();
            return payoutTransactionResult;
        }
        catch (Exception error)
        {
            string concatenatedErrorMessage = error.InnerException != null ? string.Format("{0}\n{1}", error.Message, error.InnerException.Message) : error.Message;

            Utils.WriteToEventLog(
                string.Format("RemittancePartnerPayout_01\n{0}", concatenatedErrorMessage),
                EventLogEntryType.Error);

            //Utils.WriteToEventLog(string.Format("{0}\n{1}", System.Reflection.MethodInfo.GetCurrentMethod().Name, concatenatedErrorMessage), EventLogEntryType.Error);
            
            PayoutTransactionResult errorResponse = new PayoutTransactionResult();
            errorResponse.ResultCode = PayoutTransactionResultCode.ServerError;

            if (error is RemittanceException)
            {
                errorResponse.MessageToClient = error.Message;
            }
            else if (error is SoapException)
            {
                errorResponse.MessageToClient = "An error in the partner's web service has occurred while paying out the transaction.";
            }
            else if (error is WebException)
            {
                errorResponse.MessageToClient = "An error in the connection to the partner's web service has occurred while paying out the transaction. Please try again later.";
            }
            else
            {
                errorResponse.MessageToClient = "An error has occurred while paying out the transaction. Please contact ICT Support Desk.";
            }
            
            return errorResponse;
        }
    }

    #endregion
}

