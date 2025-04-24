using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

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
            this.progressBarControl1.Properties.ShowTitle = true;
            this.progressBarControl1.Properties.PercentView = true;
            //최소값
            this.progressBarControl1.Properties.Minimum = 0;
            //최대값
            this.progressBarControl1.Properties.Maximum = 100;
            //단계
            this.progressBarControl1.Properties.Step = 20;
        }

        //---------------------------------------------------------------------
        private void simpleButton_Ok_Click(object sender, EventArgs e)
        {
            string appid = mainForm.GetAPPID();
            string projectid = data.ProjectID;

            string diff = mainForm.GetDiffFilePath();
            string norm = mainForm.GetNormFilePath();
            string metal = mainForm.GetMetalFrom3DViewer();
            string drape = mainForm.GetDrapeFrom3DViewer();
            string obj = mainForm.GetObjectFrom3DViewer();
            //Trace.WriteLine("metal : " + metal + ", drape : " + drape);

            var parameters = new Dictionary<string, string>();
            parameters.Add("APPID", appid);
            parameters.Add("PROJECTID", projectid);
            parameters.Add("METAL", metal);
            parameters.Add("DRAPE", drape);
            parameters.Add("OBJECT", obj);

            _ = UploadFileAsync(diff, norm, parameters);
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// .net core 5.0은 지원이 끝난관계로 8.0 버젼 업그레이드. by ilkwon 2025.04.24
        /// NET 5 이후부터 Microsoft는 WebRequest.Create()와 관련된 전체 API를 Obsolete
        /// WebRequest -> HttpClient 변경.
        /// </summary>
        /// <param name="diff"></param>
        /// <param name="norm"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private async Task UploadFileAsync(string diff, string norm
            , Dictionary<string, string> parameters)
        {
            try
            {
                // 스케일 초기화
                this.mainForm.ResetViewScale();
                UpdateProgress();
#if DEBUG
                // 인증서 검증 임시 내부 테스트용.(추후 반드시 수정)
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
                };
#endif

                // .net 8.0부터 기존의 HttpWebRequest
                // 폐기. http 헤더는 자동으로 세팅해주므로 헤더부분 필요 없음.
                using var client = new HttpClient(handler);                
                using var form = new MultipartFormDataContent();

                ///////////////////////////////////////////////////////////
                /// 파라미터
                ///////////////////////////////////////////////////////////
                foreach (var param in parameters)
                {
                    form.Add(new StringContent(param.Value), param.Key);
                }
                UpdateProgress();

                ///////////////////////////////////////////////////////////
                /// Diff 파일
                ///////////////////////////////////////////////////////////
                var diffBytes = await File.ReadAllBytesAsync(diff);
                form.Add(new ByteArrayContent(diffBytes), "FILE_DIFF", Path.GetFileName(diff));                                    
                UpdateProgress();

                ///////////////////////////////////////////////////////////
                /// Norm 파일
                ///////////////////////////////////////////////////////////
                var normBytes = await File.ReadAllBytesAsync(norm);
                form.Add(new ByteArrayContent(normBytes), "FILE_NORM", Path.GetFileName(norm));
                UpdateProgress();

                ///////////////////////////////////////////////////////////
                /// 응답 데이터
                ///////////////////////////////////////////////////////////
                var response = await client.PostAsync(MainForm.UPLOAD_URL, form);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();
                UpdateProgress();

                XtraMessageBox.Show("업로드를 완료 했습니다.");
                this.progressBarControl1.EditValue = 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nResponse Exception :\n{0}", ex.Message);
                XtraMessageBox.Show("파일 업로드에 오류가 발생했습니다. \r\n" + ex.Message);

                this.progressBarControl1.EditValue = 0;
            }
        }
        //---------------------------------------------------------------------
        private void UpdateProgress()
        {
            if (this.progressBarControl1.InvokeRequired)
            {
                this.progressBarControl1.Invoke(new Action(() =>  {
                    this.progressBarControl1.PerformStep();
                    this.progressBarControl1.Update(); 

                }));
            } else {
                this.progressBarControl1.PerformStep();
                this.progressBarControl1.Update();
            }
        }
        //---------------------------------------------------------------------
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
