using System;
using System.Windows.Forms;
using System.Threading;
using Gecko;
using System.Collections.Generic;
using Sniffer.Common;
using Polenter.Serialization;
using Sniffer.Sniffs;
using System.Runtime.InteropServices;
using System.Text;
using System.Reflection;

namespace Sniffer
{
    public partial class MainForm : Form
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        static extern bool DestroyWindow(IntPtr hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        private HoveWebBrowser browser;
        private Settings.Settings settings;
        private volatile bool inSettings = false;
        private System.Windows.Forms.Timer tmrCheckAlert = new System.Windows.Forms.Timer();

        Sniff selectedSniffer;
        bool _stopped = true;
        bool stopped
        {
            get
            {
                return _stopped;
            }
            set
            {
                _stopped = value;
                panel1.Enabled = _stopped;
            }
        }

        public MainForm()
        {
            InitializeComponent();

            Xpcom.Initialize(Properties.Settings.Default.XulRunnerPath);
            browser = new HoveWebBrowser();
            pnlWebBrowser.Controls.Add(browser);
            browser.Dock = DockStyle.Fill;

            tmrCheckAlert.Interval = 1000;
            tmrCheckAlert.Tick += tmrCheckAlert_Tick;
            tmrCheckAlert.Enabled = true;

            List<KeyValuePair<SniffEnum, string>> sniffs = new List<KeyValuePair<SniffEnum, string>>();
            foreach (SniffEnum pe in Enum.GetValues(typeof(SniffEnum)))
            {
                sniffs.Add(new KeyValuePair<SniffEnum, string>(pe, EnumStringValue.GetEnumStringValue(pe)));
            }
            cbPage.ValueMember = "Key";
            cbPage.DisplayMember = "Value";
            cbPage.DataSource = sniffs;

            Sniff.OnLog += selectedSniffer_OnLog;
        }

        private void tmrCheckAlert_Tick(object sender, EventArgs e)
        {
            IntPtr alertWindow = FindWindow("MozillaDialogClass", "Alert");
            if (alertWindow != IntPtr.Zero)
            {
                DestroyWindow(alertWindow);
                selectedSniffer_OnLog("Alert");
            }
        }

        public void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            selectedSniffer_OnLog(e.Exception);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cbPage.SelectedIndex = 0;
            loadSettings();
        }

        private void stop()
        {
            stopped = true;
            if (selectedSniffer != null)
            {
                selectedSniffer.Dispose();
                selectedSniffer = null;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!stopped)
            {
                stop();
                btnStart.Text = "Start";
            }
            else
            {
                btnStart.Text = "Stop";

                SniffEnum sniffersEnum = (SniffEnum)cbPage.SelectedValue;
                selectedSniffer = (Sniff)ReflectionHelper.CreateNewInstance(SniffMappings.SniffsMappings[sniffersEnum].Sniffer, browser);
                PropertyInfo pi = SniffMappings.SniffsMappings[sniffersEnum].Sniffer.GetProperty("Settings");
                if (pi != null)
                {
                    pi.SetValue(selectedSniffer, settings.SnifferSettings[sniffersEnum], null);
                }

                stopped = false;
                selectedSniffer.SniffingCompleted += selectedSniffer_SniffingCompleted;
                selectedSniffer.Start();
            }
        }

        private void selectedSniffer_SniffingCompleted(object sender, EventArgs e)
        {
            if (!stopped)
            {
                new Thread((tbWaitInMinutes) =>
                {
                    DateTime date = DateTime.Now.AddMinutes((double)tbWaitInMinutes);
                    while (!stopped && DateTime.Now < date)
                    {
                        System.Threading.Thread.Sleep(1);
                    }
                    if (!stopped)
                    {
                        this.Invoke(Delegate.CreateDelegate(typeof(EventHandler), this, "restartSelectedSniffer"), sender, e);
                    }
                }).Start(Convert.ToDouble(tbRepeatDelay.Text));
            }
        }

        private void restartSelectedSniffer(object sender, EventArgs e)
        {
            selectedSniffer.Start();
        }

