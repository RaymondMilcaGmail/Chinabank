using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using ChinaBankControlLibrary;
using System.Security.Cryptography;
using System.Net;

namespace RemittancePartnerApplication
{
    public partial class RemittancePartnerForm : Form
    {
        public RemittancePartnerForm()
        {
            InitializeComponent();
        }

        private void btnSearchReferenceNumber_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = RemittancePartnerControlMain.ShowPayoutForm(txtReferenceNumber.Text);

            if (dialogResult == DialogResult.Abort)
            {
                MessageBox.Show("Placeholder for abortion of payout dialog.");
            }
        }
    }
}
