using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp.WinForms;
using CefSharp;
using EasyTabs;
using System.Net;
using System.IO;
using FireFox.Properties;
using System.Text.RegularExpressions;

namespace FireFox
{
    public partial class Form1 : Form
    {
        private class NewTabLifespanHandler : ILifeSpanHandler
        {
            private Form1 _tab;

            public NewTabLifespanHandler(Form1 tab)
            {
                _tab = tab;
            }

            public bool DoClose(IWebBrowser chromiumWebBrowser, IBrowser browser)
            {
                return true;
            }

            public void OnAfterCreated(IWebBrowser chromiumWebBrowser, IBrowser browser)
            {
            }

            public void OnBeforeClose(IWebBrowser chromiumWebBrowser, IBrowser browser)
            {
            }

            public bool OnBeforePopup(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser)
            {
                Form1 newTab = null;

                _tab.ParentTabs.Invoke(new Action(() =>
                {
                    _tab.ParentTabs.AddNewTab();
                    newTab = _tab.ParentTabs.SelectedTab.Content as Form1;

                    bool newTabLoaded = false;

                    if (!newTab.WebBrowser.IsBrowserInitialized)
                    {
                        newTab.WebBrowser.IsBrowserInitializedChanged += (_, __) =>
                        {
                            newTab.WebBrowser.LoadingStateChanged += (___, e) =>
                            {
                                if (!newTabLoaded)
                                {
                                    if (!e.IsLoading)
                                    {
                                        newTabLoaded = true;
                                        newTab.WebBrowser.Load(targetUrl);
                                    }
                                }
                            };
                        };
                    }

                    else
                    {
                        newTab.WebBrowser.LoadingStateChanged += (_, e) =>
                        {
                            if (!newTabLoaded)
                            {
                                if (!e.IsLoading)
                                {
                                    newTabLoaded = true;
                                    newTab.WebBrowser.Load(targetUrl);
                                }
                            }
                        };
                    }
                }));

                newBrowser = null;
                return true;
            }

            private void WebBrowser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
            {
                throw new NotImplementedException();
            }
        }

        protected TitleBarTabs ParentTabs
        {
            get
            {
                return (ParentForm as TitleBarTabs);
            }
        }
        public Form1()
        {
            InitializeComponent();

            

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            WebBrowser = new ChromiumWebBrowser("about:blank")
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Location = new Point(0, 30),
                MinimumSize = new Size(20, 20),
                Name = "webBrowser",
                Size = new Size(1280, 713),
                TabIndex = 6,
                LifeSpanHandler = new NewTabLifespanHandler(this)
                
            };

            Controls.Add(WebBrowser);

            WebBrowser.TitleChanged += WebBrowser_TitleChanged;
            WebBrowser.AddressChanged += WebBrowser_AddressChanged;
            WebBrowser.LoadingStateChanged += webBrowser_DocumentCompleted;
        }

        private void webBrowser_DocumentCompleted(object sender, LoadingStateChangedEventArgs e)
        {
            if (siticoneTextBox1.Text == "about:blank")
            {
                Invoke(new Action(() => Icon = Resources.DefaultIcon));
            }
        }

        private void WebBrowser_AddressChanged(object sender, AddressChangedEventArgs e)
        {
            Invoke(new Action(() => siticoneTextBox1.Text = e.Address));

            if (e.Address != "about.blank" && !faviconLoaded)
            {
                Uri uri = new Uri(e.Address);

                if (uri.Scheme == "http" || uri.Scheme == "https")
                {
                    try
                    {
                        HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uri.Scheme + "://" + uri.Host + "/favicon.ico");
                        webRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.83 Safari/537.36";
                        webRequest.KeepAlive = false;
                        webRequest.AllowAutoRedirect = true;

                        WebResponse response = webRequest.GetResponse();
                        Stream stream = response.GetResponseStream();

                        if (stream != null)
                        {
                            byte[] buffer = new byte[1024];

                            using (MemoryStream ms = new MemoryStream())
                            {
                                int read;

                                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                                    ms.Write(buffer, 0, read);

                                ms.Seek(0, SeekOrigin.Begin);

                                Invoke(new Action(() =>
                                {
                                    Icon = new Icon(ms);

                                    ParentTabs.UpdateThumbnailPreviewIcon(ParentTabs.Tabs.Single(t => t.Content == this));
                                    ParentTabs.RedrawTabs();
                                }));
                            }
                        }
                    }

                    catch
                    {
                        Invoke(new Action(() => Icon = Resources.DefaultIcon));
                    }
                }

                Invoke(new Action(() => Parent.Refresh()));
                faviconLoaded = true;
            }
        }

        private void WebBrowser_TitleChanged(object sender, TitleChangedEventArgs e)
        {
            Invoke(new Action(() => Text = e.Title));
        }

        public readonly ChromiumWebBrowser WebBrowser;
        private bool faviconLoaded = true;
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void siticoneTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string fullUrl = siticoneTextBox1.Text;

                if (!Regex.IsMatch(fullUrl, "^[a-zA-Z0-9]+\\://"))
                    fullUrl = "http://" + fullUrl;

                faviconLoaded = true;
                WebBrowser.Load(fullUrl);
            }
        }

        private void siticoneButton1_Click(object sender, EventArgs e)
        {
            WebBrowser.Back();
        }

        private void siticoneButton2_Click(object sender, EventArgs e)
        {
            WebBrowser.Forward();
        }

        private void siticoneButton4_Click(object sender, EventArgs e)
        {
            WebBrowser.Reload();
        }

        private void siticoneTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            siticonePanel2.Visible = true;
        }

        private void newTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ParentTabs.AddNewTab();
        }

        private void newWindowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
           
            
        }

        private void siticoneButton8_Click(object sender, EventArgs e)
        {
            siticonePanel2.Visible = false;
        }

        private void siticoneButton3_Click(object sender, EventArgs e)
        {
            contextMenuStrip1.Show(siticoneButton3, new System.Drawing.Point(-60, 20));
        }
    }
}
