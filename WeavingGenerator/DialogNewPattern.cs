using DevExpress.XtraEditors;
using DevExpress.XtraWaitForm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WeavingGenerator
{
    public partial class DialogNewPattern : XtraForm
    {
        MainForm mainForm;

        public DialogNewPattern(MainForm mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
        }

    }
}
