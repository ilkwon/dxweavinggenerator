using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace WeavingGenerator
{
    public partial class XtraReportPrint : DevExpress.XtraReports.UI.XtraReport
    {
        bool isVisibleWarpOfPrint = true;

        public XtraReportPrint(bool isVisibleWarpOfPrint)
        {
            InitializeComponent();
            this.isVisibleWarpOfPrint = isVisibleWarpOfPrint;

            if (isVisibleWarpOfPrint == false)
            {
                DetailReport.Visible = false;
                DetailReport1.Visible = false;
            }
            xrPictureBox1.UseImageResolution = false;
        }

        private void XtraReportPrint_DesignerLoaded(object sender, DevExpress.XtraReports.UserDesigner.DesignerLoadedEventArgs e)
        {
            
        }
    }
}
