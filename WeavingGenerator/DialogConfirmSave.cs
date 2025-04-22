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
    public partial class DialogConfirmSave : DevExpress.XtraEditors.XtraForm
    {

        //델리게이트
        public delegate void DataPassEventHandler(string data);
        public event DataPassEventHandler DataPassEvent;


        public DialogConfirmSave()
        {
            InitializeComponent();
        }

        private void btnYes_Click(object sender, EventArgs e)
        {
            DataPassEvent("Y");
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            DataPassEvent("N");
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
