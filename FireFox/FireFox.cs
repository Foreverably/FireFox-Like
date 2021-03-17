using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EasyTabs;

namespace FireFox
{
    public partial class FireFox : TitleBarTabs
    {
        public FireFox()
        {
            InitializeComponent();
            AeroPeekEnabled = false;
            TabRenderer = new ChromeTabRenderer(this);
        }

        private void FireFox_Load(object sender, EventArgs e)
        {

        }
        public override TitleBarTab CreateTab()
        {
            return new TitleBarTab(this)
            {
                Content = new Form1 { Text = "New Tab" }
            };
        }
    }
}
