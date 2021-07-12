using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CRMSModuleII;
using ChinaBankControlLibrary.RemittancePartnerWebReference;

namespace ChinaBankControlLibrary
{
    public partial class PayoutForm : Form
    {
        #region Contructors

        internal PayoutForm(RemittancePartnerControlMain remittancePartnerControlMain)
        {
            InitializeComponent();
            GroupControls();
            InitializePayoutForm();
            _parentRemittancePartnerControlMain = remittancePartnerControlMain;
        }

        #endregion

        #region Fields

        private bool _isCalledFromStaticMethod = false;

        private RemittancePartnerControlMain _parentRemittancePartnerControlMain;
        private Control[] _inputReferenceNumberControls;
        private Control[] _inputCustomerControls;
        private Control[] _inputCustomerValidationControls;
        private Control[] _inputProcessingControls;
        private Control[] _inputPostProcessingControls;

        private Control[] _outputReferenceNumberControls;
        private Control[] _outputCustomerControls;
        private Control[] _outputCustomerValidationControls;
        private Control[] _outputProcessingControls;
        private Control[] _outputPostProcessingControls;

        private CebuanaCustomerInformation _cebuanaCustomerInformation;
        private CebuanaCustomerID[] _cebuanaCustomerIDs;
        private LookupTransaction _remittancePartnerLookupTransaction;
        private PayoutTransaction _remittancePartnerPayoutTransaction;

        #endregion

        #region Properties

        internal string ReferenceNumber
        {
            get { return txtReferenceNumber.Text; }
            set { txtReferenceNumber.Text = value; }
        }

        internal bool IsCalledFromStaticMethod
        {
            get { return _isCalledFromStaticMethod; }
            set { _isCalledFromStaticMethod = value; }
        }

        #endregion

        #region Methods

        private void BuildCustomerIDTypesList()
        {
            try
            {
                _cebuanaCustomerIDs = RemittancePartnerConfiguration.GetCebuanaCustomerIDs();
                cboCustomerIDSubmitted.DataSource = _cebuanaCustomerIDs;
                cboCustomerIDSubmitted.ValueMember = "IDCode";
                cboCustomerIDSubmitted.DisplayMember = "IDDescription";
            }
            catch (Exception error)
            {
                throw error;
            }
        }

        private void InitializePayoutForm()
        {
            Utils.ClearControls(
                _inputReferenceNumberControls,
                _inputCustomerControls,
                _inputCustomerValidationControls,
                _inputProcessingControls,
                _inputPostProcessingControls,

                _outputReferenceNumberControls,
                _outputCustomerControls,
                _outputCustomerValidationControls,
                _outputProcessingControls,
                _outputPostProcessingControls);

            Utils.ToggleControlEnabledState(
                true,
                _inputReferenceNumberControls);

            Utils.ToggleControlEnabledState(
                false,
                _inputCustomerControls,
                _inputCustomerValidationControls,
                _inputProcessingControls,
                _inputPostProcessingControls);

            lblPayoutCurrency.BackColor = Color.Azure;
            lblProcessingStatus.BackColor = Color.DarkOrange;
            lblProcessingStatus.Text = "Input reference number.";

            _remittancePartnerLookupTransaction = null;
            _remittancePartnerPayoutTransaction = null;
        }

        private void GroupControls()
        {
            #region Input Controls

            Control[] inputReferenceNumberControls = { txtReferenceNumber, btnLoadReferenceNumber };
            _inputReferenceNumberControls = inputReferenceNumberControls;

            Control[] inputCustomerControls = { };
            _inputCustomerControls = inputCustomerControls;

            Control[] inputCustomerValidationControls = { cboCustomerIDSubmitted, txtCustomerIDSubmittedNumber };
            _inputCustomerValidationControls = inputCustomerValidationControls;

            Control[] inputProcessingControls = { btnProcessPayout };
            _inputProcessingControls = inputProcessingControls;

            Control[] inputPostProcessingControls = { btnPrintReceipt };
            _inputPostProcessingControls = inputPostProcessingControls;

            #endregion

            #region Output Controls

            Control[] outputReferenceNumberControls = {
                                                          lblPayoutTransactionNumber,
                                                          lblTransactionDate,
                                                          lblTransactionStatus,
                                                          lblPayoutAmount,
                                                          lblPayoutCurrency,
                                                          lblPayoutCountry,
                                                          //lblSenderMessage,

                                                          lblSenderLastName,
                                                          lblSenderFirstName,
                                                          //lblSenderMiddleName,
                                                          //lblSenderCity,
                                                          //lblSenderNationality,
                                                          
                                                          lblBeneficiaryLastName,
                                                          lblBeneficiaryFirstName,
                                                          //lblBeneficiaryMiddleName,
                                                          //lblBeneficiaryCity,
                                                          //lblBeneficiaryNationality
                                                      };
            _outputReferenceNumberControls = outputReferenceNumberControls;

            Control[] outputCustomerControls = {
                                                   txtCustomerNumber,
                                                   lblCustomerLastName,
                                                   lblCustomerFirstName,
                                                   lblCustomerMiddleName
                                               };
            _outputCustomerControls = outputCustomerControls;

            Control[] outputCustomerValidationControls = {
                                                             cboCustomerIDSubmitted,
                                                             txtCustomerIDSubmittedNumber
                                                         };
            _outputCustomerValidationControls = outputCustomerValidationControls;

            Control[] outputProcessingControls = {
                                                     lblProcessingStatus
                                                 };

            _outputProcessingControls = outputProcessingControls;

            Control[] outputPostProcessingControls = { };
            _outputPostProcessingControls = outputPostProcessingControls;

            #endregion
        }

