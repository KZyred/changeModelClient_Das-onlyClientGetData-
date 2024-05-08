
namespace changeModelClient_Das
{
    partial class mainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(mainForm));
            this.textBoxLog = new System.Windows.Forms.TextBox();
            this.btn_reset = new System.Windows.Forms.Button();
            this.tmReadNewLog = new System.Windows.Forms.Timer(this.components);
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.staConnectSIO = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lb_lineName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBoxLog
            // 
            this.textBoxLog.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.textBoxLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxLog.Location = new System.Drawing.Point(2, 30);
            this.textBoxLog.Multiline = true;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ReadOnly = true;
            this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxLog.Size = new System.Drawing.Size(341, 202);
            this.textBoxLog.TabIndex = 0;
            this.textBoxLog.TextChanged += new System.EventHandler(this.textBoxLog_TextChanged);
            // 
            // btn_reset
            // 
            this.btn_reset.Location = new System.Drawing.Point(2, 3);
            this.btn_reset.Name = "btn_reset";
            this.btn_reset.Size = new System.Drawing.Size(123, 21);
            this.btn_reset.TabIndex = 1;
            this.btn_reset.Text = "reset";
            this.btn_reset.UseVisualStyleBackColor = true;
            this.btn_reset.Click += new System.EventHandler(this.btn_reset_Click);
            // 
            // tmReadNewLog
            // 
            this.tmReadNewLog.Interval = 1000;
            this.tmReadNewLog.Tick += new System.EventHandler(this.tmReadNewLog_Tick);
            // 
            // notifyIcon
            // 
            this.notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon.BalloonTipText = "hide";
            this.notifyIcon.BalloonTipTitle = "Model Change Project (DAS)";
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "KZ";
            this.notifyIcon.DoubleClick += new System.EventHandler(this.notifyIcon_DoubleClick);
            // 
            // staConnectSIO
            // 
            this.staConnectSIO.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.staConnectSIO.ForeColor = System.Drawing.Color.Transparent;
            this.staConnectSIO.Location = new System.Drawing.Point(184, 1);
            this.staConnectSIO.Name = "staConnectSIO";
            this.staConnectSIO.Size = new System.Drawing.Size(10, 23);
            this.staConnectSIO.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(131, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Socket :";
            // 
            // lb_lineName
            // 
            this.lb_lineName.AutoSize = true;
            this.lb_lineName.Location = new System.Drawing.Point(200, 7);
            this.lb_lineName.Name = "lb_lineName";
            this.lb_lineName.Size = new System.Drawing.Size(11, 13);
            this.lb_lineName.TabIndex = 10;
            this.lb_lineName.Text = "*";
            // 
            // mainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(345, 238);
            this.Controls.Add(this.lb_lineName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.staConnectSIO);
            this.Controls.Add(this.btn_reset);
            this.Controls.Add(this.textBoxLog);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "mainForm";
            this.Text = "Model Change Project (DAS)";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.mainForm_FormClosing);
            this.Load += new System.EventHandler(this.mainForm_Load);
            this.Resize += new System.EventHandler(this.mainForm_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxLog;
        private System.Windows.Forms.Button btn_reset;
        private System.Windows.Forms.Timer tmReadNewLog;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.Label staConnectSIO;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lb_lineName;
    }
}

