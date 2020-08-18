namespace OcularisDragonfruitPlugin.WinFormNet
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.btnExport = new System.Windows.Forms.Button();
            this.axAuga = new AxHeimdallLib.AxAuga();
            this.pnlCamera = new System.Windows.Forms.Panel();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.axAuga)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlCamera
            // 
            this.pnlCamera.Location = new System.Drawing.Point(612, 12);
            this.pnlCamera.Name = "pnlCamera";
            this.pnlCamera.Size = new System.Drawing.Size(400, 320);
            this.pnlCamera.TabIndex = 0;
            // 
            // txtIP
            // 
            this.txtIP.Location = new System.Drawing.Point(12, 358);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(125, 27);
            this.txtIP.TabIndex = 2;
            this.txtIP.Text = "IP";
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(164, 358);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(125, 27);
            this.txtUserName.TabIndex = 2;
            this.txtUserName.Text = "User Name";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(312, 358);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(125, 27);
            this.txtPassword.TabIndex = 2;
            this.txtPassword.Text = "Password";
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(23, 399);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 23);
            this.btnExport.TabIndex = 1;
            this.btnExport.Text = "Export";
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            this.btnExport.UseVisualStyleBackColor = true;
            // 
            // axAuga
            // 
            this.axAuga.Enabled = true;
            this.axAuga.Location = new System.Drawing.Point(12, 12);
            this.axAuga.Name = "axAuga";
            this.axAuga.Size = new System.Drawing.Size(400, 337);
            this.axAuga.TabIndex = 14;
            this.axAuga.TriggerSelected += new AxHeimdallLib._IAugaEvents_TriggerSelectedEventHandler(this.axAuga_TriggerSelected);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 1000);
            this.Controls.Add(this.axAuga);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtUserName);
            this.Controls.Add(this.txtIP);
            this.Controls.Add(this.pnlCamera);
            this.Controls.Add(this.btnExport);
            this.Name = "MainForm";
            this.Text = "Ocularis plugin";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.axAuga)).EndInit();

        }

        #endregion

        private System.Windows.Forms.Button btnExport;
        private AxHeimdallLib.AxAuga axAuga;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Panel pnlCamera;
    }
}

