namespace PrnView
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this._btnRefresh = new System.Windows.Forms.Button();
            this._txtInfo = new System.Windows.Forms.TextBox();
            this._btnOpenPrn = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this._pic = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pic)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this._btnRefresh);
            this.splitContainer1.Panel2.Controls.Add(this._txtInfo);
            this.splitContainer1.Panel2.Controls.Add(this._btnOpenPrn);
            this.splitContainer1.Size = new System.Drawing.Size(800, 450);
            this.splitContainer1.SplitterDistance = 592;
            this.splitContainer1.TabIndex = 0;
            // 
            // _btnRefresh
            // 
            this._btnRefresh.Location = new System.Drawing.Point(82, 3);
            this._btnRefresh.Name = "_btnRefresh";
            this._btnRefresh.Size = new System.Drawing.Size(73, 34);
            this._btnRefresh.TabIndex = 2;
            this._btnRefresh.Text = "Refresh";
            this._btnRefresh.UseVisualStyleBackColor = true;
            // 
            // _txtInfo
            // 
            this._txtInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._txtInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._txtInfo.Location = new System.Drawing.Point(3, 43);
            this._txtInfo.Multiline = true;
            this._txtInfo.Name = "_txtInfo";
            this._txtInfo.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this._txtInfo.Size = new System.Drawing.Size(198, 302);
            this._txtInfo.TabIndex = 1;
            // 
            // _btnOpenPrn
            // 
            this._btnOpenPrn.Location = new System.Drawing.Point(3, 3);
            this._btnOpenPrn.Name = "_btnOpenPrn";
            this._btnOpenPrn.Size = new System.Drawing.Size(73, 34);
            this._btnOpenPrn.TabIndex = 0;
            this._btnOpenPrn.Text = "Open Prn";
            this._btnOpenPrn.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this._pic);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(592, 450);
            this.panel1.TabIndex = 1;
            // 
            // _pic
            // 
            this._pic.Location = new System.Drawing.Point(0, 0);
            this._pic.Name = "_pic";
            this._pic.Size = new System.Drawing.Size(461, 342);
            this._pic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this._pic.TabIndex = 1;
            this._pic.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.splitContainer1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pic)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button _btnOpenPrn;
        private System.Windows.Forms.TextBox _txtInfo;
        private System.Windows.Forms.Button _btnRefresh;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox _pic;
    }
}

