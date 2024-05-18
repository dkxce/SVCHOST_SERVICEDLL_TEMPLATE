//
// C# 
// svchost.exe Service DLL Example
// v 0.1, 18.05.2024
// https://github.com/dkxce
// en,ru,1251,utf-8
//

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSharpSvcHostDLL
{
    public partial class RunProcStdOutForm : Form
    {
        private delegate void DoText(string text);
        private DateTime Started = DateTime.Now;
        private string _stdText = "";


        #region CONSTRUCTOR
        public RunProcStdOutForm(string caption)
        {
            InitializeComponent();
            this.Text = caption;
            this.SubText.Text = "";
            this.DialogResult = DialogResult.Cancel;
            timer1_Tick(this, null);
        }

        public RunProcStdOutForm(string caption, string subText)
        {
            InitializeComponent();
            this.Text = caption;
            this.SubText.Text = subText;
            this.DialogResult = DialogResult.Cancel;
            timer1_Tick(this, null);
        }
        #endregion CONSTRUCTOR

        #region PUBLIC
        public void SetText(string text)
        {
            std.Text = text;
            std.SelectionStart = std.Text.Length;
        }

        public void Write(string text)
        {
            std.Text += text;
            std.SelectionStart = std.Text.Length;
        }

        public void WriteLine(string line = "")
        {
            std.Text += line + "\r\n";
            std.SelectionStart = std.Text.Length;
            std.ScrollToCaret();
        }        

        public string StdText { get { return _stdText; } }

        public DialogResult StartProcAndShowDialogWhileRunning(System.Diagnostics.ProcessStartInfo psi)
        {
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.CreateNoWindow = true;
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = psi;
            proc.OutputDataReceived += new System.Diagnostics.DataReceivedEventHandler(StdOutputDataReceived);
            proc.Start();
            proc.BeginOutputReadLine();
            (new Task(() => {
                while (true)
                {
                    if (proc.HasExited)
                    {
                        try
                        {
                            string txt = proc.StandardOutput.ReadToEnd();
                            if (!string.IsNullOrEmpty(txt) && !string.IsNullOrEmpty(txt.Trim())) _stdText += txt;
                        }
                        catch { };
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    };
                    System.Threading.Thread.Sleep(100);
                };
            })).Start();
            return this.ShowDialog();            
        }

        public string StartProcAndShow(System.Diagnostics.ProcessStartInfo psi)
        {            
            _stdText = "";
            WriteLine($"---- Start process `{psi.FileName} {psi.Arguments}` ----");
            Show();
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.CreateNoWindow = true;
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = psi;
            proc.OutputDataReceived += new System.Diagnostics.DataReceivedEventHandler(StdOutputDataReceived);
            proc.Start();
            proc.BeginOutputReadLine();
            System.Windows.Forms.Timer t = new System.Windows.Forms.Timer() { Interval = 100 };
            t.Tick += (object sender, EventArgs e) =>
            {
                if (proc.HasExited)
                {
                    try
                    {
                        string txt = proc.StandardOutput.ReadToEnd();
                        if (!string.IsNullOrEmpty(txt) && !string.IsNullOrEmpty(txt.Trim())) _stdText += txt;
                    }
                    catch {};                    
                };
            };
            while (!proc.HasExited)
            {
                Application.DoEvents();
                Thread.Sleep(100);
            };
            t.Dispose();
            Application.DoEvents();
            WriteLine($"---- Process Exited with Code: {proc.ExitCode} ----");
            return _stdText;
        }        

        public void WaitWhileShowed()
        {
            timer1.Enabled = false;
            SelfText.Text += ". DONE";
            while (this.Visible) { Application.DoEvents(); Thread.Sleep(100); };
        }
        #endregion PUBLIC

        #region PRIVATE
        public bool StatusVisible { get { return panel2.Visible; } set { panel2.Visible = value; } }

        private void StdOutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            try
            {
                string data = e.Data.Trim(new char[] { '\r', '\n', ' ' });
                _stdText += data;
                if (!string.IsNullOrEmpty(data))
                    this.Invoke(new DoText(WriteLine), new object[] { data });
            }
            catch { };
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                SelfText.Text = String.Format("Started: {0}, Elapsed: {1}", Started.ToString("HH:mm:ss dd.MM.yyyy"), DateTime.Now.Subtract(Started).ToString().Substring(0, 8));
            }
            catch { };
        }

        private void RunProcStdOutForm_FormClosed(object sender, FormClosedEventArgs e) { try { Dispose(); } catch { }; }

        #endregion  PRIVATE
    }
}