        private void LookupReferenceNumber()
        {
            LookupTransactionRequest lookupTransactionRequest = new LookupTransactionRequest();
            lookupTransactionRequest.CebuanaBranchInformation = _parentRemittancePartnerControlMain.BranchInformation;
            lookupTransactionRequest.TransactionNumber = txtReferenceNumber.Text;

            LookupTransaction remittancePartnerLookupTransaction = new LookupTransaction(lookupTransactionRequest);
            remittancePartnerLookupTransaction.WebServiceURL = RemittancePartnerConfiguration.WebServiceURL;

            Utils.ToggleControlEnabledState(false, _inputReferenceNumberControls);
            timer1.Start();
            lblProcessingStatus.Text = "Searching for transaction...";

            backgroundWorkerRemittanceLookup.RunWorkerAsync(remittancePartnerLookupTransaction);
        }
        
        private void ProcessLookupTransactionResult()
        {
            switch (_remittancePartnerLookupTransaction.LookupTransactionResult.ResultCode)
            {
                case LookupTransactionResultCode.Successful:
                    MessageBox.Show(_remittancePartnerLookupTransaction.LookupTransactionResult.MessageToClient, "Lookup Transaction", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult dialogResult = LookupSummaryForm.ShowSummary(_remittancePartnerLookupTransaction.LookupTransactionResult);
                    switch (dialogResult)
                    {
                        case DialogResult.OK:
                            {
                                DisplayLookupTransactionDetails();
                                CheckTransactionCurrency();
                            }
                            break;
                        default:
                            ClearForm();
                            break;
                    }
                    break;
                case LookupTransactionResultCode.ServerError:
                    MessageBox.Show(_remittancePartnerLookupTransaction.LookupTransactionResult.MessageToClient, "Server Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ClearForm();
                    break;
                case LookupTransactionResultCode.Unsuccessful:
                    MessageBox.Show(_remittancePartnerLookupTransaction.LookupTransactionResult.MessageToClient, "Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    ClearForm();
                    break;
                default:
                    MessageBox.Show(_remittancePartnerLookupTransaction.LookupTransactionResult.MessageToClient, "Unsuccessful", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ClearForm();
                    break;
            }
        }

        private void DisplayLookupTransactionDetails()
        {
            if (_remittancePartnerLookupTransaction != null && _remittancePartnerLookupTransaction.LookupTransactionResult != null)
            {
                lblPayoutTransactionNumber.Text = _remittancePartnerLookupTransaction.LookupTransactionResult.TransactionNumber;
                lblTransactionDate.Text = _remittancePartnerLookupTransaction.LookupTransactionResult.TransactionDate.ToString("MMM dd, yyyy HH:mm:ss");
                string transactionStatus;
                switch (_remittancePartnerLookupTransaction.LookupTransactionResult.TransactionStatus)
                {
                    case TransactionStatus.ForPayout:
                        transactionStatus = "For Payout";
                        break;
                    case TransactionStatus.PaidOut:
                        transactionStatus = "Paid Out";
                        break;
                    case TransactionStatus.Cancelled:
                        transactionStatus = "Cancelled";
                        break;
                    case TransactionStatus.Blocked:
                        transactionStatus = "Blocked";
                        break;
                    case TransactionStatus.ProcessedForPayout:
                        transactionStatus = "Ready For Payout";
                        break;
                    default:
                        transactionStatus = "Unrecognized Status";
                        break;
                }
                lblTransactionStatus.Text = transactionStatus;

                lblPayoutAmount.Text = _remittancePartnerLookupTransaction.LookupTransactionResult.PayoutAmount.ToString("N2");
                lblPayoutCurrency.Text = _remittancePartnerLookupTransaction.LookupTransactionResult.PayoutCurrency;
                lblPayoutCountry.Text = Utils.GetCountryName(_remittancePartnerLookupTransaction.LookupTransactionResult.PayoutCountry);

                lblSenderLastName.Text = _remittancePartnerLookupTransaction.LookupTransactionResult.SenderLastName;
                lblSenderFirstName.Text = _remittancePartnerLookupTransaction.LookupTransactionResult.SenderFirstName;

                lblBeneficiaryLastName.Text = _remittancePartnerLookupTransaction.LookupTransactionResult.BeneficiaryLastName;
                lblBeneficiaryFirstName.Text = _remittancePartnerLookupTransaction.LookupTransactionResult.BeneficiaryFirstName;
            }
            else
            {
                MessageBox.Show("No lookup transaction result found.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void CheckTransactionCurrency()
        {
            string transactionCurrencyCode = _remittancePartnerLookupTransaction.LookupTransactionResult.PayoutCurrency.Trim().ToUpper();

            if (transactionCurrencyCode != "PHP")
            {
                lblPayoutCurrency.BackColor = Color.LightGreen;
                string message = string.Empty;

                if (transactionCurrencyCode == "USD")
                {
                    MultiCurrencyWrapper multiCurrencyWrapper = new MultiCurrencyWrapper(
                            transactionCurrencyCode,
                            _remittancePartnerLookupTransaction.LookupTransactionResult.PayoutAmount);

                    message = multiCurrencyWrapper.ReturnMessage;
                }
                else
                {
                    message = "Warning: This is a non-PHP payout transaction. Are you sure you want to proceed?";
                }

                DialogResult dialogResult = MessageBox.Show(message, "Non-PHP Transaction", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);

                if (dialogResult != DialogResult.Yes)
                {
                    ClearForm();
                    return;
                }
            }

            Utils.ToggleControlEnabledState(true, _inputCustomerControls);
            lblProcessingStatus.BackColor = Color.PaleGoldenrod;
            lblProcessingStatus.Text = "Please choose customer beneficiary.";
            SearchForCustomer();
        }

        private void SearchForCustomer()
        {
            try
            {

#if DEBUG
                bool isCustomerSelected = true;
#else
                bool isCustomerSelected = CustomerSearch.OpenSearch(_parentRemittancePartnerControlMain.BranchInformation.BranchUserID, _parentRemittancePartnerControlMain.BranchInformation.BranchCode, _parentRemittancePartnerControlMain.BranchInformation.BranchAreaCode, _parentRemittancePartnerControlMain.BranchInformation.BranchRegionCode, "REM");
#endif

                if (isCustomerSelected == true)
                {
                    CebuanaCustomerInformation cebuanaCustomerInformation = new CebuanaCustomerInformation();
                    List<CebuanaCustomerID> registeredIDs = new List<CebuanaCustomerID>();
                    //string customerIDTypesString;
                    //string customerIDNumbersString;
#if DEBUG
                    cebuanaCustomerInformation.CustomerNumber = "999990000025";
                    cebuanaCustomerInformation.LastName = "Rivero";
                    cebuanaCustomerInformation.FirstName = "Josef";
                    cebuanaCustomerInformation.MiddleName = string.Empty;
                    cebuanaCustomerInformation.City = "Metro Manila";
                    //customerIDTypesString = Convert.ToString("COMPANY ID~BARANGAY ID~MEMBERSHIP ID~TEST ID").Trim();
                    //customerIDNumbersString = Convert.ToString("000000~111111~000000~333333~").Trim();
                    cebuanaCustomerInformation.Country = "PH";
#else
                    cebuanaCustomerInformation.CustomerNumber = CustomerInformation.CustomerNumber;
                    cebuanaCustomerInformation.LastName = CustomerInformation.LastName;
                    cebuanaCustomerInformation.FirstName = CustomerInformation.FirstName;
                    cebuanaCustomerInformation.MiddleName = CustomerInformation.MiddleName;
                    cebuanaCustomerInformation.City = CustomerInformation.AddressTownCity;
                    //customerIDTypesString = Convert.ToString(CustomerInformation.PresentedIDType).Trim();
                    //customerIDNumbersString = Convert.ToString(CustomerInformation.PresentedIDNumber).Trim();
                    cebuanaCustomerInformation.Country = "PH";
#endif
                    //if (string.IsNullOrEmpty(customerIDTypesString) || string.IsNullOrEmpty(customerIDNumbersString))
                    //{
                    //    throw new RemittanceException("Incomplete ID details were retrieved from the customer's record.");
                    //}

                    //string[] customerIDTypes = customerIDTypesString.Split("~".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    //string[] customerIDNumbers = customerIDNumbersString.Split("~".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                    //if (customerIDTypes.Length != customerIDNumbers.Length)
                    //{
                    //    throw new RemittanceException("Number of ID Types does not match number of ID Numbers for this customer.");
                    //}

                    //for (int i = 0; i < customerIDTypes.Length; i++)
                    //{
                    //    CebuanaCustomerID customerID = new CebuanaCustomerID();
                    //    customerID.IDDescription = customerIDTypes[i].Trim();
                    //    customerID.IDNumber = customerIDNumbers[i].Trim();
                    //    registeredIDs.Add(customerID);
                    //}

                    //cebuanaCustomerInformation.RegisteredIDs = registeredIDs.ToArray();

                    DialogResult beneficiaryConfirmationDialogResult = BeneficiaryConfirmationForm.ShowForm(
                        _remittancePartnerLookupTransaction.LookupTransactionResult.BeneficiaryLastName,
                        _remittancePartnerLookupTransaction.LookupTransactionResult.BeneficiaryFirstName,
                        cebuanaCustomerInformation);

                    if (beneficiaryConfirmationDialogResult == DialogResult.Abort)
                    {
                        this.DialogResult = DialogResult.Abort;
                    }
                    else
                    {
                        _cebuanaCustomerInformation = cebuanaCustomerInformation;
                        lblProcessingStatus.Text = "Please enter customer validation details.";
                        Utils.ClearControls(_inputCustomerValidationControls);
                        Utils.ToggleControlEnabledState(true, _inputCustomerValidationControls);
                        DisplayCustomerDetails();
                    }
                }
                else
                {
                    this.DialogResult = DialogResult.Abort;
                }
            }
            catch (Exception error)
            {
                if (error is RemittanceException)
                {
                    Utils.DisplayErrorMessageBox(error);
                }
                else
                {
                    Utils.DisplayErrorMessageBox(new RemittanceException( "An error has occured in the CRMS module. Please contact ICT Support Desk.", error));
                }

                ClearForm();
            }
        }

        private void DisplayCustomerDetails()
        {
            if (_cebuanaCustomerInformation != null)
            {
                txtCustomerNumber.Text = _cebuanaCustomerInformation.CustomerNumber;
                lblCustomerLastName.Text = _cebuanaCustomerInformation.LastName;
                lblCustomerFirstName.Text = _cebuanaCustomerInformation.FirstName;
                lblCustomerMiddleName.Text = _cebuanaCustomerInformation.MiddleName;

                //try
                //{
                //    cboCustomerIDSubmitted.DataSource = _cebuanaCustomerInformation.RegisteredIDs;
                //    cboCustomerIDSubmitted.DisplayMember = "IDDescription";
                //    cboCustomerIDSubmitted.SelectedIndex = -1;
                //}
                //catch (Exception error)
                //{
                //    Utils.DisplayErrorMessageBox(new RemittanceException("An error has occured while building the customer ID list.", error));
                //    ClearForm();
                //}
            }
            else
            {
                MessageBox.Show("No customer selected.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void PayoutReferenceNumber()
        {
            if (!(_remittancePartnerLookupTransaction.LookupTransactionResult.TransactionStatus == TransactionStatus.ForPayout || _remittancePartnerLookupTransaction.LookupTransactionResult.TransactionStatus == TransactionStatus.ProcessedForPayout))
            {
                MessageBox.Show("This transaction is not for payout.", "Not available for payout", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                if (_remittancePartnerLookupTransaction.LookupTransactionResult.PayoutCurrency.Trim().ToUpper() != "PHP")
                {
                    DialogResult dialogResult = MessageBox.Show("You are about to process a non-PHP transaction. Do you want to continue?", "Non-PHP Transaction", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (dialogResult != DialogResult.Yes)
                    {
                        ClearForm();
                        return;
                    }
                }
                else
                {
                    if (Utils.IsAvailableBalanceSufficient(_remittancePartnerLookupTransaction.LookupTransactionResult.PayoutAmount) == false)
                    {
                        MessageBox.Show("Available balance is not enough for this payout transaction.", "Insufficient Balance", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }
            catch (Exception error)
            {
                Utils.DisplayErrorMessageBox(error);
                return;
            }
            
            PayoutTransactionRequest payoutTransactionRequest = new PayoutTransactionRequest();
            payoutTransactionRequest.CebuanaBranchInformation = _parentRemittancePartnerControlMain.BranchInformation;

            payoutTransactionRequest.TransactionNumber = _remittancePartnerLookupTransaction.LookupTransactionResult.TransactionNumber;
            payoutTransactionRequest.PayoutAmount = _remittancePartnerLookupTransaction.LookupTransactionResult.PayoutAmount;
            payoutTransactionRequest.PayoutCurrency = _remittancePartnerLookupTransaction.LookupTransactionResult.PayoutCurrency;
            payoutTransactionRequest.SendingCurrency = _remittancePartnerLookupTransaction.LookupTransactionResult.PayoutCurrency;
            payoutTransactionRequest.CurrencyConversionRate = 1M;
            payoutTransactionRequest.PayoutCountry = _remittancePartnerLookupTransaction.LookupTransactionResult.PayoutCountry;

            payoutTransactionRequest.SenderLastName = _remittancePartnerLookupTransaction.LookupTransactionResult.SenderLastName;
            payoutTransactionRequest.SenderFirstName = _remittancePartnerLookupTransaction.LookupTransactionResult.SenderFirstName;

            payoutTransactionRequest.ReceiverCustomerNumber = _cebuanaCustomerInformation.CustomerNumber;
            payoutTransactionRequest.ReceiverLastName = _remittancePartnerLookupTransaction.LookupTransactionResult.BeneficiaryLastName;
            payoutTransactionRequest.ReceiverFirstName = _remittancePartnerLookupTransaction.LookupTransactionResult.BeneficiaryFirstName;
            payoutTransactionRequest.ReceiverIDCode = _cebuanaCustomerIDs[cboCustomerIDSubmitted.SelectedIndex].IDCode;
            payoutTransactionRequest.ReceiverIDType = _cebuanaCustomerIDs[cboCustomerIDSubmitted.SelectedIndex].IDDescription;
            payoutTransactionRequest.ReceiverIDNumber = txtCustomerIDSubmittedNumber.Text;
            payoutTransactionRequest.ReceiverCity = _cebuanaCustomerInformation.City;
            payoutTransactionRequest.ReceiverCountry = _cebuanaCustomerInformation.Country;

            payoutTransactionRequest.SessionID = _remittancePartnerLookupTransaction.LookupTransactionResult.SessionID;
            
            PayoutTransaction payoutTransaction = new PayoutTransaction(payoutTransactionRequest);
            payoutTransaction.WebServiceURL = RemittancePartnerConfiguration.WebServiceURL;

            Utils.ToggleControlEnabledState(
                false,
                _inputCustomerControls,
                _inputCustomerValidationControls,
                _inputProcessingControls);
            lblProcessingStatus.Text = "Processing payout...";
            timer1.Start();
            backgroundWorkerRemittancePayout.RunWorkerAsync(payoutTransaction);
        }

        private void ProcessPayoutTransactionResult()
        {
            switch (_remittancePartnerPayoutTransaction.PayoutTransactionResult.ResultCode)
            {
                case PayoutTransactionResultCode.Successful:
                    MessageBox.Show(_remittancePartnerPayoutTransaction.PayoutTransactionResult.MessageToClient, "Payout Transaction", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Utils.ToggleControlEnabledState(true, _inputPostProcessingControls);
                    PostProcessing();
                    break;
                case PayoutTransactionResultCode.PartnerError:
                case PayoutTransactionResultCode.ServerError:
                    MessageBox.Show(_remittancePartnerPayoutTransaction.PayoutTransactionResult.MessageToClient, "WebService Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    lblProcessingStatus.Text = "Error";
                    lblProcessingStatus.BackColor = Color.Red;
                    break;
                case PayoutTransactionResultCode.Unsuccessful:
                default:
                    MessageBox.Show(_remittancePartnerPayoutTransaction.PayoutTransactionResult.MessageToClient, "Partner Remittance Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    lblProcessingStatus.BackColor = Color.Crimson;
                    lblProcessingStatus.Text = "Payout Unsuccessful.";
                    break;
            }

        }

        private void PostProcessing()
        {
            try
            {
                switch (_remittancePartnerLookupTransaction.LookupTransactionResult.PayoutCurrency.Trim().ToUpper())
                {
                    case "PHP":
                        {
                            Utils.BranchLedgerCashout(_remittancePartnerPayoutTransaction.PayoutTransactionRequest.PayoutCurrency, _remittancePartnerPayoutTransaction.PayoutTransactionRequest.PayoutAmount, _remittancePartnerPayoutTransaction.PayoutTransactionRequest.TransactionNumber, _remittancePartnerPayoutTransaction.PayoutTransactionRequest.ReceiverCustomerNumber);
                            PrintReceipt();
                        }
                        break;
                    case "USD":
                        {
                            PrintReceipt();
                            DollarPayoutPopup.frmDollarPayout dollarPopup = new DollarPayoutPopup.frmDollarPayout();

                            try
                            {
#if !DEBUG
                                dollarPopup.DisplayDollarPayout(
                                    _remittancePartnerLookupTransaction.LookupTransactionResult.MultiCurrencyPayoutCode,
                                    _remittancePartnerLookupTransaction.LookupTransactionResult.TransactionNumber,
                                    string.Format("{0} {1}", _remittancePartnerLookupTransaction.LookupTransactionResult.SenderFirstName, _remittancePartnerLookupTransaction.LookupTransactionResult.SenderLastName),
                                    _remittancePartnerPayoutTransaction.PayoutTransactionRequest.ReceiverCustomerNumber,
                                    string.Format("{0} {1} {2}", _cebuanaCustomerInformation.FirstName, _cebuanaCustomerInformation.MiddleName, _cebuanaCustomerInformation.LastName),
                                    _remittancePartnerLookupTransaction.LookupTransactionResult.PayoutAmount.ToString("N2"),
                                    false,
                                    string.Empty);
#endif
                            }
                            catch (Exception error)
                            {
                                throw new RemittanceException("Error in opening Dollar Payout. Please continue processing this transaction in the Dollar Payout module.", error);
                            }
                        }
                        break;
                    default:
                        {
                            PrintReceipt();
                            MessageBox.Show("Please continue processing in multi-currency module.", "Multi-Currency Payout.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        break;

                }

                lblProcessingStatus.BackColor = Color.LightGreen;
                lblProcessingStatus.Text = "Payout successful!";
            }
            catch (Exception error)
            {
                Utils.DisplayErrorMessageBox(error);
                lblProcessingStatus.BackColor = Color.Yellow;
                lblProcessingStatus.Text = "Payout successful, but with errors.";
            }
        }

        private void PrintReceipt()
        {
            MessageBox.Show("Will now print receipt. Please make sure a printer is online and there is paper in the tray. Click OK when ready.", "Print", MessageBoxButtons.OK, MessageBoxIcon.Information);

            bool printStatus = false;
            do
            {
                try
                {
                    if (System.IO.File.Exists(System.Environment.CurrentDirectory + @"\act_newform_InternationalRemittance.txt"))
                    {
                        NewPrintSettings();
                    }
                    else
                    {
                        printDocument1.Print();
                    }
                    printStatus = true;
                    lblProcessingStatus.BackColor = Color.DeepSkyBlue;
                    lblProcessingStatus.Text = "Transaction Complete";
                }
                catch
                {
                    DialogResult dialogResult = MessageBox.Show("An error has occured while printing this receipt. Please make sure a printer is online and there is paper in the tray.\n\nClick OK to try printing again\nClick Cancel to cancel printing right now.", "Print", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    if (dialogResult == DialogResult.Cancel)
                    {
                        break;
                    }
                }
            }
            while (printStatus == false);

            if (printStatus == false)
            {
                MessageBox.Show("Warning: Printing was not completed. Please make sure that you print a receipt by clicking \"Print Receipt\".", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                lblProcessingStatus.BackColor = Color.Orange;
                lblProcessingStatus.Text = "Receipt not printed.";
            }
        }

        private void ClearForm()
        {
            if (_isCalledFromStaticMethod)
            {
                CloseForm();
            }
            else
            {
                if (_remittancePartnerLookupTransaction != null && _remittancePartnerLookupTransaction.LookupTransactionResult.ResultCode == LookupTransactionResultCode.Successful)
                {
                    if (_remittancePartnerPayoutTransaction == null || _remittancePartnerPayoutTransaction.PayoutTransactionResult.ResultCode != PayoutTransactionResultCode.Successful)
                    {
                        MessageBox.Show("Placeholder for clearing form after lookup with no corresponding payout", "Placeholder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                InitializePayoutForm();
            }
        }

        private void CloseForm()
        {
            if (_isCalledFromStaticMethod)
            {
                if (_remittancePartnerLookupTransaction != null && _remittancePartnerLookupTransaction.LookupTransactionResult.ResultCode == LookupTransactionResultCode.Successful)
                {
                    if (_remittancePartnerPayoutTransaction == null || _remittancePartnerPayoutTransaction.PayoutTransactionResult.ResultCode != PayoutTransactionResultCode.Successful)
                    {
                        this.DialogResult = DialogResult.Abort;
                    }
                }

                this.Close();
                this.Dispose();
            }
            else
            {
                if (_remittancePartnerLookupTransaction != null && _remittancePartnerLookupTransaction.LookupTransactionResult.ResultCode == LookupTransactionResultCode.Successful)
                {
                    if (_remittancePartnerPayoutTransaction == null || _remittancePartnerPayoutTransaction.PayoutTransactionResult.ResultCode != PayoutTransactionResultCode.Successful)
                    {
                        MessageBox.Show("Placeholder for closing form after lookup with no corresponding payout", "Placeholder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                this.Close();
                this.Dispose();
            }
        }

        #endregion

        #region Events

        private void PayoutForm_Load(object sender, EventArgs e)
        {
            try
            {
                BuildCustomerIDTypesList();
            }
            catch (Exception error)
            {
                Utils.DisplayErrorMessageBox(error);
                CloseForm();
                return;
            }

            if (_isCalledFromStaticMethod)
            {
                btnClearAll.Enabled = false;
            }

            if (!string.IsNullOrEmpty(txtReferenceNumber.Text))
            {
                LookupReferenceNumber();
            }
        }

        private void btnLoadReferenceNumber_Click(object sender, EventArgs e)
        {
            LookupReferenceNumber();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (progressBar1.Value == 100)
            {
                progressBar1.Value = 0;
            }
            else
            {
                progressBar1.Value = progressBar1.Value + 1;
            }
        }

        private void backgroundWorkerRemittanceLookup_DoWork(object sender, DoWorkEventArgs e)
        {
            LookupTransaction lookupTransaction = e.Argument as LookupTransaction;
            lookupTransaction.Lookup();
            e.Result = lookupTransaction;
        }

        private void backgroundWorkerRemittanceLookup_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            timer1.Stop();
            progressBar1.Value = 100;
            lblProcessingStatus.Text = "Done.";

            if (e.Error != null)
            {
                lblProcessingStatus.Text = "Error";
                lblProcessingStatus.BackColor = Color.Red;
                Utils.DisplayErrorMessageBox(e.Error);
                ClearForm();
            }
            else if (e.Cancelled == true)
            {
                lblProcessingStatus.Text = "Processing cancelled.";
                lblProcessingStatus.BackColor = Color.Blue;
                MessageBox.Show("Lookup cancelled.", "Cancelled.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
            }
            else if (e.Result == null)
            {
                lblProcessingStatus.Text = "Error";
                lblProcessingStatus.BackColor = Color.Red;
                MessageBox.Show("No result received. Please contact ICT Support Desk", "Result Missing", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                ClearForm();
            }
            else
            {
                LookupTransaction lookupTransaction = e.Result as LookupTransaction;
                _remittancePartnerLookupTransaction = lookupTransaction;
                ProcessLookupTransactionResult();
            }

            progressBar1.Value = 0;
        }

        private void cboCustomerIDSubmitted_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboCustomerIDSubmitted.SelectedIndex > -1
                && txtCustomerIDSubmittedNumber.Text.Trim() != string.Empty
                )
            {
                //txtCustomerIDSubmittedNumber.Text = _cebuanaCustomerInformation.RegisteredIDs[cboCustomerIDSubmitted.SelectedIndex].IDNumber;
                Utils.ToggleControlEnabledState(true, _inputProcessingControls);
                lblProcessingStatus.Text = "Click Process Payout to payout transaction.";
                lblProcessingStatus.BackColor = Color.LightGreen;
            }
            else
            {
                Utils.ToggleControlEnabledState(false, _inputProcessingControls);
                txtCustomerIDSubmittedNumber.Text = string.Empty;
                lblProcessingStatus.Text = "Please enter customer validation details.";
                lblProcessingStatus.BackColor = Color.PaleGoldenrod;
            }
        }

        private void txtCustomerIDSubmittedNumber_TextChanged(object sender, EventArgs e)
        {
            if (cboCustomerIDSubmitted.SelectedIndex > -1 && txtCustomerIDSubmittedNumber.Text.Trim() != string.Empty)
            {
                Utils.ToggleControlEnabledState(true, _inputProcessingControls);
                lblProcessingStatus.Text = "Click Process Payout to payout transaction.";
                lblProcessingStatus.BackColor = Color.LightGreen;
            }
            else
            {
                Utils.ToggleControlEnabledState(false, _inputProcessingControls);
                lblProcessingStatus.Text = "Please enter customer validation details.";
                lblProcessingStatus.BackColor = Color.PaleGoldenrod;
            }
        }

        private void btnProcessPayout_Click(object sender, EventArgs e)
        {
            PayoutReferenceNumber();
        }

        private void backgroundWorkerRemittancePayout_DoWork(object sender, DoWorkEventArgs e)
        {
            PayoutTransaction payoutTransaction = e.Argument as PayoutTransaction;
            payoutTransaction.Payout();
            e.Result = payoutTransaction;
        }

        private void backgroundWorkerRemittancePayout_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            timer1.Stop();
            progressBar1.Value = 100;
            lblProcessingStatus.Text = "Done";

            if (e.Error != null)
            {
                lblProcessingStatus.Text = "Error";
                lblProcessingStatus.BackColor = Color.Red;
                Utils.DisplayErrorMessageBox(e.Error);
            }
            else if (e.Cancelled == true)
            {
                lblProcessingStatus.Text = "Processing cancelled";
                lblProcessingStatus.BackColor = Color.Blue;
                MessageBox.Show("Payout cancelled.", "Cancelled.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Utils.ToggleControlEnabledState(
                    true,
                    _inputCustomerControls,
                    _inputCustomerValidationControls,
                    _inputProcessingControls);
                
            }
            else if (e.Result == null)
            {
                lblProcessingStatus.Text = "Error";
                lblProcessingStatus.BackColor = Color.Red;
                MessageBox.Show("No result received. Please contact ICT Support Desk", "Result Missing", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Utils.ToggleControlEnabledState(
                    true,
                    _inputCustomerControls,
                    _inputCustomerValidationControls,
                    _inputProcessingControls);                
            }
            else
            {
                PayoutTransaction payoutTransaction = e.Result as PayoutTransaction;
                _remittancePartnerPayoutTransaction = payoutTransaction;
                ProcessPayoutTransactionResult();
            }

            progressBar1.Value = 0;
        }

        private void btnPrintReceipt_Click(object sender, EventArgs e)
        {
            PrintReceipt();
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            PrintParameters printParameters = new PrintParameters();
            printParameters.ReferenceNumber = _remittancePartnerPayoutTransaction.PayoutTransactionRequest.TransactionNumber;
            printParameters.TransactionDate = _remittancePartnerPayoutTransaction.PayoutTransactionResult.PayoutDate;
            printParameters.CustomerNumber = _remittancePartnerPayoutTransaction.PayoutTransactionRequest.ReceiverCustomerNumber;
            printParameters.SendingBranch = RemittancePartnerConfiguration.PartnerName;
            string senderName = string.IsNullOrEmpty(_remittancePartnerPayoutTransaction.PayoutTransactionRequest.SenderFirstName) ? _remittancePartnerPayoutTransaction.PayoutTransactionRequest.SenderLastName : String.Format("{0}, {1}", _remittancePartnerPayoutTransaction.PayoutTransactionRequest.SenderLastName, _remittancePartnerPayoutTransaction.PayoutTransactionRequest.SenderFirstName);
            printParameters.SenderName = senderName;
            string receiverName = string.IsNullOrEmpty(_remittancePartnerPayoutTransaction.PayoutTransactionRequest.ReceiverFirstName) ? _remittancePartnerPayoutTransaction.PayoutTransactionRequest.ReceiverLastName : String.Format("{0}, {1}", _remittancePartnerPayoutTransaction.PayoutTransactionRequest.ReceiverLastName, _remittancePartnerPayoutTransaction.PayoutTransactionRequest.ReceiverFirstName);
            printParameters.ReceiverName = receiverName;
            printParameters.PayoutBranch = String.Format("{0} {1}", _parentRemittancePartnerControlMain.BranchInformation.BranchCode, _parentRemittancePartnerControlMain.BranchInformation.BranchName);
            printParameters.PrincipalAmountCurrency = _remittancePartnerPayoutTransaction.PayoutTransactionRequest.PayoutCurrency;
            printParameters.PrincipalAmount = _remittancePartnerPayoutTransaction.PayoutTransactionRequest.PayoutAmount;
            printParameters.ServiceCharge = decimal.Zero;
            printParameters.DiscountAmount = decimal.Zero;
            printParameters.BranchUserID = _remittancePartnerPayoutTransaction.PayoutTransactionRequest.CebuanaBranchInformation.BranchUserID;
            printParameters.BranchOperatingHours = Utils.GetBranchOperatingHours(_remittancePartnerPayoutTransaction.PayoutTransactionRequest.CebuanaBranchInformation.BranchCode);
            Utils.PrintRemittanceReceipt(printParameters, e);
        }

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            if (!(backgroundWorkerRemittanceLookup.IsBusy || backgroundWorkerRemittancePayout.IsBusy))
            {
                DialogResult dialogResult = MessageBox.Show("Are you sure you want to clear this form?", "Clear Form", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                if (dialogResult == DialogResult.Yes)
                {
                    ClearForm();
                }
            }
        }

        private void btnCloseForm_Click(object sender, EventArgs e)
        {
            if (!(backgroundWorkerRemittanceLookup.IsBusy || backgroundWorkerRemittancePayout.IsBusy))
            {
                DialogResult dialogResult = MessageBox.Show("Are you sure you want to close this form?", "Close Form", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                if (dialogResult == DialogResult.Yes)
                {
                    CloseForm();
                }
            }
        }

        #endregion

        #region Print Settings
        public void NewPrintSettings()
        {
            PrintParameters printParameters = new PrintParameters();
#if DEBUG
            printParameters.MarketingMessage = "Have a good day.";
#else
            printParameters.MarketingMessage = B2BUtilities.GlobalFunction.GetCompMessage();
#endif
            printParameters.ReferenceNumber = _remittancePartnerPayoutTransaction.PayoutTransactionRequest.TransactionNumber;
            printParameters.TransactionDate = _remittancePartnerPayoutTransaction.PayoutTransactionResult.PayoutDate;
            printParameters.CustomerNumber = _remittancePartnerPayoutTransaction.PayoutTransactionRequest.ReceiverCustomerNumber;
            printParameters.SendingBranch = RemittancePartnerConfiguration.PartnerName;
            string senderName = String.Format("{0}, {1}", _remittancePartnerPayoutTransaction.PayoutTransactionRequest.SenderLastName, _remittancePartnerPayoutTransaction.PayoutTransactionRequest.SenderFirstName);
            printParameters.SenderName = Utils.AddEllipsis(senderName, 14);
            string receiverName = String.Format("{0}, {1}", _remittancePartnerPayoutTransaction.PayoutTransactionRequest.ReceiverLastName, _remittancePartnerPayoutTransaction.PayoutTransactionRequest.ReceiverFirstName);
            printParameters.ReceiverName = Utils.AddEllipsis(receiverName, 14);
            printParameters.PayoutBranch = String.Format("{0} {1}", _parentRemittancePartnerControlMain.BranchInformation.BranchCode, _parentRemittancePartnerControlMain.BranchInformation.BranchName);
            printParameters.PrincipalAmountCurrency = _remittancePartnerPayoutTransaction.PayoutTransactionRequest.PayoutCurrency;
            printParameters.PrincipalAmount = _remittancePartnerPayoutTransaction.PayoutTransactionRequest.PayoutAmount;
            printParameters.ServiceCharge = decimal.Zero;
            printParameters.DiscountAmount = decimal.Zero;
            printParameters.BranchUserID = _remittancePartnerPayoutTransaction.PayoutTransactionRequest.CebuanaBranchInformation.BranchUserID;
            printParameters.BranchOperatingHours = Utils.GetBranchOperatingHours(_remittancePartnerPayoutTransaction.PayoutTransactionRequest.CebuanaBranchInformation.BranchCode);

            string[] detailArray = new string[16];
            detailArray[0] = string.Format("#{0}", printParameters.ReferenceNumber); // Control number
            detailArray[1] = printParameters.PayoutBranch;  // Payout branch
            detailArray[2] = printParameters.BranchOperatingHours; // Business hours
            detailArray[3] = printParameters.ReceiverName; // Receiver Name
            detailArray[4] = printParameters.CustomerNumber; // Client number
            // detailArray[5] = decimal.Zero.ToString(); // CLP Points
            detailArray[6] = printParameters.MarketingMessage; // Company Message 1
            detailArray[7] = string.Format("SENDER : {0}", printParameters.SenderName); // Sender Name     
            detailArray[8] = printParameters.SendingBranch; // Remittance Partner
            detailArray[9] = printParameters.TransactionDate.ToString("MM/dd/yyyy HH:mm"); // Date
            detailArray[10] = printParameters.BranchUserID; // Processed by
            detailArray[11] = printParameters.PrincipalAmount.ToString(); // Principal amount
            detailArray[12] = printParameters.ServiceCharge.ToString(); // Service charge
            detailArray[13] = printParameters.DiscountAmount.ToString(); // Discount
            detailArray[14] = printParameters.TotalAmount.ToString(); // Total
            detailArray[15] = printParameters.PrincipalAmountCurrency.ToString(); // Currency
#if DEBUG
            //************************************* PRINT from xml file *************************************//
            //PrintModule.PrintUtil.PrintFromFile(@"C:\Users\esfranco\Desktop\InternationalPayoutNew.xml", detailArray);            

            //************************************* PRINT from database *************************************//

            PrintModule.PrintUtil.Print("NRPF-InternationalPayout", detailArray, true);
#else
            PrintModule.PrintUtil.Print("NRPF-InternationalPayout", detailArray, false);
#endif
        }
        #endregion
    }
}
