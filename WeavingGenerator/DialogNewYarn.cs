using DevExpress.Utils.MVVM;
using DevExpress.XtraEditors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WeavingGenerator
{

    public delegate void DialogNewYarnEventHandler(object sender, int newIdx);

    public partial class DialogNewYarn : DevExpress.XtraEditors.XtraForm
    {
        public DialogNewYarnEventHandler dialogNewYarnEventHandler = null;

        MainForm mainForm;

        public DialogNewYarn(MainForm main)
        {
            InitializeComponent();
            mainForm = main;
        }


        private void DialogNewYarn_Load(object sender, EventArgs e)
        {

            comboBoxEdit_Textured.Properties.Items.AddRange(new object[] {
                "Filament",
                "DTY",
                "Hi-multi",
                "Twist",
                "ATY",
                "ITY",
                "ITY_S",
                "ITY_Z",
                "NEP"
            });
            comboBoxEdit_Textured.SelectedIndex = 0;
            comboBoxEdit_Textured.SelectedIndexChanged += ComboBoxEdit_Textured_SelectedIndexChanged;
            comboBoxEdit_Textured.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;

            comboBoxEdit_Type.Properties.Items.AddRange(new object[] {
                "장섬유",
                "단섬유"
            });
            comboBoxEdit_Type.SelectedIndex = 0;
            comboBoxEdit_Type.SelectedIndexChanged += ComboBoxEdit_Type_SelectedIndexChanged;
            comboBoxEdit_Type.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;

            comboBoxEdit_Unit.Properties.Items.AddRange(new object[] {
                "Denier",
                "Dtex",
                "Ne",
                "Nm",
                "Lea"
            });
            comboBoxEdit_Unit.SelectedIndex = 0;
            comboBoxEdit_Unit.SelectedIndexChanged += ComboBoxEdit_Unit_SelectedIndexChanged;
            comboBoxEdit_Unit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;

            pictureEdit_Image.Image = Util.GetYarnResource("FILAMENT");

            //TrackBarControl1_ValueChanged
            //trackBarControl_Metal += trackBarControl_Metal_EditValueChanged;
        }



        ///////////////////////////////////////////////////////////////////////
        /// 시작 - 이벤트
        ///////////////////////////////////////////////////////////////////////

        private void ComboBoxEdit_Textured_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxEdit_Textured.SelectedIndex < 0)
            {
                return;
            }
            ComboBoxEdit cb = (ComboBoxEdit)sender;
            if (cb.SelectedIndex > -1)
            {
                string v = cb.SelectedItem.ToString();
                //.WriteLine("Textured_SelectedIndexChanged : " + v);

                pictureEdit_Image.Image = Util.GetYarnResource(v);
            }
        }

        private void ComboBoxEdit_Unit_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxEdit_Unit.SelectedIndex < 0)
            {
                return;
            }

            ComboBoxEdit cb = (ComboBoxEdit)sender;
            if (cb.SelectedIndex > -1)
            {
                string v = cb.SelectedItem.ToString();
                if (string.Equals(v, "Denier") || string.Equals(v, "Dtex"))
                {
                    //장섬유 
                    comboBoxEdit_Type.SelectedIndex = 0;
                }
                else if (string.Equals(v, "Ne") || string.Equals(v, "Nm") || string.Equals(v, "Lea"))
                {
                    //단섬유
                    comboBoxEdit_Type.SelectedIndex = 1;
                }

            }
        }

        private void ComboBoxEdit_Type_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxEdit_Type.SelectedIndex < 0)
            {
                return;
            }
            ComboBoxEdit cb = (ComboBoxEdit)sender;
            if (cb.SelectedIndex > -1)
            {
                string v = cb.SelectedItem.ToString();
                if (string.Equals(v, "장섬유"))
                {
                    //장섬유 
                    comboBoxEdit_Textured.Properties.Items.Clear();
                    comboBoxEdit_Textured.Properties.Items.AddRange(new object[] {
                        "Filament",
                        "DTY",
                        "Hi-multi",
                        "Twist",
                        "ATY",
                        "ITY",
                        "ITY_S",
                        "ITY_Z",
                        "NEP"
                    });
                    comboBoxEdit_Textured.SelectedIndex = 0;
                }
                else if (string.Equals(v, "단섬유"))
                {
                    //단섬유
                    comboBoxEdit_Textured.Properties.Items.Clear();
                    comboBoxEdit_Textured.Properties.Items.AddRange(new object[] {
                        "2Ply",
                        "NEP",
                        "Slub"
                    });
                    comboBoxEdit_Textured.SelectedIndex = 0;
                }
            }
        }

        private void button_save_Click(object sender, EventArgs e)
        {

            if (comboBoxEdit_Unit.SelectedIndex < 0)
            {
                XtraMessageBox.Show("굵기의 단위를 선택해주세요.");
                comboBoxEdit_Unit.Focus();
                return;
            }
            if (comboBoxEdit_Type.SelectedIndex < 0)
            {
                XtraMessageBox.Show("종류를 선택해주세요.");
                comboBoxEdit_Type.Focus();
                return;
            }
            if (comboBoxEdit_Textured.SelectedIndex < 0)
            {
                XtraMessageBox.Show("가연여부를 선택해주세요");
                comboBoxEdit_Textured.Focus();
                return;
            }
            string name = textEdit_Name.Text;
            string weight = textEdit_Weight.Text;
            string unit = comboBoxEdit_Unit.SelectedItem.ToString();
            string type = comboBoxEdit_Type.SelectedItem.ToString();
            string textured = comboBoxEdit_Textured.SelectedItem.ToString();
            string metal = trackBarControl_Metal.Value.ToString();
            string image = "yarn02";

            if (string.IsNullOrEmpty(name))
            {
                XtraMessageBox.Show("이름을 입력해주세요.");
                textEdit_Name.Focus();
                return;
            }
            if (string.IsNullOrEmpty(weight))
            {
                XtraMessageBox.Show("굵기를 입력해주세요.");
                textEdit_Weight.Focus();
                return;
            }
            if (MainForm.isNumber(weight) == false)
            {
                XtraMessageBox.Show("숫자를 입력해주세요");
                textEdit_Weight.Focus();
                return;
            }
            if (string.IsNullOrEmpty(unit))
            {
                XtraMessageBox.Show("굵기의 단위를 선택해주세요.");
                comboBoxEdit_Unit.Focus();
                return;
            }
            if (string.IsNullOrEmpty(type))
            {
                XtraMessageBox.Show("종류를 선택해주세요.");
                comboBoxEdit_Type.Focus();
                return;
            }
            if (string.IsNullOrEmpty(textured))
            {
                XtraMessageBox.Show("가연여부를 선택해주세요");
                comboBoxEdit_Textured.Focus();
                return;
            }

            Yarn yarn = new Yarn();
            yarn.Name = name;
            yarn.Weight = weight;
            yarn.Unit = unit;
            yarn.Type = type;
            yarn.Textured = textured;
            yarn.Image = image;
            yarn.Metal = metal;

            int idx = mainForm.SaveDAOYarn(yarn);
            if (idx < 0)
            {
                XtraMessageBox.Show("오류가 발생했습니다.");
                return;
            }

            // 이벤트
            if (dialogNewYarnEventHandler != null)
            {
                dialogNewYarnEventHandler(this, idx);
            }

            this.Close();
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void simpleButton_Save_Click(object sender, EventArgs e)
        {

            if (comboBoxEdit_Unit.SelectedIndex < 0)
            {
                XtraMessageBox.Show("굵기의 단위를 선택해주세요.");
                comboBoxEdit_Unit.Focus();
                return;
            }
            if (comboBoxEdit_Type.SelectedIndex < 0)
            {
                XtraMessageBox.Show("종류를 선택해주세요.");
                comboBoxEdit_Type.Focus();
                return;
            }
            if (comboBoxEdit_Textured.SelectedIndex < 0)
            {
                XtraMessageBox.Show("가연여부를 선택해주세요");
                comboBoxEdit_Textured.Focus();
                return;
            }
            string name = textEdit_Name.Text;
            string weight = textEdit_Weight.Text;
            string unit = comboBoxEdit_Unit.SelectedItem.ToString();
            string type = comboBoxEdit_Type.SelectedItem.ToString();
            string textured = comboBoxEdit_Textured.SelectedItem.ToString();
            string metal = trackBarControl_Metal.Value.ToString();
            string image = "yarn02";

            if (string.IsNullOrEmpty(name))
            {
                XtraMessageBox.Show("이름을 입력해주세요.");
                textEdit_Name.Focus();
                return;
            }
            if (string.IsNullOrEmpty(weight))
            {
                XtraMessageBox.Show("굵기를 입력해주세요.");
                textEdit_Weight.Focus();
                return;
            }
            if (MainForm.isNumber(weight) == false)
            {
                XtraMessageBox.Show("숫자를 입력해주세요");
                textEdit_Weight.Focus();
                return;
            }
            if (string.IsNullOrEmpty(unit))
            {
                XtraMessageBox.Show("굵기의 단위를 선택해주세요.");
                comboBoxEdit_Unit.Focus();
                return;
            }
            if (string.IsNullOrEmpty(type))
            {
                XtraMessageBox.Show("종류를 선택해주세요.");
                comboBoxEdit_Type.Focus();
                return;
            }
            if (string.IsNullOrEmpty(textured))
            {
                XtraMessageBox.Show("가연여부를 선택해주세요");
                comboBoxEdit_Textured.Focus();
                return;
            }

            Yarn yarn = new Yarn();
            yarn.Name = name;
            yarn.Weight = weight;
            yarn.Unit = unit;
            yarn.Type = type;
            yarn.Textured = textured;
            yarn.Image = image;
            yarn.Metal = metal;

            int idx = mainForm.SaveDAOYarn(yarn);
            if (idx < 0)
            {
                XtraMessageBox.Show("오류가 발생했습니다.");
                return;
            }

            // 이벤트
            if (dialogNewYarnEventHandler != null)
            {
                dialogNewYarnEventHandler(this, idx);
            }

            this.Close();
        }

        private void simpleButton_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void trackBarControl_Metal_EditValueChanged(object sender, EventArgs e)
        {

        }


        ///////////////////////////////////////////////////////////////////////
        /// 시작 - 끝
        ///////////////////////////////////////////////////////////////////////

    }
}
