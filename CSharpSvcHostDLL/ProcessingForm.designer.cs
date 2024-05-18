namespace CSharpSvcHostDLL
{
    partial class RunProcStdOutForm
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
            this.std = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.SubText = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.SelfText = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // std
            // 
            this.std.BackColor = System.Drawing.SystemColors.Window;
            this.std.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.std.Dock = System.Windows.Forms.DockStyle.Fill;
            this.std.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.std.ForeColor = System.Drawing.SystemColors.WindowText;
            this.std.Location = new System.Drawing.Point(0, 22);
            this.std.Multiline = true;
            this.std.Name = "std";
            this.std.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.std.Size = new System.Drawing.Size(712, 351);
            this.std.TabIndex = 0;
            this.std.WordWrap = false;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.SubText);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(712, 22);
            this.panel1.TabIndex = 1;
            // 
            // SubText
            // 
            this.SubText.AutoSize = true;
            this.SubText.Location = new System.Drawing.Point(3, 4);
            this.SubText.Name = "SubText";
            this.SubText.Size = new System.Drawing.Size(68, 13);
            this.SubText.TabIndex = 0;
            this.SubText.Text = "SUB TEST";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.SelfText);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 373);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(712, 22);
            this.panel2.TabIndex = 2;
            // 
            // SelfText
            // 
            this.SelfText.AutoSize = true;
            this.SelfText.Location = new System.Drawing.Point(3, 4);
            this.SelfText.Name = "SelfText";
            this.SelfText.Size = new System.Drawing.Size(13, 13);
            this.SelfText.TabIndex = 0;
            this.SelfText.Text = "--";
            // 
            // RunProcStdOutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(712, 395);
            this.Controls.Add(this.std);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RunProcStdOutForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ProcessingForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.RunProcStdOutForm_FormClosed);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox std;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label SubText;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label SelfText;
    }
}