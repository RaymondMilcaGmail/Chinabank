using System;
using System.Windows.Forms;
using ChinaBankControlLibrary.RemittancePartnerWebReference;

namespace ChinaBankControlLibrary
{
    public partial class LookupSummaryForm : Form
    {
        #region Contructors

        private LookupSummaryForm(LookupTransactionResult lookupTransactionResult)
        {
            InitializeComponent();
            _lookupTransactionResult = lookupTransactionResult;
        }

        #endregion

        #region Fields

        private LookupTransactionResult _lookupTransactionResult;

        #endregion

        #region Methods

        public static DialogResult ShowSummary(LookupTransactionResult lookupTransactionResult)
        {
            LookupSummaryForm lookupSummaryForm = new LookupSummaryForm(lookupTransactionResult);
            return lookupSummaryForm.ShowDialog();
        }

        #endregion

        #region Events

        private void LookupSummaryForm_Load(object sender, EventArgs e)
        {
            string beneficiaryName = string.IsNullOrEmpty(_lookupTransactionResult.BeneficiaryFirstName) ? _lookupTransactionResult.BeneficiaryLastName : string.Format("{0}, {1}", _lookupTransactionResult.BeneficiaryLastName, _lookupTransactionResult.BeneficiaryFirstName);
            lblBeneficiaryName.Text = beneficiaryName;
            lblTransactionNumber.Text = string.Format("{0}", _lookupTransactionResult.TransactionNumber);
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.F1:
                    this.DialogResult = DialogResult.Cancel;
                    return true;
                case Keys.F2:
                    this.DialogResult = DialogResult.OK;
                    return true;
                case Keys.Escape:
                    this.DialogResult = DialogResult.Abort;
                    return true;
                default:
                    return base.ProcessDialogKey(keyData);
            }
        }

        #endregion

    }
}
