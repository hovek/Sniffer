using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.IO;
using Gecko;
using System.Linq;

namespace Sniffer.Sniffs
{
    public delegate bool SniffCallback(object state = null);
    public delegate void LogEventHandler(object logObject);

    public abstract class Sniff : IDisposable
    {
        public SniffProcessCollection Sniffs = new SniffProcessCollection();
        private string currentSniff;
        public SniffProcess CurrentSniff
        {
            get
            {
                return Sniffs[currentSniff];
            }
        }
        public event EventHandler SniffingCompleted;

        private System.Windows.Forms.Timer navigationTimeoutTimer = new System.Windows.Forms.Timer();
        /// <summary>
        /// In millisecodns.
        /// </summary>
        public int NavigationTimeoutInterval
        {
            get
            {
                return navigationTimeoutTimer.Interval;
            }
            set
            {
                navigationTimeoutTimer.Interval = value;
            }
        }
        public bool ContinueProcessingOnNavigationTimeout { get; set; }

        public static event LogEventHandler OnLog;
        private bool _navigating = false;
        private bool navigating
        {
            get
            {
                return _navigating;
            }
            set
            {
                _navigating = value;
                if (_navigating)
                {
                    navigationTimeoutTimer.Start();
                }
                else
                {
                    navigationTimeoutTimer.Stop();
                }
            }
        }

        private bool _isRunning = false;
        public bool IsRunning
        {
            get
            {
                return _isRunning;
            }
            private set
            {
                bool prevVal = _isRunning;
                _isRunning = value;
                if (!_isRunning && prevVal)
                {
                    callSniffingCompleted();
                }
            }
        }

        public bool IsDisposed
        {
            get;
            private set;
        }

        public Sniff(HoveWebBrowser webBrowser)
        {
            WebBrowser = webBrowser;
            navigationTimeoutTimer.Tick += stopOnTimeoutTimer_Tick;
            navigationTimeoutTimer.Interval = 60000;
        }

        private void stopOnTimeoutTimer_Tick(object sender, EventArgs e)
        {
            if (ContinueProcessingOnNavigationTimeout)
            {
                WebBrowser.Stop();
            }
            else
            {
                Stop();
            }
        }

        private HoveWebBrowser webBrowser;
        public virtual HoveWebBrowser WebBrowser
        {
            get
            {
                return webBrowser;
            }
            set
            {
                if (webBrowser != null)
                {
                    webBrowser.DocumentCompleted -= webBrowser_DocumentCompleted;
                    webBrowser.OnNavigate -= webBrowser_OnNavigate;
                }
                webBrowser = value;
                if (webBrowser != null)
                {
                    webBrowser.DocumentCompleted += webBrowser_DocumentCompleted;
                    webBrowser.OnNavigate += webBrowser_OnNavigate;
                }
            }
        }

        void webBrowser_OnNavigate(object sender, EventArgs e)
        {
            navigating = true;
        }

        private void webBrowser_DocumentCompleted(object sender, EventArgs e)
        {
            navigating = false;
            if (WebBrowser != null && _isRunning)
            {
                process();
            }
        }

        private void process()
        {
            try
            {
                SniffProcess sp = Sniffs[currentSniff];
                if (sp.SniffCallback(sp.State))
                {
                    bool currentReached = false;
                    bool foundNext = false;
                    foreach (SniffProcess sniff in Sniffs)
                    {
                        if (currentReached)
                        {
                            foundNext = true;
                            currentSniff = sniff.Name;
                            break;
                        }
                        else if (sniff.Name == currentSniff)
                        {
                            currentReached = true;
                        }
                    }

                    if (!foundNext)
                    {
                        IsRunning = false;
                    }

                    if (IsRunning && !navigating)
                    {
                        process();
                    }
                }
            }
            catch (Exception ex)
            {
                Stop();
                throw ex;
            }
        }

        private void prepareForStart()
        {
            IsRunning = true;
            string scc = "";
            foreach (SniffProcess sc in Sniffs)
            {
                scc = sc.Name;
                break;
            }
            currentSniff = scc;
        }

        public virtual void Start()
        {
            prepareForStart();
            process();
        }

        protected virtual void Start(string url)
        {
            prepareForStart();
            WebBrowser.Navigate(url);
        }

        public void Stop()
        {
            _isRunning = false;
            if (webBrowser != null)
            {
                webBrowser.Stop();
            }
            _isRunning = true;
            IsRunning = false;
        }

        public virtual void Dispose()
        {
            Stop();
            Sniffs.Clear();
            SniffingCompleted = null;
            WebBrowser = null;
            IsDisposed = true;
        }

        public static void Log(object logObject)
        {
            if (OnLog != null)
            {
                OnLog(logObject);
            }
        }

        private void callSniffingCompleted()
        {
            if (SniffingCompleted != null)
            {
                SniffingCompleted(this, EventArgs.Empty);
            }
        }
    }

    public class SniffProcess
    {
        public string Name { get; set; }
        public SniffCallback SniffCallback { get; set; }
        public object State { get; set; }
    }

    public class SniffProcessCollection : List<SniffProcess>
    {
        public SniffProcess this[string name]
        {
            get
            {
                return this.Find(p => p.Name == name);
            }
            set
            {
                SniffProcess sp = this.Find(p => p.Name == name);
                int i = this.IndexOf(sp);
                if (i < 0)
                {
                    this.Add(value);
                }
                else
                {
                    this[i] = value;
                }
            }
        }

        public void Add(string name, SniffCallback callback, object state = null)
        {
            this.Add(new SniffProcess() { Name = name, SniffCallback = callback, State = state });
        }

        public void InsertAfter(string nameIndex, string name, SniffCallback callback, object state = null)
        {
            int index = this.FindIndex(s => s.Name == nameIndex) + 1;
            this.Insert(index, new SniffProcess() { Name = name, SniffCallback = callback, State = state });
        }

        public SniffCallback GetCallbackByName(string name)
        {
            return this[name].SniffCallback;
        }
    }
}