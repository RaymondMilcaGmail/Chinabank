﻿using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using CashModule;
using CLPFunctions;
using CommonLibrary;
using System.Data.SqlClient;

namespace ChinaBankControlLibrary
{
    class Utils
    {
        internal static void DisplayErrorMessageBox(Exception exception)
        {
            if (exception is SoapException)
            {
                SoapException soapError = exception as SoapException;
                MessageBox.Show("An error in calling the webservice has occured. Please contact ICT Support Desk.", "SoapError", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (exception is WebException)
            {
                WebException webError = exception as WebException;
                MessageBox.Show("An error in network connectivity has occured. Please contact ICT Support Desk.", "Web Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (exception is RemittanceException)
            {
                RemittanceException remittanceError = exception as RemittanceException;
                MessageBox.Show(remittanceError.Message, "Remittance Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show(String.Format("An error has occurred. Please contact ICT Support Desk. {0}{0}Details:{0}{1}", Environment.NewLine, exception.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        internal static string GetMacAddress()
        {
            string macAddress = "NONE";

            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface adapter in networkInterfaces)
            {
                if (adapter.GetPhysicalAddress().ToString().Trim().Length > 0 && adapter.OperationalStatus == OperationalStatus.Up)
                {
                    macAddress = adapter.GetPhysicalAddress().ToString();
                    break;
                }
            }

            return macAddress;
        }

        internal static bool IsURIOnline(string uriString)
        {
            bool isOnline = false;

            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(uriString);
                httpWebRequest.Timeout = 10000;
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                    isOnline = true;

                httpWebResponse.Close();
            }
            catch
            {

            }

            return isOnline;
        }

        internal static void ClearControls(params Control[] controls)
        {
            foreach (Control control in controls)
            {
                if (control is Button)
                {
                    continue;
                }
                else if (control is TextBox)
                {
                    TextBox textBox = control as TextBox;
                    textBox.Text = string.Empty;
                }
                else if (control is NumericUpDown)
                {
                    NumericUpDown numericUpDown = control as NumericUpDown;
                    numericUpDown.Value = decimal.Zero;
                }
                else if (control is ComboBox)
                {
                    ComboBox comboBox = control as ComboBox;
                    //if (comboBox.DataSource != null)
                    //{
                    //    comboBox.DataSource = null;
                    //}
                    //else
                    //{
                    //    comboBox.Items.Clear();
                    //}
                    comboBox.SelectedIndex = -1;
                }
                else if (control is Label)
                {
                    Label label = control as Label;
                    label.Text = string.Empty;
                }
                else if (control is Panel)
                {
                    Panel panel = control as Panel;
                    Control[] controlsInPanel = new Control[panel.Controls.Count];
                    panel.Controls.CopyTo(controlsInPanel, 0);
                    ClearControls(controlsInPanel);
                }
            }
        }

        internal static void ClearControls(params Control[][] controlArrays)
        {
            foreach (Control[] controls in controlArrays)
            {
                ClearControls(controls);
            }
        }

        internal static void ToggleControlEnabledState(bool isEnabled, params Control[] controls)
        {
            foreach (Control control in controls)
            {
                control.Enabled = isEnabled;
            }
        }

        internal static void ToggleControlEnabledState(bool isEnabled, params Control[][] controlArrays)
        {
            foreach (Control[] controls in controlArrays)
            {
                ToggleControlEnabledState(isEnabled, controls);
            }
        }

        internal static void ToggleControlVisibility(bool isVisible, params Control[] controls)
        {
            foreach (Control control in controls)
            {
                control.Visible = isVisible;
            }
        }

        internal static void ToggleControlVisibility(bool isVisible, params Control[][] controlArrays)
        {
            foreach (Control[] controls in controlArrays)
            {
                ToggleControlVisibility(isVisible, controls);
            }
        }

        internal static string GetCountryName(string countryCode)
        {
            string countryName = countryCode;

            if (countryCode.Length == 2)
            {
                try
                {
                    RegionInfo regionInfo = new RegionInfo(countryCode);
                    countryName = string.Format("[{0}] {1}", countryCode, regionInfo.DisplayName);
                }
                catch
                {
                }
            }
            else if (countryCode.Length == 3)
            {

                CultureInfo[] allCultureInfos = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures);

                try
                {
                    foreach (CultureInfo ci in allCultureInfos)
                    {
                        RegionInfo regionInfo = new RegionInfo(ci.LCID);
                        if (regionInfo.ThreeLetterISORegionName == countryCode)
                        {
                            countryName = string.Format("[{0}] {1}", countryCode, regionInfo.DisplayName);
                            break;
                        }
                    }
                }
                catch
                {
                }
            }

            return countryName;
        }

        internal static void BranchLedgerCashout(string currency, decimal cashoutAmount, string referenceNumber, string customerNumber)
        {
            if (currency.ToUpper() != "PHP")
            {
                MessageBox.Show("Please register this transaction in the Multi-Currency module.", "Non-PHP Transaction", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
#if DEBUG
            return;
#endif
            try
            {
                DataSet dataSet = CommonHelper.GetAcctEntrySetup((int)CommonHelper.ProductTxnType.Payout);

                if (dataSet == null || dataSet.Tables.Count <= 0)
                {
                    throw new RemittanceException("An error has occured while setting up the branch ledger. Please manually cash out this transaction.");
                }

                foreach (DataTable dt in dataSet.Tables)
                {
                    if (dt.TableName.ToString().Trim().ToUpper() == "DTMAIN")
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            dr["AcctEntryMainID"] = Helper.GetNextKey("tblAcctEntryMain");
                            dr["ProductTxnTypeID"] = (int)CommonHelper.ProductTxnType.Payout;
                            dr["TxnReferenceID"] = 0; //default for alphanumeric reference #s //Convert.ToInt32(referenceNumber); 
                            //dr["TxnReferenceID"] = Convert.ToInt32(referenceNumber); 
                            dr["Remarks"] = referenceNumber.Replace("-", "_") + " - PAYING TRANSACTION";
                        }
                    }

                    if (dt.TableName.ToString().Trim().ToUpper() == "DTDETAIL")
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            dr["AcctEntryDetailID"] = Helper.GetNextKey("tblAcctEntryDetail");
                            switch (dr["AcctVariableCode"].ToString().Trim().ToUpper())
                            {
                                case "RPTU": // Remittance Principal for Tieups Payout
                                    dr["Amount"] = cashoutAmount;
                                    break;
                            }
                        }
                    }
                }

                bool writeStatus = CommonHelper.InsertAcctEntry(dataSet);
                int writeAttempts = 1;

                while (writeAttempts < 3)
                {
                    if (writeStatus == true)
                        break;

                    System.Threading.Thread.Sleep(5000);
                    writeStatus = CommonHelper.InsertAcctEntry(dataSet);
                    writeAttempts++;
                }

                if (writeStatus == true)
                {
                    if (dataSet.Tables.Contains("DTCLP"))
                    {

                        if (dataSet.Tables["DTCLP"].Rows.Count > 0)
                        {
                            dataSet.Tables["DTCLP"].Rows[0]["CustomerNo"] = customerNumber;
                            dataSet.Tables["DTCLP"].Rows[0]["CLPAmount"] = 0;
                        }
                        else
                        {
                            //creating new data row
                            DataRow dtaRow = dataSet.Tables["DTCLP"].NewRow();
                            dtaRow["CustomerNo"] = customerNumber;
                            dtaRow["CLPAmount"] = 0;
                            //adding new row to table
                            dataSet.Tables["DTCLP"].Rows.Add(dtaRow);
                        }
                    }
                    else
                    {
                        //Creating new data table
                        DataTable dtaCLPTable = new DataTable("DTCLP");
                        dtaCLPTable.Columns.Add("CustomerNo", typeof(string));
                        dtaCLPTable.Columns.Add("CLPAmount", typeof(decimal));
                        //creating new data row
                        DataRow dtaRow = dtaCLPTable.NewRow();
                        dtaRow["CustomerNo"] = customerNumber;
                        dtaRow["CLPAmount"] = 0;
                        //adding new row to table
                        dtaCLPTable.Rows.Add(dtaRow);
                        //addting new table to dataset
                        dataSet.Tables.Add(dtaCLPTable);
                    }

                    try
                    {
                        CLP.TieupRewards(dataSet, "");
                    }
                    catch
                    { }

                }

                if (writeStatus == false)
                {
                    throw new RemittanceException("Could not insert branch ledger entry. Please manually cash out this transaction.");
                }
            }
            catch (RemittanceException remittanceError)
            {
                throw remittanceError;
            }
            catch (Exception error)
            {
                throw new RemittanceException("An error has occured while saving to the branch ledger. Please manually cash out this transaction.", error);
            }
        }

        internal static bool IsAvailableBalanceSufficient(decimal amount)
        {
            try
            {

#if DEBUG
                return true;
#else
                bool isBalanceSufficient = false;
                isBalanceSufficient = CommonHelper.GetOpeningSystemBalance(CommonHelper.UserBOD) > amount;
                return isBalanceSufficient;
#endif


            }
            catch (RemittanceException remittanceError)
            {
                throw remittanceError;
            }
            catch (Exception error)
            {
                throw new RemittanceException("An error has occured while checking for available cash.", error);
            }
        }

        internal static void PrintRemittanceReceipt(PrintParameters printParameters, System.Drawing.Printing.PrintPageEventArgs e)
        {
            try
            {
                Graphics graphics = e.Graphics;
                Brush brush = Brushes.Black;
                Font bodyfont = new Font("Courier New", 10, FontStyle.Regular);

                /** START: Title Line **/
                graphics.DrawString(String.Format("TIE-UP PAYOUT - {0}", printParameters.SendingBranch), new Font("Courier New", 12, FontStyle.Bold), Brushes.Black, 1, 310);
                /** END: Title Line **/

                /** START Top Line **/
                if (string.IsNullOrEmpty(printParameters.BranchOperatingHours) == false)
                {
                    graphics.DrawString(String.Format("BUSINESS HRS: {0}", printParameters.BranchOperatingHours), bodyfont, brush, 260, 330);
                }
                graphics.DrawString(String.Format("DATE: {0:MM/dd/yyyy HH:mm:ss}  Customer ID: {1}", printParameters.TransactionDate, printParameters.CustomerNumber), bodyfont, brush, 260, 350);
                graphics.DrawString(String.Format("#{0}", printParameters.ReferenceNumber), new Font("Courier New", 12, FontStyle.Bold), brush, 1, 330);
                /** END Top Line **/

                /** START: Left Column **/
                graphics.DrawString(String.Format("SENDER'S NAME: {0}", printParameters.SenderName), bodyfont, brush, 1, 370);
                graphics.DrawString(String.Format("SENDING BRANCH: {0}", printParameters.SendingBranch), bodyfont, brush, 1, 390);
                graphics.DrawString(String.Format("RECEIVER'S NAME: {0}", printParameters.ReceiverName), bodyfont, brush, 1, 420);
                graphics.DrawString(String.Format("PAYOUT BRANCH: {0}", printParameters.PayoutBranch), bodyfont, brush, 1, 440);
                graphics.DrawString(String.Format("PROCESSED BY: {0}\t\t REFERENCE No.:{1}", printParameters.BranchUserID, printParameters.ReferenceNumber), bodyfont, brush, 1, 460);
                /** END: Left Column **/

                /** START: Right Column **/
                graphics.DrawString(String.Format("     PRIN. AMOUNT : {0} {1}", printParameters.PrincipalAmountCurrency, printParameters.PrincipalAmount.ToString("N").PadLeft(12, ' ')), bodyfont, brush, 400, 370);
                graphics.DrawString(String.Format("   SERVICE CHARGE : {0} {1}", printParameters.PrincipalAmountCurrency, printParameters.ServiceCharge.ToString("N").PadLeft(12, ' ')), bodyfont, brush, 400, 390);
                graphics.DrawString(String.Format("         DISCOUNT : {0} {1}", printParameters.PrincipalAmountCurrency, printParameters.DiscountAmount.ToString("N").PadLeft(12, ' ')), bodyfont, brush, 400, 410);
                graphics.DrawString(String.Format("            TOTAL : {0} {1}", printParameters.PrincipalAmountCurrency, printParameters.TotalAmount.ToString("N").PadLeft(12, ' ')), bodyfont, brush, 400, 430);
                /** END: Right Column **/
            }
            catch (Exception error)
            {
                throw new RemittanceException("An error has occured while printing this receipt.", error);
            }
        }

        internal static string GetBranchOperatingHours(string branchCode)
        {
            try
            {
                AvailmentRatioModule.AvailmentRatio availmentRatio = new AvailmentRatioModule.AvailmentRatio();

                string branchWeekDayOperatingHours = availmentRatio.GetBusinessHours(branchCode.Trim()) ?? string.Empty;
                string branchSundayOperatingHours = availmentRatio.GetSundayBusinessHours(branchCode.Trim()) ?? string.Empty;

                if (string.IsNullOrEmpty(branchWeekDayOperatingHours) == true)
                {
                    if (string.IsNullOrEmpty(branchSundayOperatingHours) == true)
                    {
                        return string.Empty;
                    }
                    else
                    {
                        return string.Format(@"{2} (Sun)", Environment.NewLine, branchWeekDayOperatingHours, branchSundayOperatingHours);
                    }
                }

                if (string.IsNullOrEmpty(branchSundayOperatingHours) == true)
                {
                    return string.Format(@"{1} (Mon-Sat)", Environment.NewLine, branchWeekDayOperatingHours, branchSundayOperatingHours);
                }

                return string.Format(@"{1} (Mon-Sat) {2} (Sun)", Environment.NewLine, branchWeekDayOperatingHours, branchSundayOperatingHours);

            }
            catch
            {
                return string.Empty;
            }
        }

        internal static string AddEllipsis(string fullName, int baseLength)
        {
            string[] arrFullName = fullName.Split(',');
            string lastName = arrFullName[0];
            string firstName = arrFullName[1].Trim();
            string ellipsis = "...";
            string finalFullName = string.Empty;

            lastName = (lastName.Length > baseLength) ? lastName.Substring(0, baseLength) : lastName = lastName.Substring(0);
            firstName = (firstName.Length > baseLength) ? firstName.Substring(0, baseLength) + ellipsis : firstName = firstName.Substring(0);
            finalFullName = string.Format("{0}, {1}", lastName, firstName);
            return finalFullName;
        }
    }
}
