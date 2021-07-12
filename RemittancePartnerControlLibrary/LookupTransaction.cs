using System;
using System.Net;
using ChinaBankControlLibrary.RemittancePartnerWebReference;
using System.Web.Services.Protocols;

namespace ChinaBankControlLibrary
{
    class LookupTransaction
    {
        #region Contructors

        internal LookupTransaction(LookupTransactionRequest lookupTransactionRequest)
        {
            _lookupTransactionRequest = lookupTransactionRequest;
        }

        private LookupTransaction()
            : this(null)
        { }

        #endregion

        #region Fields

        private string _webServiceURL;

        private LookupTransactionRequest _lookupTransactionRequest;

        private LookupTransactionResult _lookupTransactionResult;

        #endregion

        #region Properties

        internal string WebServiceURL
        {
            get { return _webServiceURL; }
            set { _webServiceURL = value; }
        }

        internal LookupTransactionResult LookupTransactionResult
        {
            get { return _lookupTransactionResult; }
            //set { _lookupTransactionResult = value; }
        }

        #endregion

        #region Methods

        internal void Lookup()
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

                _lookupTransactionResult = remittancePartnerWebService.RemittancePartnerLookup(_lookupTransactionRequest);
            }
            catch (Exception error)
            {
                if (error is RemittanceException)
                {
                    throw error;
                }
                else if (error is WebException)
                {
                    throw new RemittanceException("Reference number lookup failed. Branch may be OFFLINE.", error);
                }
                else if (error is SoapException)
                {
                    throw new RemittanceException("An error in the web service has occured while looking up this transaction. ", error);
                }
                else
                {
                    throw new RemittanceException("An error has occured while looking up this transaction. ", error);
                }
            }
        }

        #endregion

        #region EventHandlers

        //internal event EventHandler LookupTransactionResultChanged;

        //private void LookupResultChanged(LookupTransactionResult lookupTransactionResult)
        //{
        //    if (LookupTransactionResultChanged != null)
        //    {
        //        LookupTransactionResultChanged(this, new LookupEventArgs(lookupTransactionResult));
        //    }
        //}

        #endregion
    }
}
