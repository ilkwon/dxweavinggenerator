using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WeavingGenerator
{
    public partial class DialogUpload : XtraForm
    {
        MainForm mainForm;
        ProjectData data;
        string viewerUrl;
        public DialogUpload(MainForm mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
        }

        private void DialogUpload_Load(object sender, EventArgs e)
        {
            ///////////////////////////////////////////////////////////////////
            //
            ///////////////////////////////////////////////////////////////////
            data = mainForm.GetProjectData();
            if (data == null)
            {
                XtraMessageBox.Show("프로젝트 생성 후 이용해주세요..");
                return;
            }

            ///////////////////////////////////////////////////////////////////
            //
            ///////////////////////////////////////////////////////////////////
            string appid = mainForm.GetAPPID();
            string projectid = data.ProjectID;

            viewerUrl = MainForm.VIEWER_URL + "?APPID=" + appid + "&PROJECTID=" + projectid;
            textEdit_Path.Text = viewerUrl;
            //System.Diagnostics.Process.Start(url);


            ///////////////////////////////////////////////////////////////////
            //
            ///////////////////////////////////////////////////////////////////
            progressBarControl1.Properties.ShowTitle = true;
            progressBarControl1.Properties.PercentView = true;
            //최소값
            progressBarControl1.Properties.Minimum = 0;
            //최대값
            progressBarControl1.Properties.Maximum = 100;
            //단계
            progressBarControl1.Properties.Step = 20;



        }

        private void simpleButton_Ok_Click(object sender, EventArgs e)
        {

            //설정값(값에 따라 진행도 강제 설정)
            //progressBarControl1.EditValue = 0;
            //for(int i = 0; i < 5; i++)
            //{
            //    //Step에 따라 단계가 진행
            //    progressBarControl1.PerformStep();
            //    progressBarControl1.Update();
            //}


            
            string appid = mainForm.GetAPPID();
            string projectid = data.ProjectID;

            string diff = mainForm.GetDiffFilePath();
            string norm = mainForm.GetNormFilePath();
            string metal = mainForm.GetMetalFrom3DViewer();
            string drape = mainForm.GetDrapeFrom3DViewer();
            string obj = mainForm.GetObjectFrom3DViewer();
            
            try
            {
                // 스케일 초기화
                mainForm.ResetViewScale();

                ///////////////////////////////////////////////////////////
                ///
                ///////////////////////////////////////////////////////////
                progressBarControl1.PerformStep();
                progressBarControl1.Update();

                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("APPID", appid);
                parameters.Add("PROJECTID", projectid);
                parameters.Add("METAL", metal);
                parameters.Add("DRAPE", drape);
                parameters.Add("OBJECT", obj);
                
                //Trace.WriteLine("metal : " + metal + ", drape : " + drape);

                string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
                string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";

                byte[] boundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
                byte[] endboundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                byte[] buffer = new byte[4096];

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(MainForm.UPLOAD_URL);
                if (MainForm.UPLOAD_URL.StartsWith("https"))
                {
                    request.Proxy = null;
                    request.Credentials = CredentialCache.DefaultCredentials;
                    ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback(ValidateServerCertificate);
                }
                request.ServicePoint.Expect100Continue = false;
                request.Method = "POST";
                request.ContentType = "multipart/form-data; boundary=" + boundary;


                ///////////////////////////////////////////////////////////
                ///
                ///////////////////////////////////////////////////////////
                progressBarControl1.PerformStep();
                progressBarControl1.Update();


                using (Stream requestStream = request.GetRequestStream())
                {

                    ///////////////////////////////////////////////////////////
                    /// 파라미터
                    ///////////////////////////////////////////////////////////
                    foreach (KeyValuePair<string, string> param in parameters)
                    {
                        requestStream.Write(boundarybytes, 0, boundarybytes.Length);
                        string formItem = string.Format(formdataTemplate, param.Key, param.Value);
                        byte[] formItemByte = System.Text.Encoding.UTF8.GetBytes(formItem);
                        requestStream.Write(formItemByte, 0, formItemByte.Length);
                    }


                    string header;
                    byte[] headerByte;
                    int bytesRead = 0;
                    ///////////////////////////////////////////////////////////
                    /// Diff 파일
                    ///////////////////////////////////////////////////////////
                    requestStream.Write(boundarybytes, 0, boundarybytes.Length);
                    //octet-stream 으로 변경 처리 진행 
                    //string header = string.Format(headerTemplate, "outfaxfile", Path.GetFileName(filePath), "multipart/form-data");
                    header = string.Format(headerTemplate, "FILE_DIFF", Path.GetFileName(diff), "octet-stream");
                    headerByte = System.Text.Encoding.UTF8.GetBytes(header);
                    requestStream.Write(headerByte, 0, headerByte.Length);

                    bytesRead = 0;
                    using (FileStream fileStream = new FileStream(diff, FileMode.Open, FileAccess.Read))
                    {
                        while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            requestStream.Write(buffer, 0, bytesRead);
                        }
                    }

                    ///////////////////////////////////////////////////////////
                    ///
                    ///////////////////////////////////////////////////////////
                    progressBarControl1.PerformStep();
                    progressBarControl1.Update();


                    ///////////////////////////////////////////////////////////
                    /// Norm 파일
                    ///////////////////////////////////////////////////////////
                    requestStream.Write(boundarybytes, 0, boundarybytes.Length);
                    //octet-stream 으로 변경 처리 진행 
                    //string header = string.Format(headerTemplate, "outfaxfile", Path.GetFileName(filePath), "multipart/form-data");
                    header = string.Format(headerTemplate, "FILE_NORM", Path.GetFileName(norm), "octet-stream");
                    headerByte = System.Text.Encoding.UTF8.GetBytes(header);
                    requestStream.Write(headerByte, 0, headerByte.Length);

                    bytesRead = 0;
                    using (FileStream fileStream = new FileStream(norm, FileMode.Open, FileAccess.Read))
                    {
                        while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            requestStream.Write(buffer, 0, bytesRead);
                        }
                    }



                    requestStream.Write(endboundarybytes, 0, endboundarybytes.Length);

                    ///////////////////////////////////////////////////////////
                    ///
                    ///////////////////////////////////////////////////////////
                    progressBarControl1.PerformStep();
                    progressBarControl1.Update();



                    ///////////////////////////////////////////////////////////
                    /// 응답 데이터
                    ///////////////////////////////////////////////////////////
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        string resdata;
                        using (StreamReader sr = new StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8, true))
                        {
                            resdata = sr.ReadToEnd();
                            //Trace.WriteLine("resdata : " + resdata);
                        }
                    }


                    ///////////////////////////////////////////////////////////
                    ///
                    ///////////////////////////////////////////////////////////
                    progressBarControl1.PerformStep();
                    progressBarControl1.Update();
                }

            }
            catch (Exception ex) { 
                Console.WriteLine("\nResponse Exception :\n{0}", ex.Message);
                XtraMessageBox.Show("파일 업로드에 오류가 발생했습니다. \r\n" + ex.Message);

                progressBarControl1.EditValue = 0;
            }

            XtraMessageBox.Show("업로드를 완료 했습니다.");
            progressBarControl1.EditValue = 0;
        }
        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        private void simpleButton_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void simpleButton_Find_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(new ProcessStartInfo { FileName = @viewerUrl, UseShellExecute = true });
        }
    }
}
