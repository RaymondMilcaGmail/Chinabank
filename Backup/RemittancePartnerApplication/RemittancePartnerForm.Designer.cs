namespace RemittancePartnerApplication
{
    partial class RemittancePartnerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnSearchReferenceNumber = new System.Windows.Forms.Button();
            this.txtReferenceNumber = new System.Windows.Forms.TextBox();
            this.remittancePartnerControlMain1 = new ChinaBankControlLibrary.RemittancePartnerControlMain();
            this.SuspendLayout();
            // 
            // btnSearchReferenceNumber
            // 
            this.btnSearchReferenceNumber.Location = new System.Drawing.Point(13, 224);
            this.btnSearchReferenceNumber.Name = "btnSearchReferenceNumber";
            this.btnSearchReferenceNumber.Size = new System.Drawing.Size(133, 23);
            this.btnSearchReferenceNumber.TabIndex = 1;
            this.btnSearchReferenceNumber.Text = "Search";
            this.btnSearchReferenceNumber.UseVisualStyleBackColor = true;
            this.btnSearchReferenceNumber.Click += new System.EventHandler(this.btnSearchReferenceNumber_Click);
            // 
            // txtReferenceNumber
            // 
            this.txtReferenceNumber.Location = new System.Drawing.Point(13, 198);
            this.txtReferenceNumber.Name = "txtReferenceNumber";
            this.txtReferenceNumber.Size = new System.Drawing.Size(133, 20);
            this.txtReferenceNumber.TabIndex = 3;
            this.txtReferenceNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // remittancePartnerControlMain1
            // 
            this.remittancePartnerControlMain1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.remittancePartnerControlMain1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.remittancePartnerControlMain1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.remittancePartnerControlMain1.Location = new System.Drawing.Point(13, 13);
            this.remittancePartnerControlMain1.Name = "remittancePartnerControlMain1";
            this.remittancePartnerControlMain1.Size = new System.Drawing.Size(428, 178);
            this.remittancePartnerControlMain1.TabIndex = 0;
            // 
            // RemittancePartnerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(454, 266);
            this.Controls.Add(this.txtReferenceNumber);
            this.Controls.Add(this.btnSearchReferenceNumber);
            this.Controls.Add(this.remittancePartnerControlMain1);
            this.Name = "RemittancePartnerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RemittancePartnerForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ChinaBankControlLibrary.RemittancePartnerControlMain remittancePartnerControlMain1;
        private System.Windows.Forms.Button btnSearchReferenceNumber;
        private System.Windows.Forms.TextBox txtReferenceNumber;


    }
}