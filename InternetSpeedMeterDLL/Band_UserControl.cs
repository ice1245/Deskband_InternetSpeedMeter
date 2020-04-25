using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Management;
using System.Drawing;
using System.Threading;
using System.ComponentModel;

namespace InternetSpeedMeter
{
    public partial class Band_UserControl : UserControl
    {
        Program p = new Program();
        string[] suffixes =
            { "B/s", "KB/s", "MB/s", "GB/s", "TB/s", "PB/s" };
        public Band_UserControl(CSDeskBand.CSDeskBandWin w)
        {
            InitializeComponent();
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            p.init();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = 1000;
            timer.Start();
        }



        private void timer_Tick(object sender, EventArgs e)
        {

            label4.Text = GetSpeedInString(p.uploadBytesPerSecond);
            label3.Text = GetSpeedInString(p.downloadBytesPerSecond);
        }
        private  string GetSpeedInString(double bytes)
        {
            int counter = 0;
            decimal number = (decimal)bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number = number / 1024;
                counter++;
            }
            return string.Format("{0:n1}{1}", number, suffixes[counter]);
        }
    }


}
