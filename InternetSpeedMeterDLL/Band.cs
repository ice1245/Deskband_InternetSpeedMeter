using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace InternetSpeedMeter
{
    [ComVisible(true)]
    [Guid("FB17B6DA-E3D7-4D17-9E43-3416983372A9")]
    [CSDeskBand.CSDeskBandRegistration(Name = "InternetSpeedMeter", ShowDeskBand = true)]

    public class Band : CSDeskBand.CSDeskBandWin
    {
        private static Control _control;
        public Band()
        {
            Options.MinHorizontalSize = new Size(90, 30);
            _control = new Band_UserControl(this);
        }
        protected override Control Control => _control;
    }
}
