using DevExpress.LookAndFeel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WeavingGenerator
{
    internal static class Program
    {
        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //DevExpress.LookAndFeel.UserLookAndFeel.Default.SkinName = "DevExpress Dark Style";
            //DevExpress.LookAndFeel.UserLookAndFeel.Default.SkinName = SkinStyle.Basic;
            DevExpress.LookAndFeel.UserLookAndFeel.Default.SkinName = SkinStyle.VisualStudio2013Blue;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