        private void selectedSniffer_OnLog(object logObject)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(Delegate.CreateDelegate(typeof(LogEventHandler), this, "selectedSniffer_OnLog"), logObject);
                return;
            }

            string log = "";
            if (logObject is Exception)
            {
                Exception ex = Common.Common.GetMostInnerException((Exception)logObject);
                log = ex.GetType().ToString() + "; " + ex.Message + "\r\n" + ex.StackTrace.ToString();
            }
            else
            {
                log = logObject.ToString();
            }

            txtLog.Text = DateTime.Now.ToString("dd.MM.yy HH:mm:ss") + ": " + log + "\r\n\r\n" + txtLog.Text;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            stop();
            saveSettings();
        }

        private void cbPage_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Text = EnumStringValue.GetEnumStringValue<SniffEnum>((SniffEnum)cbPage.SelectedValue);
            btnSettings.Enabled = SniffMappings.SniffsMappings != null && SniffMappings.SniffsMappings[(SniffEnum)cbPage.SelectedValue].SnifferSettingsForm != null;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (inSettings)
            {
                return;
            }
            inSettings = true;

            SniffEnum sniffersEnum = (SniffEnum)cbPage.SelectedValue;
            object snifferSettingsForm = ReflectionHelper.CreateNewInstance(SniffMappings.SniffsMappings[sniffersEnum].SnifferSettingsForm);

            if (!settings.SnifferSettings.ContainsKey(sniffersEnum))
            {
                SniffMappings snifferMapping = SniffMappings.SniffsMappings[sniffersEnum];
                Settings.SniffSettings snifferSettings = (Settings.SniffSettings)ReflectionHelper.CreateNewInstance(snifferMapping.SnifferSettings);
                settings.SnifferSettings[sniffersEnum] = snifferSettings;
            }

            new Thread(() =>
            {
                DialogResult result = (DialogResult)SniffMappings.SniffsMappings[sniffersEnum].SnifferSettingsForm.GetMethod("Open", new Type[] { typeof(SniffEnum), typeof(Settings.SniffSettings) }).Invoke(snifferSettingsForm, new object[] { sniffersEnum, settings.SnifferSettings[sniffersEnum] });
                inSettings = false;
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    this.Invoke(Delegate.CreateDelegate(typeof(EventHandler), this, "saveSettingsInvoke"));
                }
            }).Start();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtLog.Text = "";
        }

        private void loadSettings()
        {
            try
            {
                SharpSerializer serializer = new SharpSerializer();
                settings = (Settings.Settings)serializer.Deserialize("settings.xml");
            }
            catch { }

            if (settings == null)
            {
                settings = createNewSettings();
            }

            cbPage.SelectedValue = settings.ProgramSettings.SelectedSniffer;
            tbRepeatDelay.Text = settings.ProgramSettings.RepeatDelay.ToString();
        }

        private void saveSettingsInvoke(object sender, EventArgs e)
        {
            saveSettings();
        }

        private void saveSettings()
        {
            settings.ProgramSettings.SelectedSniffer = (SniffEnum)cbPage.SelectedValue;
            settings.ProgramSettings.RepeatDelay = decimal.Parse(tbRepeatDelay.Text);

            SharpSerializer serializer = new SharpSerializer();
            serializer.Serialize(settings, "settings.xml");
        }

        private Settings.Settings createNewSettings()
        {
            Settings.Settings newSettings = new Settings.Settings();

            foreach (SniffEnum snifferEnum in Enum.GetValues(typeof(SniffEnum)))
            {
                SniffMappings snifferMapping = SniffMappings.SniffsMappings[snifferEnum];
                if (snifferMapping.SnifferSettings != null)
                {
                    Settings.SniffSettings snifferSettings = (Settings.SniffSettings)ReflectionHelper.CreateNewInstance(snifferMapping.SnifferSettings);
                    newSettings.SnifferSettings[snifferEnum] = snifferSettings;
                }
            }

            return newSettings;
        }
    }
}
