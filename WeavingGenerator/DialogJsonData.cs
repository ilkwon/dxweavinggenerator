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
    public partial class DialogJsonData : Form
    {
        MainForm mainForm;
        public DialogJsonData(MainForm mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
        }

        public void SetJsonData(string json)
        {
            textBox1.Text = json;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mainForm.SetJsonData(textBox1.Text);
            this.Close();
        }
    }
}
