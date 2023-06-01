namespace Managed_Data_Request
{
    partial class Form1
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
            this.buttonConnect = new System.Windows.Forms.Button();
            this.buttonDisconnect = new System.Windows.Forms.Button();
            this.buttonRequestSixPackData = new System.Windows.Forms.Button();
            this.richResponse = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonRequestMakeFaireData = new System.Windows.Forms.Button();
            this.cbComPort = new System.Windows.Forms.ComboBox();
            this.labelCOMPort = new System.Windows.Forms.Label();
            this.labelBaudRate = new System.Windows.Forms.Label();
            this.cbBaudRate = new System.Windows.Forms.ComboBox();
            this.buttonSendSingleMessageBlock = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonConnect
            // 
            this.buttonConnect.Location = new System.Drawing.Point(12, 38);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(131, 23);
            this.buttonConnect.TabIndex = 0;
            this.buttonConnect.Text = "Connect to FSX";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // buttonDisconnect
            // 
            this.buttonDisconnect.Location = new System.Drawing.Point(12, 168);
            this.buttonDisconnect.Name = "buttonDisconnect";
            this.buttonDisconnect.Size = new System.Drawing.Size(131, 23);
            this.buttonDisconnect.TabIndex = 1;
            this.buttonDisconnect.Text = "Disconnect from FSX";
            this.buttonDisconnect.UseVisualStyleBackColor = true;
            this.buttonDisconnect.Click += new System.EventHandler(this.buttonDisconnect_Click);
            // 
            // buttonRequestSixPackData
            // 
            this.buttonRequestSixPackData.Location = new System.Drawing.Point(12, 74);
            this.buttonRequestSixPackData.Name = "buttonRequestSixPackData";
            this.buttonRequestSixPackData.Size = new System.Drawing.Size(131, 41);
            this.buttonRequestSixPackData.TabIndex = 2;
            this.buttonRequestSixPackData.Text = "Request Aircraft SixPack Data";
            this.buttonRequestSixPackData.UseVisualStyleBackColor = true;
            this.buttonRequestSixPackData.Click += new System.EventHandler(this.buttonRequestSixPackData_Click);
            // 
            // richResponse
            // 
            this.richResponse.Location = new System.Drawing.Point(149, 40);
            this.richResponse.Name = "richResponse";
            this.richResponse.ReadOnly = true;
            this.richResponse.Size = new System.Drawing.Size(410, 151);
            this.richResponse.TabIndex = 3;
            this.richResponse.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(149, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "Responses";
            // 
            // buttonRequestMakeFaireData
            // 
            this.buttonRequestMakeFaireData.Location = new System.Drawing.Point(12, 121);
            this.buttonRequestMakeFaireData.Name = "buttonRequestMakeFaireData";
            this.buttonRequestMakeFaireData.Size = new System.Drawing.Size(131, 41);
            this.buttonRequestMakeFaireData.TabIndex = 5;
            this.buttonRequestMakeFaireData.Text = "Request Aircraft MakerFaire Data";
            this.buttonRequestMakeFaireData.UseVisualStyleBackColor = true;
            this.buttonRequestMakeFaireData.Click += new System.EventHandler(this.buttonRequestMakeFaireData_Click);
            // 
            // cbComPort
            // 
            this.cbComPort.FormattingEnabled = true;
            this.cbComPort.Location = new System.Drawing.Point(438, 197);
            this.cbComPort.Name = "cbComPort";
            this.cbComPort.Size = new System.Drawing.Size(121, 21);
            this.cbComPort.TabIndex = 6;
            // 
            // labelCOMPort
            // 
            this.labelCOMPort.AccessibleRole = System.Windows.Forms.AccessibleRole.Pane;
            this.labelCOMPort.AutoSize = true;
            this.labelCOMPort.Location = new System.Drawing.Point(149, 197);
            this.labelCOMPort.Name = "labelCOMPort";
            this.labelCOMPort.Size = new System.Drawing.Size(86, 20);
            this.labelCOMPort.TabIndex = 7;
            this.labelCOMPort.Text = "COM Ports";
            // 
            // labelBaudRate
            // 
            this.labelBaudRate.AccessibleRole = System.Windows.Forms.AccessibleRole.Pane;
            this.labelBaudRate.AutoSize = true;
            this.labelBaudRate.Location = new System.Drawing.Point(149, 232);
            this.labelBaudRate.Name = "labelBaudRate";
            this.labelBaudRate.Size = new System.Drawing.Size(86, 20);
            this.labelBaudRate.TabIndex = 8;
            this.labelBaudRate.Text = "Baud Rate";
            // 
            // cbBaudRate
            // 
            this.cbBaudRate.FormattingEnabled = true;
            this.cbBaudRate.Location = new System.Drawing.Point(438, 234);
            this.cbBaudRate.Name = "cbBaudRate";
            this.cbBaudRate.Size = new System.Drawing.Size(121, 21);
            this.cbBaudRate.TabIndex = 9;
            // 
            // buttonSendSingleMessageBlock
            // 
            this.buttonSendSingleMessageBlock.Location = new System.Drawing.Point(149, 267);
            this.buttonSendSingleMessageBlock.Name = "buttonSendSingleMessageBlock";
            this.buttonSendSingleMessageBlock.Size = new System.Drawing.Size(410, 41);
            this.buttonSendSingleMessageBlock.TabIndex = 10;
            this.buttonSendSingleMessageBlock.Text = "Send single message block to fsmaster";
            this.buttonSendSingleMessageBlock.UseVisualStyleBackColor = true;
            this.buttonSendSingleMessageBlock.Click += new System.EventHandler(this.buttonSendSingleMessageBlock_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(571, 322);
            this.Controls.Add(this.buttonSendSingleMessageBlock);
            this.Controls.Add(this.cbBaudRate);
            this.Controls.Add(this.labelBaudRate);
            this.Controls.Add(this.labelCOMPort);
            this.Controls.Add(this.cbComPort);
            this.Controls.Add(this.buttonRequestMakeFaireData);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.richResponse);
            this.Controls.Add(this.buttonRequestSixPackData);
            this.Controls.Add(this.buttonDisconnect);
            this.Controls.Add(this.buttonConnect);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Form1";
            this.Text = "c172hc-fsconnect";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.Button buttonDisconnect;
        private System.Windows.Forms.Button buttonRequestSixPackData;
        private System.Windows.Forms.RichTextBox richResponse;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonRequestMakeFaireData;
        private System.Windows.Forms.ComboBox cbComPort;
        private System.Windows.Forms.Label labelCOMPort;
        private System.Windows.Forms.Label labelBaudRate;
        private System.Windows.Forms.ComboBox cbBaudRate;
        private System.Windows.Forms.Button buttonSendSingleMessageBlock;
    }
}

