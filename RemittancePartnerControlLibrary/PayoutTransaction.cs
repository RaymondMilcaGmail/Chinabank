using System;
using System.Net;
using ChinaBankControlLibrary.RemittancePartnerWebReference;
using System.Web.Services.Protocols;

namespace ChinaBankControlLibrary
{
    class PayoutTransaction
    {
        #region Constructors

        internal PayoutTransaction(PayoutTransactionRequest payoutTransactionRequest)
        {
            _payoutTransactionRequest = payoutTransactionRequest;
        }

        #endregion

        #region Fields

        private string _webServiceURL;

        private PayoutTransactionRequest _payoutTransactionRequest;

        private PayoutTransactionResult _payoutTransactionResult;

        #endregion

        #region Properties

        internal string WebServiceURL
        {
            get { return _webServiceURL; }
            set { _webServiceURL = value; }
        }

        internal PayoutTransactionRequest PayoutTransactionRequest
        {
            get { return _payoutTransactionRequest; }
            //set { _payoutTransactionRequest = value; }
        }

        internal PayoutTransactionResult PayoutTransactionResult
        {
            get { return _payoutTransactionResult; }
            //set { _payoutTransactionResult = value; }
        }

        #endregion

        #region Methods

        internal void Payout()
        {
            try
            {
                if (!Utils.IsURIOnline(_webServiceURL))
                {
                    throw new RemittanceException("Connectivity test failed. The branch may be OFFLINE.");
                }

                RemittancePartnerWebService remittancePartnerWebService = new RemittancePartnerWebService();
                remittancePartnerWebService.Url = _webServiceURL;
                remittancePartnerWebService.Timeout = RemittancePartnerConfiguration.WebServiceCallTimeout;

                //_payoutTransactionResult = new PayoutTransactionResult();
                //_payoutTransactionResult.MessageToClient = "Success";
                //_payoutTransactionResult.ResultCode = PayoutTransactionResultCode.Successful;
                //_payoutTransactionResult.TransactionNumber = "123";
                //_payoutTransactionResult.PayoutDate = DateTime.Now;

                _payoutTransactionResult = remittancePartnerWebService.RemittancePartnerPayout(_payoutTransactionRequest);
            }
            catch (Exception error)
            {
                if (error is RemittanceException)
                {
                    throw error;
                }
                else if (error is WebException)
                {
                    throw new RemittanceException("Reference number payout failed. Branch may be OFFLINE.", error);
                }
                else if (error is SoapException)
                {
                    throw new RemittanceException("An error in the web service has occured while paying out this transaction. ", error);
                }
                else
                {
                    throw new RemittanceException("An error has occured while paying out this transaction. ", error);
                }
            }
        }

        #endregion
    }
}
