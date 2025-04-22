using DevExpress.Utils;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;


namespace WeavingGenerator
{

    public partial class WeaveViewer : Panel//PictureBox
    {

        static int OFFSET_X = 40;
        static int OFFSET_Y = 40;

        int boxXCount = 10;
        int boxYCount = 10;

        int boxWidth = 100;
        int boxHeight = 100;

        //double warpWidth = 0.9;
        //double weftWidth = 0.9;



        // 프로젝트 번호
        int idx;
        // 프로젝트 데이터
        ProjectData pData;
        // 위경사 데이터
        List<List<BoxData>> listWeaveData = new List<List<BoxData>>();

        bool IsDrawYarn = false;
        bool IsNormalMap = true;
        //bool IsInchBar = true;

        //2025-02-05 soonchol
        String YarnDyeColor = null;

        Color BG_COLOR = Color.White;

        PictureBox PBOX = null;

        Bitmap canvas = null;

        MainForm mainForm;
        Dictionary<string, Bitmap> yarnMap = new Dictionary<string, Bitmap>();
        Dictionary<string, Bitmap> yarnShinyMap = new Dictionary<string, Bitmap>();
        List<Yarn> yarnList = new List<Yarn>();


        // 1인치당 픽셀을 기준으로 조직을 그림 (PIXEL_PER_INCH)
        public static int PPI = 600;
        //int PPI = 6000;

        // density 밀도 (사용자가 설정하는 경/위사 밀도)
        int densityX = 30;
        int densityY = 30;

        int DEN_MAX_WARP = 15000;
        int DEN_MAX_WEFT = 10000;

        // 확대
        int viewScale = 1;

        public WeaveViewer(MainForm mainForm)
        {
            this.mainForm = mainForm;
            //this.PBOX = this;

            BG_COLOR = Util.ToColor("585858");
            DoubleBuffered = true;
        }

        public WeaveViewer(MainForm mainForm, PictureBox box)
        {
            this.mainForm = mainForm;
            this.PBOX = box;

            BG_COLOR = Util.ToColor("585858");

        }

        private void RemoveAllBoxData()
        {
            for (int i = listWeaveData.Count - 1; i > -1; i--)
            {
                List<BoxData> rows = listWeaveData[i];
                for (int j = rows.Count - 1; j > -1; j--)
                {
                    BoxData box = rows[j];
                    rows.RemoveAt(j);
                }
                rows.Clear();
                rows = null;
            }
            listWeaveData.Clear();
        }
        public void ResetViewer()
        {
            idx = -1;
            pData = null;
            RemoveAllBoxData();
            RepaintCanvas();
        }
        public void SetProjectData(int _idx, ProjectData _wData)
        {
            this.idx = _idx;
            this.pData = _wData;
            InitPatternData2();
        }
        public void SetYarnImage(bool b)
        {
            IsDrawYarn = b;
            IsNormalMap = b;
        }
        //2025-02-05 soonchol
        public void SetYarnDyeColor(String DyeColor)
        {
            YarnDyeColor = DyeColor;
        }
        public void SetViewScale(int i)
        {
            SetViewScale(i, true);
        }
        public void SetViewScale(int i, bool isRepaint)
        {
            this.viewScale = i;
            if (isRepaint == true)
            {
                RepaintCanvas();
            }
        }
        public int GetViewScale()
        {
            return this.viewScale;
        }



        private Bitmap GetYarnBitmap(int idxYarn)
        {
            Bitmap bm = (Properties.Resources.DEFAULT_2);
            for (int i = 0; i < yarnList.Count; i++)
            {
                Yarn y = yarnList[i];
                if (y.Idx == idxYarn)
                {
                    /*
                     "Filament",
                    "DTY",
                    "Hi-multi",
                    "Twist",
                    "ATY",
                    "ITY",
                    "NEP"
                     */
                    string textured = y.Textured;
                    if (!string.IsNullOrEmpty(textured))
                    {
                        if (textured == "Filament")
                        {
                            bm = (Properties.Resources.FILAMENT_2);
                        }
                        else if (textured == "DTY")
                        {
                            bm = (Properties.Resources.DTY_2);
                        }
                        else if (textured == "ATY")
                        {
                            bm = (Properties.Resources.ATY_2);
                        }
                        else if (textured == "ITY")
                        {
                            bm = (Properties.Resources.ITY_2);
                        }
                        else if (textured == "ITY_S")
                        {
                            bm = (Properties.Resources.ITY_S);
                        }
                        else if (textured == "ITY_Z")
                        {
                            bm = (Properties.Resources.ITY_Z);
                            //bm = (Properties.Resources.test_yarn);
                        }
                    }
                }
            }
            return bm;
        }
        // Denier 로 변환
        private int GetYarnWeight(int idxYarn)
        {
            int weight = 250;
            for (int i = 0; i < yarnList.Count; i++)
            {
                Yarn y = yarnList[i];
                if (y.Idx == idxYarn)
                {
                    weight = Util.ToDenier(y.Weight, y.Unit);
                    break;
                }
            }
            return weight;
        }
        private int GetYarnShiny(int idxYarn)
        {
            int shiny = 0;
            for (int i = 0; i < yarnList.Count; i++)
            {
                Yarn y = yarnList[i];
                if (y.Idx == idxYarn)
                {
                    shiny = Util.ToInt(y.Metal, 0);
                    break;
                }
            }
            return shiny;
        }
        private Color GetYarnShinyColor(int shiny)
        {
            Color c = Color.White;
            switch (shiny)
            {
                case 1:
                    c = Util.ToColor("A3A3A3");
                    break;
                case 2:
                    c = Util.ToColor("535353");
                    break;
                case 3:
                    c = Util.ToColor("030303");
                    break;
                default:
                    c = Color.White;
                    break;

            }
            return c;
        }

        private string CreateYarnBitmap(string xy, int idxYarn, string HexColor)
        {
            string key = xy + "_" + idxYarn + "_" + HexColor;
            if (yarnMap.ContainsKey(key) == false)
            {
                Bitmap yarn = GetYarnBitmap(idxYarn);
                Rectangle rect = new Rectangle(0, 0, yarn.Width, yarn.Height);
                Bitmap temp = new Bitmap(yarn.Width, yarn.Height);

                Graphics g2 = Graphics.FromImage(temp);

                // 예외 처리 ) 사용자가 black 을 선택 한 경우 
                if (HexColor == "000000")
                {
                    HexColor = "030303";
                }
                Color color = Util.ToColor(HexColor);
                g2.FillRectangle(new SolidBrush(color), rect);
                // 검은색을 투명 처리
                g2.DrawImage(yarn, 0, 0, yarn.Width, yarn.Height);
                temp.MakeTransparent(Color.Black);

                // 경사
                if (xy == "X")
                {
                    temp.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    Bitmap warpImage = new Bitmap(temp);
                    yarnMap[key] = warpImage;
                }
                // 위사
                else
                {
                    Bitmap weftImage = new Bitmap(temp);
                    yarnMap[key] = weftImage;
                }
                temp.Dispose();
                yarn.Dispose();
            }
            return key;
        }


        private string CreateYarnShinyBitmap(string xy, int idxYarn, int shiny)
        {
            string key = xy + "_" + idxYarn + "_" + shiny;
            if (yarnShinyMap.ContainsKey(key) == false)
            {
                Bitmap yarn = GetYarnBitmap(idxYarn);
                Rectangle rect = new Rectangle(0, 0, yarn.Width, yarn.Height);
                Bitmap temp = new Bitmap(yarn.Width, yarn.Height);

                Graphics g2 = Graphics.FromImage(temp);

                Color color = GetYarnShinyColor(shiny);
                g2.FillRectangle(new SolidBrush(color), rect);
                // 검은색을 투명 처리
                g2.DrawImage(yarn, new Point(0, 0));
                temp.MakeTransparent(Color.Black);

                // 경사
                if (xy == "X")
                {
                    temp.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    Bitmap warpImage = new Bitmap(temp);
                    yarnShinyMap[key] = warpImage;
                }
                // 위사
                else
                {
                    Bitmap weftImage = new Bitmap(temp);
                    yarnShinyMap[key] = weftImage;
                }
                temp.Dispose();
                yarn.Dispose();
            }
            return key;
        }

        private void InitPatternData2()
        {
            if (pData == null)
            {
                Trace.WriteLine("InitPatternData Project Data IS NULL.....");
                return;
            }
            if (pData.Warp == null || pData.Weft == null)
            {
                Trace.WriteLine("InitPatternData Warp Data IS NULL.....");
                return;
            }
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            ///////////////////////////////////////////////////////////////////
            /// 리소스 원사 마이크로 이미지 저장공간 초기화
            ///////////////////////////////////////////////////////////////////
            var keys = yarnMap.Keys.ToList();
            for (int index = keys.Count - 1; index > -1; index--)
            {
                string key = keys[index];
                Bitmap bm = yarnMap[key];
                bm.Dispose();
                bm = null;
                yarnMap.Remove(key);
            }
            yarnMap.Clear();



            ///////////////////////////////////////////////////////////////////
            /// 원사 정보 조회
            ///////////////////////////////////////////////////////////////////
            for (int index = yarnList.Count - 1; index > -1; index--)
            {
                Yarn yarn = yarnList[index];
                yarn = null;
            }
            if (yarnList != null)
            {
                yarnList.Clear();
            }
            yarnList = mainForm.ListDAOYarn();



            ///////////////////////////////////////////////////////////////////
            // 경위사 밀도 Density(인치당 원사 개수)
            ///////////////////////////////////////////////////////////////////
            // 밀도
            densityX = pData.Warp.Density;
            densityY = pData.Weft.Density;
            if (densityX <= 0) densityX = 300;
            if (densityY <= 0) densityY = 300;

            ///////////////////////////////////////////////////////////////////
            boxWidth = PPI / densityX;
            boxHeight = PPI / densityY;
            ///////////////////////////////////////////////////////////////////


            ///////////////////////////////////////////////////////////////////
            // ver 1
            ///////////////////////////////////////////////////////////////////
            boxXCount = 0;
            boxYCount = 0;
            int[] warpArr = pData.Warp.GetWArrayInt();
            int[] weftArr = pData.Weft.GetWArrayInt();

            if (warpArr == null || warpArr.Length <= 0)
            {
                Trace.WriteLine("InitPatternData Project Data, warpArr IS NULL.....");
                return;
            }
            if (weftArr == null || weftArr.Length <= 0)
            {
                Trace.WriteLine("InitPatternData Project Data, weftArr IS NULL.....");
                return;
            }
            boxXCount = warpArr.Length;
            boxYCount = weftArr.Length;



            ///////////////////////////////////////////////////////////////////
            // 시작 - BoxData 자료 생성
            ///////////////////////////////////////////////////////////////////
            List<BoxData> col;

            RemoveAllBoxData();

            Pattern patt = pData.Pattern;
            int Col = patt.XLength;
            int Row = patt.YLength;

            boxXCount = boxXCount - 1;
            boxYCount = boxYCount - 1;

            boxXCount = ((boxXCount / Col) + 1) * Col;
            boxYCount = ((boxYCount / Row) + 1) * Row;

            int i = 0;
            int j = 0;

            int a = 0;
            int b = 0;

            for (int y = 0; y < boxYCount; y++)
            {
                if (i >= Row) i = 0;
                if (a >= weftArr.Length) a = 0;

                int idx2 = weftArr[a];

                WInfo weftInfo = pData.Weft.GetWInfo(idx2);
                if (weftInfo == null) continue;
                int idxYarnWeft = weftInfo.IdxYarn;

                int weftWeight = GetYarnWeight(idxYarnWeft);
                //string keyWeft = "Y_" + idxYarnWeft + "_" + weftInfo.HexColor;

                //2025-02-05 soonchol
                string keyWeft;
                Color weftColor;

                if (YarnDyeColor == null)
                {
                    keyWeft = CreateYarnBitmap("Y", idxYarnWeft, weftInfo.HexColor);
                    weftColor = weftInfo.GetColor();
                }
                else
                {
                    keyWeft = CreateYarnBitmap("Y", idxYarnWeft, ProjectData.GetHexDyeColor(ProjectData.GetDyeColor(YarnDyeColor)));
                    weftColor = ProjectData.GetDyeColor(YarnDyeColor);
                }

                int weftShiny = GetYarnShiny(idxYarnWeft);
                string shinyWeft = CreateYarnShinyBitmap("Y", idxYarnWeft, weftShiny);
                Color weftShinyColor = GetYarnShinyColor(weftShiny);

                col = new List<BoxData>();
                for (int x = 0; x < boxXCount; x++)
                {
                    if (j >= Col) j = 0;
                    if (b >= warpArr.Length) b = 0;
                    int Data = patt.Data[i, j];

                    int idx1 = warpArr[b];
                    WInfo warpInfo = pData.Warp.GetWInfo(idx1);
                    if (warpInfo == null) continue;

                    int idxYarnWarp = warpInfo.IdxYarn;
                    int warpWeight = GetYarnWeight(idxYarnWarp);

                    //2025-02-05 soonchol
                    string keyWarp;
                    Color warpColor;

                    if (YarnDyeColor == null)
                    {
                        keyWarp = CreateYarnBitmap("X", idxYarnWarp, warpInfo.HexColor);
                        warpColor = warpInfo.GetColor();
                    }
                    else
                    {
                        keyWarp = CreateYarnBitmap("X", idxYarnWarp, ProjectData.GetHexDyeColor(ProjectData.GetDyeColor(YarnDyeColor)));
                        warpColor = ProjectData.GetDyeColor(YarnDyeColor);
                    }

                    int warpShiny = GetYarnShiny(idxYarnWarp);
                    string shinyWarp = CreateYarnShinyBitmap("X", idxYarnWarp, warpShiny);
                    Color warpShinyColor = GetYarnShinyColor(warpShiny);


                    ///////////////////////////////////////////////////////////
                    BoxData box;
                    box.Data = Data;
                    box.warpColor = warpColor;
                    box.weftColor = weftColor;
                    box.warpKey = keyWarp;
                    box.weftKey = keyWeft;
                    box.warpWeight = warpWeight;
                    box.weftWeight = weftWeight;

                    box.warpShinyColor = warpShinyColor;
                    box.weftShinyColor = weftShinyColor;
                    box.warpShinyKey = shinyWarp;
                    box.weftShinyKey = shinyWeft;

                    col.Add(box);
                    ///////////////////////////////////////////////////////////

                    j++;
                    b++;
                }
                listWeaveData.Add(col);

                i++;
                j = 0;

                a++;
                b = 0;
            }
            ///////////////////////////////////////////////////////////////////
            // 끝 - BoxData 자료 생성
            ///////////////////////////////////////////////////////////////////

            stopwatch.Stop();
            Trace.WriteLine("Init Weaving Data Elapsed Time is : " + stopwatch.ElapsedMilliseconds);

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // 1. 인치 바 그리기
            DrawInchBar(g);

            if (canvas == null)
            {
                return;
            }
            // 2. 직물 비트맵 그리기
            //g.DrawImage(canvas, OFFSET_X, OFFSET_X);

            //2025-01-20 soonchol
            int target_x = canvas.Width * viewScale;
            int target_y = canvas.Height * viewScale;

            g.DrawImage(canvas, new Rectangle(OFFSET_X, OFFSET_Y, OFFSET_X + target_x, OFFSET_Y + target_y),
                                new Rectangle(0, 0, canvas.Width, canvas.Height), GraphicsUnit.Pixel);
            //2025-01-20 soonchol
            //g.DrawImage(canvas, OFFSET_X + canvas.Width, OFFSET_X);
            //g.DrawImage(canvas, OFFSET_X, OFFSET_X + canvas.Height);
            //g.DrawImage(canvas, OFFSET_X + canvas.Width, OFFSET_X + canvas.Height);
        }

        public void Scroll()
        {
            //this.Location = new Point(0, 0);
            //2025-01-20 soonchol
            //((PanelControl)this.Parent).AutoScrollPosition  = new Point(0, 0);
        }
        public void RepaintCanvas()
        {
            if (boxXCount <= 0 || boxYCount <= 0)
            {
                return;
            }
            if (canvas != null)
            {
                canvas.Dispose();
                canvas = null;
            }

            //2025-01-20 soonchol
            //((PanelControl)this.Parent).AutoScrollPosition = new Point(0, 0);
            ///////////////////////////////////////////////////////////////////
            // 1. 직물 비트맵 생성
            ///////////////////////////////////////////////////////////////////

            // 박스의 폭 =  ppi(기준 pixel) / 경사 밀도
            boxWidth = PPI / densityX;
            boxHeight = PPI / densityY;

            // canvas의 총 크기
            int canvasTempWidth = boxWidth * boxXCount;
            int canvasTempHeight = boxHeight * boxYCount;

            Bitmap canvasTemp = new Bitmap(canvasTempWidth, canvasTempHeight);

            Graphics g = Graphics.FromImage(canvasTemp);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.HighSpeed;
            g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
            g.CompositingQuality = CompositingQuality.HighSpeed;

            g.FillRectangle(new SolidBrush(Color.Black), new Rectangle(0, 0, canvasTempWidth, canvasTempHeight));

            //2025-01-17 soonchol
            int xrepeat = this.Width / canvasTempWidth / viewScale + 1;
            int yrepeat = this.Height / canvasTempHeight / viewScale + 1;

            // 스케일이 적용된 canvas
            int canvasWidth = xrepeat * canvasTempWidth;
            int canvasHeight = yrepeat * canvasTempHeight;

            // 스케일이 적용된 인치상수
            int sPPI = PPI * viewScale;

            // 인치 단위로 크기 설정
            int sizeWidth = ((canvasWidth / sPPI) + 1) * sPPI;
            int sizeHeight = ((canvasHeight / sPPI) + 1) * sPPI;
            // 최소 2인치로 고정(인치 단위로 크기 설정하려 했으나 그냥 최소 2인치 고정)
            if (sizeWidth < (2 * sPPI))
            {
                sizeWidth = 2 * sPPI;
            }
            if (sizeHeight < (2 * sPPI))
            {
                sizeHeight = 2 * sPPI;
            }


            // offset 추가
            sizeWidth = sizeWidth + OFFSET_X;
            sizeHeight = sizeHeight + OFFSET_Y;

            // 인치를 그리기 위해 플러스알파 만큼 더 추가
            sizeWidth = sizeWidth + 5;
            sizeHeight = sizeHeight + 5;

            //this.Size = new Size(sizeWidth, sizeHeight);
            this.Size = new Size(2 * PPI * viewScale, 2 * PPI * viewScale);

            ///////////////////////////////////////////////////////////////////
            // 러프니스
            ///////////////////////////////////////////////////////////////////
            Bitmap canvasRoughTemp = new Bitmap(canvasTempWidth, canvasTempHeight);

            Graphics gRough = Graphics.FromImage(canvasRoughTemp);

            gRough.FillRectangle(new SolidBrush(Color.Black), new Rectangle(0, 0, canvasTempWidth, canvasTempHeight));
            ///////////////////////////////////////////////////////////////////
            //
            ///////////////////////////////////////////////////////////////////


            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            if (IsDrawYarn == true)
            {
                ///////////////////////////////////////////////////////////////
                // 원사 마이크로 이미지
                ///////////////////////////////////////////////////////////////
                DrawWeavingYarn(g, gRough, 0, 0);
            }
            else
            {
                ///////////////////////////////////////////////////////////////
                // 박스형식 그리기
                ///////////////////////////////////////////////////////////////
                DrawWeaving(g, gRough, 0, 0);
            }

            stopwatch.Stop();
            long timeCreateCanvas = stopwatch.ElapsedMilliseconds;

            canvasTemp.MakeTransparent(Color.Black);

            g.Dispose();

            canvasRoughTemp.MakeTransparent(Color.Black);

            gRough.Dispose();


            ///////////////////////////////////////////////////////////////////
            // 2. 이미지 파일 생성(3D 뷰어에 사용됨)
            // - 비트맵 이미지 생성
            // - 노멀맵 이미지 생성
            ///////////////////////////////////////////////////////////////////
            stopwatch.Start();
            Generate3DImage(canvasTemp);
            Generate3DImageRough(canvasRoughTemp);
            stopwatch.Stop();
            long timeImageFile = stopwatch.ElapsedMilliseconds;
            //


            ///////////////////////////////////////////////////////////////////
            // 3. 직물 비트맵 스케일 적용
            ///////////////////////////////////////////////////////////////////
            stopwatch.Start();

            //2025-01-17 soonchol
            xrepeat = this.Width / canvasTempWidth / viewScale + 1;
            yrepeat = this.Height / canvasTempHeight / viewScale + 1;

            //2025-01-20 soonchol
            //Bitmap canvas2 = new Bitmap(xrepeat * canvasTempWidth, yrepeat * canvasTempHeight);
            Bitmap canvas2 = new Bitmap(sizeWidth, sizeHeight);
            //Bitmap canvas2 = new Bitmap(10000, 10000);
            Graphics g2 = Graphics.FromImage(canvas2);
            //g2.FillRectangle(new SolidBrush(System.Drawing.Color.Black), new Rectangle(0, 0, 10000, 10000));

            for (int i = 0; i < xrepeat; i++)
            {
                for (int j = 0; j < yrepeat; j++)
                {
                    //g.DrawImage(canvas, OFFSET_X + (canvasTempWidth - 0) * viewScale * i, OFFSET_X + (canvasTempHeight - 0) * viewScale * j);
                    g2.DrawImage(canvasTemp, (canvasTempWidth - 0) * 1 * i, (canvasTempHeight - 0) * 1 * j);
                }
            }

            //Bitmap canvasTiled = ResizeBitmap(canvas2, viewScale * 100);
            //canvas = ResizeBitmap(canvasTemp, (viewScale * 100), true);
            //canvas = ResizeBitmap(canvas2, (viewScale * 100), true);
            canvas = ResizeBitmap(canvas2, (1 * 100), true);
            canvasTemp.Dispose();

            //canvasTiled.Dispose();
            g2.Dispose();
            canvas2.Dispose();

            //canvas = ResizeBitmap(canvasTemp, (viewScale * 100), true);
            //canvasTemp.Dispose();

            ///////////////////////////////////////////////////////////////////
            // 4. 2D 뷰어 크기 설정
            // - 인치바 와 직물 비트맵을 그리는 영역
            // - 2D 뷰어 크기를 인치 단위로 생성 (1인치, 2인치, ....n인치)
            // - 인치를 그리기 위해 플러스알파 만큼 더 추가
            ///////////////////////////////////////////////////////////////////
            //this.Size = new Size(sizeWidth, sizeHeight);
            //this.Location = new Point(0, 0);

            stopwatch.Stop();
            long timeResize = stopwatch.ElapsedMilliseconds;

            Trace.WriteLine("\n============================================================");
            Trace.WriteLine(
                "Elapsed Time Create Canvas : " + timeCreateCanvas +
                ", Create ImageFile : " + timeImageFile +
                ", Resize Canvas : " + timeResize +
                ", Total : " + (timeCreateCanvas + timeImageFile + timeResize));
            Trace.WriteLine("");

            ///////////////////////////////////////////////////////////////////
            // 3. OnPaint() 호출
            ///////////////////////////////////////////////////////////////////
            Refresh();
        }

        private Bitmap ResizeBitmap(Bitmap image, int scale, bool highQuality = true)
        {
            int resizeWidth = (int)(image.Width * (scale / 100f));
            int resizeHeight = (int)(image.Height * (scale / 100f));

            Bitmap res = new Bitmap(resizeWidth, resizeHeight);

            using (var graphic = Graphics.FromImage(res))
            {
                if (highQuality)
                {
                    graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphic.SmoothingMode = SmoothingMode.HighQuality;
                    graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    graphic.CompositingQuality = CompositingQuality.HighQuality;
                }
                else
                {
                    graphic.InterpolationMode = InterpolationMode.Low;
                    graphic.SmoothingMode = SmoothingMode.HighSpeed;
                    graphic.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                    graphic.CompositingQuality = CompositingQuality.HighSpeed;
                }
                graphic.DrawImage(image, 0, 0, resizeWidth, resizeHeight);
                graphic.Dispose();
            }

            return res;
        }



        ///////////////////////////////////////////////////////////////////////
        // 인치 바 그리기
        ///////////////////////////////////////////////////////////////////////
        private void DrawInchBar(Graphics g)
        {
            bool halfMark = false;
            Rectangle rect;

            int x, y, w, h;
            int x1, y1, x2, y2;

            int sWIDTH = this.Width;
            int sHEIGHT = this.Height;

            g.FillRectangle(new SolidBrush(BG_COLOR), new Rectangle(0, 0, sWIDTH, sHEIGHT));

            ///////////////////////////////////////////////////////////////////
            /// 가로축
            ///////////////////////////////////////////////////////////////////

            // 1. Inch 텍스트 그리기
            g.DrawString("inch", new Font("맑은고딕", 12), new SolidBrush(Color.White), OFFSET_X + 10, 5);


            // 2. 가로 선
            x = 0;
            y = OFFSET_Y - 5;
            w = sWIDTH;
            h = 2;

            rect = new Rectangle(x, y, w, h);
            g.FillRectangle(new SolidBrush(Color.White), rect);


            // 눈금
            int sPPI = PPI * viewScale;
            int sPPIHalf = sPPI / 2;
            int marks = (sWIDTH / sPPIHalf) * 2;
            for (int i = 0; i < marks; i++)
            {
                int temp = (i + 1) * sPPIHalf;
                x1 = OFFSET_X + temp;
                y1 = 5;
                x2 = OFFSET_X + temp;
                y2 = OFFSET_Y - 4;

                if ((i % 2) == 0)
                {
                    y1 = 20;
                }

                if (halfMark == false)
                {
                    g.DrawString("1/2", new Font("맑은고딕", 9), new SolidBrush(Color.White), x1 - 30, 10);
                    halfMark = true;
                }
                g.DrawLine(new Pen(Color.White), x1, y1, x2, y2);
            }


            //3. 세로 선
            x = OFFSET_X - 5;
            y = 0;
            w = 2;
            h = sHEIGHT;

            rect = new Rectangle(x, y, w, h);
            g.FillRectangle(new SolidBrush(Color.White), rect);

            // 눈금
            marks = (sHEIGHT / sPPIHalf) * 2;
            for (int i = 0; i < marks; i++)
            {
                int temp = (i + 1) * sPPIHalf;
                x1 = 5;
                y1 = OFFSET_Y + temp;
                x2 = OFFSET_X - 4;
                y2 = OFFSET_Y + temp;

                if ((i % 2) == 0)
                {
                    x1 = 20;
                }
                g.DrawLine(new Pen(Color.White), x1, y1, x2, y2);
            }

        }



        ///////////////////////////////////////////////////////////////////////
        // 박스형식 그리기
        ///////////////////////////////////////////////////////////////////////
        private void DrawWeaving(Graphics g)
        {
            DrawWeaving(g, 0, 0);
        }
        private void DrawWeaving(Graphics g, Graphics gRough)
        {
            DrawWeaving(g, gRough, 0, 0);
        }
        private void DrawWeaving(Graphics g, int offsetX, int offsetY)
        {
            if (listWeaveData.Count <= 0) return;

            int x = offsetX;
            int y = offsetY;
            for (int i = 0; i < listWeaveData.Count; i++)
            {
                List<BoxData> rows = listWeaveData[i];
                for (int j = 0; j < rows.Count; j++)
                {
                    BoxData box = rows[j];

                    int Data = box.Data;
                    Color warpColor = box.warpColor;
                    Color weftColor = box.weftColor;
                    int warpWeight = box.warpWeight;
                    int weftWeight = box.weftWeight;

                    if (Data == 1)
                    {
                        DrawWarpUpVer2(g, x, y,
                            boxWidth, boxHeight,
                            warpColor, weftColor,
                            warpWeight, weftWeight);
                    }
                    else
                    {
                        DrawWeftUpVer2(g, x, y,
                            boxWidth, boxHeight,
                            warpColor, weftColor,
                            warpWeight, weftWeight);
                    }
                    x += boxWidth;
                }
                x = offsetX;
                y += boxHeight;
            }
        }

        private void DrawWeaving(Graphics g, Graphics gRough, int offsetX, int offsetY)
        {
            if (listWeaveData.Count <= 0) return;

            int x = offsetX;
            int y = offsetY;
            for (int i = 0; i < listWeaveData.Count; i++)
            {
                List<BoxData> rows = listWeaveData[i];
                for (int j = 0; j < rows.Count; j++)
                {
                    BoxData box = rows[j];

                    int Data = box.Data;

                    Color warpColor = box.warpColor;
                    Color weftColor = box.weftColor;

                    //2025-02-05 soonchol
                    if (YarnDyeColor != null)
                    {
                        warpColor = ProjectData.GetDyeColor(YarnDyeColor);
                        weftColor = ProjectData.GetDyeColor(YarnDyeColor);
                    }

                    int warpWeight = box.warpWeight;
                    int weftWeight = box.weftWeight;

                    Color warpShinyColor = box.warpShinyColor;
                    Color weftShinyColor = box.weftShinyColor;

                    if (Data == 1)
                    {
                        DrawWarpUpVer2(g, gRough,
                            x, y,
                            boxWidth, boxHeight,
                            warpColor, weftColor,
                            warpWeight, weftWeight,
                            warpShinyColor, weftShinyColor);
                    }
                    else
                    {
                        DrawWeftUpVer2(g, gRough,
                            x, y,
                            boxWidth, boxHeight,
                            warpColor, weftColor,
                            warpWeight, weftWeight,
                            warpShinyColor, weftShinyColor);
                    }
                    x += boxWidth;
                }
                x = offsetX;
                y += boxHeight;
            }
        }
        ///////////////////////////////////////////////////////////////////////
        // weight : 굵기
        // den : 밀도
        // maxWeight : 최대 굵기
        // n : 길이
        ///////////////////////////////////////////////////////////////////////
        private int GetPadding(int weight, int den, double maxWeight, int n)
        {
            int temp = 0;
            double per = 0.0;
            int padding = 0;

            // maxWeight = 15,000(경사의경우), 10,000(위사의경우) 이면 가득차게 그림
            temp = weight * den;
            if (temp >= maxWeight) return 0;

            per = (((double)temp / maxWeight));
            if (per > 0.9) per = 0.9;
            if (per < 0.5) per = 0.5;

            padding = (n - Convert.ToInt32(n * per)) / 2;

            return padding;
        }
        private void DrawWarpUpVer2(Graphics g,
            int x, int y,
            int width, int height,
            Color warpColor, Color weftColor,
            int warpWeight, int weftWeight)
        {
            //double per = 0.0;
            int padding = 0;

            int a = 0, b = 0, c = 0, d = 0;

            // 위사 그리기
            padding = GetPadding(weftWeight, densityY, DEN_MAX_WEFT, height);
            a = x;
            b = y + padding;
            c = width;
            d = height - (padding * 2);
            if (d < 1) d = 1;
            g.FillRectangle(new SolidBrush(weftColor), new Rectangle(a, b, c, d));
            //테두리 라인
            //g.DrawLine(new Pen(Color.Gray, 1), new Point(a, b), new Point(a + c, b));
            //g.DrawLine(new Pen(Color.Gray, 1), new Point(a, b + d), new Point(a + c, b + d));

            // 경사 그리기
            padding = GetPadding(warpWeight, densityX, DEN_MAX_WARP, width);
            a = x + padding;
            b = y;
            c = width - (padding * 2);
            if (c < 1) c = 1;
            d = height;
            g.FillRectangle(new SolidBrush(warpColor), new Rectangle(a, b, c, d));
            //테두리 라인
            //g.DrawLine(new Pen(Color.Gray, 1), new Point(a, b), new Point(a, b + d));
            //g.DrawLine(new Pen(Color.Gray, 1), new Point(a + c, b), new Point(a + c, b + d));
        }
        private void DrawWarpUpVer2(Graphics g, Graphics gRough,
            int x, int y,
            int width, int height,
            Color warpColor, Color weftColor,
            int warpWeight, int weftWeight,
            Color warpShinyColor, Color weftShinyColor)
        {
            //double per = 0.0;
            int padding = 0;

            int a = 0, b = 0, c = 0, d = 0;

            // 위사 그리기
            padding = GetPadding(weftWeight, densityY, DEN_MAX_WEFT, height);
            a = x;
            b = y + padding;
            c = width;
            d = height - (padding * 2);
            if (d < 1) d = 1;
            g.FillRectangle(new SolidBrush(weftColor), new Rectangle(a, b, c, d));
            gRough.FillRectangle(new SolidBrush(weftShinyColor), new Rectangle(a, b, c, d));
            //테두리 라인
            //g.DrawLine(new Pen(Color.Gray, 1), new Point(a, b), new Point(a + c, b));
            //g.DrawLine(new Pen(Color.Gray, 1), new Point(a, b + d), new Point(a + c, b + d));

            // 경사 그리기
            padding = GetPadding(warpWeight, densityX, DEN_MAX_WARP, width);
            a = x + padding;
            b = y;
            c = width - (padding * 2);
            if (c < 1) c = 1;
            d = height;
            g.FillRectangle(new SolidBrush(warpColor), new Rectangle(a, b, c, d));
            gRough.FillRectangle(new SolidBrush(warpShinyColor), new Rectangle(a, b, c, d));
            //테두리 라인
            //g.DrawLine(new Pen(Color.Gray, 1), new Point(a, b), new Point(a, b + d));
            //g.DrawLine(new Pen(Color.Gray, 1), new Point(a + c, b), new Point(a + c, b + d));
        }
        private void DrawWeftUpVer2(Graphics g, int x, int y,
            int width, int height,
            Color warpColor, Color weftColor,
            int warpWeight, int weftWeight)
        {
            //double per = 0.0;
            int padding = 0;

            int a = 0, b = 0, c = 0, d = 0;

            // 경사 그리기
            padding = GetPadding(warpWeight, densityX, DEN_MAX_WARP, width);
            a = x + padding;
            b = y;
            c = width - (padding * 2);
            if (c < 1) c = 1;
            d = height;
            g.FillRectangle(new SolidBrush(warpColor), new Rectangle(a, b, c, d));
            //테두리 라인
            //g.DrawLine(new Pen(Color.Gray, 1), new Point(a, b), new Point(a, b + d));
            //g.DrawLine(new Pen(Color.Gray, 1), new Point(a + c, b), new Point(a + c, b + d));

            // 위사 그리기
            padding = GetPadding(weftWeight, densityY, DEN_MAX_WEFT, height);
            a = x;
            b = y + padding;
            c = width;
            d = height - (padding * 2);
            if (d < 1) d = 1;
            g.FillRectangle(new SolidBrush(weftColor), new Rectangle(a, b, c, d));
            //테두리 라인
            //g.DrawLine(new Pen(Color.Gray, 1), new Point(a, b), new Point(a + c, b));
            //g.DrawLine(new Pen(Color.Gray, 1), new Point(a, b + d), new Point(a + c, b + d));
        }

        private void DrawWeftUpVer2(Graphics g, Graphics gRough,
            int x, int y,
            int width, int height,
            Color warpColor, Color weftColor,
            int warpWeight, int weftWeight,
            Color warpShinyColor, Color weftShinyColor)
        {
            //double per = 0.0;
            int padding = 0;

            int a = 0, b = 0, c = 0, d = 0;

            // 경사 그리기
            padding = GetPadding(warpWeight, densityX, DEN_MAX_WARP, width);
            a = x + padding;
            b = y;
            c = width - (padding * 2);
            if (c < 1) c = 1;
            d = height;
            g.FillRectangle(new SolidBrush(warpColor), new Rectangle(a, b, c, d));
            gRough.FillRectangle(new SolidBrush(warpShinyColor), new Rectangle(a, b, c, d));
            //테두리 라인
            //g.DrawLine(new Pen(Color.Gray, 1), new Point(a, b), new Point(a, b + d));
            //g.DrawLine(new Pen(Color.Gray, 1), new Point(a + c, b), new Point(a + c, b + d));

            // 위사 그리기
            padding = GetPadding(weftWeight, densityY, DEN_MAX_WEFT, height);
            a = x;
            b = y + padding;
            c = width;
            d = height - (padding * 2);
            if (d < 1) d = 1;
            g.FillRectangle(new SolidBrush(weftColor), new Rectangle(a, b, c, d));
            gRough.FillRectangle(new SolidBrush(weftShinyColor), new Rectangle(a, b, c, d));
            //테두리 라인
            //g.DrawLine(new Pen(Color.Gray, 1), new Point(a, b), new Point(a + c, b));
            //g.DrawLine(new Pen(Color.Gray, 1), new Point(a, b + d), new Point(a + c, b + d));
        }



        ///////////////////////////////////////////////////////////////////////
        // 원사 이미지
        ///////////////////////////////////////////////////////////////////////
        private void DrawWeavingYarn(Graphics g)
        {
            DrawWeavingYarn(g, 0, 0);
        }
        private void DrawWeavingYarn(Graphics g, Graphics gRough)
        {
            DrawWeavingYarn(g, gRough, 0, 0);
        }
        private void DrawWeavingYarn(Graphics g, int offsetX, int offsetY)
        {
            if (listWeaveData.Count <= 0) return;
            if (listWeaveData[0].Count <= 0) return;

            //Trace.WriteLine("ENTER - DrawWeavingSNU()");
            //int temp = 0;
            int x = offsetX;
            int y = offsetY;
            Random rand = new Random();

            // 세로 원사 시작 점을 랜덤으로 설정
            int[] randY = new int[listWeaveData[0].Count];
            for (int i = 0; i < listWeaveData[0].Count; i++)
            {
                randY[i] = rand.Next(100);
            }


            for (int i = 0; i < listWeaveData.Count; i++)
            {
                List<BoxData> row = listWeaveData[i];

                // 가로 원사 시작 점을 랜덤으로 설정
                int rX = rand.Next(100);

                for (int j = 0; j < row.Count; j++)
                {
                    BoxData weaveData = row[j];

                    int Data = weaveData.Data;
                    Color warpColor = weaveData.warpColor;
                    Color weftColor = weaveData.weftColor;
                    int warpWeight = weaveData.warpWeight;
                    int weftWeight = weaveData.weftWeight;
                    Bitmap warpBitmap = yarnMap[weaveData.warpKey];
                    Bitmap weftBitmap = yarnMap[weaveData.weftKey];
                    //Trace.Write(j + "\t");
                    if (Data == 1)
                    {
                        DrawWeftYarn2(g, x, y, boxWidth, boxHeight, j + rX, weftWeight, weftBitmap);
                        DrawWarpYarn2(g, x, y, boxWidth, boxHeight, i + randY[j], warpWeight, warpBitmap);
                    }
                    else
                    {
                        DrawWarpYarn2(g, x, y, boxWidth, boxHeight, i + randY[j], warpWeight, warpBitmap);
                        DrawWeftYarn2(g, x, y, boxWidth, boxHeight, j + rX, weftWeight, weftBitmap);
                    }
                    x += boxWidth;
                }
                //Trace.Write("\r\n");
                x = offsetX;
                y += boxHeight;
            }
        }
        private void DrawWeavingYarn(Graphics g, Graphics gRough, int offsetX, int offsetY)
        {
            if (listWeaveData.Count <= 0) return;
            if (listWeaveData[0].Count <= 0) return;

            //Trace.WriteLine("ENTER - DrawWeavingSNU()");
            //int temp = 0;
            int x = offsetX;
            int y = offsetY;
            Random rand = new Random();

            // 세로 원사 시작 점을 랜덤으로 설정
            int[] randY = new int[listWeaveData[0].Count];
            for (int i = 0; i < listWeaveData[0].Count; i++)
            {
                randY[i] = rand.Next(100);
            }


            for (int i = 0; i < listWeaveData.Count; i++)
            {
                List<BoxData> row = listWeaveData[i];

                // 가로 원사 시작 점을 랜덤으로 설정
                int rX = rand.Next(100);

                for (int j = 0; j < row.Count; j++)
                {
                    BoxData weaveData = row[j];

                    int Data = weaveData.Data;
                    Color warpColor = weaveData.warpColor;
                    Color weftColor = weaveData.weftColor;
                    int warpWeight = weaveData.warpWeight;
                    int weftWeight = weaveData.weftWeight;
                    Bitmap warpBitmap = yarnMap[weaveData.warpKey];
                    Bitmap weftBitmap = yarnMap[weaveData.weftKey];

                    Bitmap warpShinyBitmap = yarnShinyMap[weaveData.warpShinyKey];
                    Bitmap weftShinyBitmap = yarnShinyMap[weaveData.weftShinyKey];

                    //Trace.Write(j + "\t");
                    if (Data == 1)
                    {
                        //DrawWeftYarn2(g, gRough, x, y, boxWidth, boxHeight, j + rX, weftWeight, weftBitmap, weftShinyBitmap);
                        //DrawWarpYarn2(g, gRough, x, y, boxWidth, boxHeight, i + randY[j], warpWeight, warpBitmap, warpShinyBitmap);
                        DrawWeftYarn3(g, gRough, x, y, boxWidth, boxHeight, j + rX, weftWeight, weftBitmap, weftShinyBitmap);
                        DrawWarpYarn3(g, gRough, x, y, boxWidth, boxHeight, i + randY[j], warpWeight, warpBitmap, warpShinyBitmap);
                    }
                    else
                    {
                        //DrawWarpYarn2(g, gRough, x, y, boxWidth, boxHeight, i + randY[j], warpWeight, warpBitmap, warpShinyBitmap);
                        //DrawWeftYarn2(g, gRough, x, y, boxWidth, boxHeight, j + rX, weftWeight, weftBitmap, weftShinyBitmap);
                        DrawWarpYarn3(g, gRough, x, y, boxWidth, boxHeight, i + randY[j], warpWeight, warpBitmap, warpShinyBitmap);
                        DrawWeftYarn3(g, gRough, x, y, boxWidth, boxHeight, j + rX, weftWeight, weftBitmap, weftShinyBitmap);
                    }
                    x += boxWidth;
                }
                //Trace.Write("\r\n");
                x = offsetX;
                y += boxHeight;
            }
        }
        ///////////////////////////////////////////////////////////////////////
        // 필요없는 코드
        //
        // **************** 원사 마이크로 이미지 적용 방법 ****************
        //
        // 1. 원사 마이크로 이미지 제작 (디자인툴 이용)
        //  1.1. 원사 이미지 제작 ( 250 x40 )
        //  - 색상을 변경 적용해야 하는 부분은 투명으로 처리
        //  - 실의 무늬를 표현하는 색은 검은색(0x000000)이 아닌 색을 선택
        //  - 실의 바깥 부분은 검은색(0x000000)으로 칠함.
        //  1.2. 반전시켜서 길이 방향 끝에 추가 ( 500 x 40 )
        //  1.3. Visual Studio 프로젝트 리소스에 추가
        //
        // [런타임]
        // 2. 원사 비트맵 이미지 생성
        //  2.1. 리소스의 이미지를 반전하여 길이 방향으로 추가( 1000 x 40 )
        //  2.2. 사용자의 경/위사 색상을 원사 비트맵의 투명 부분에 적용
        //  2.3. 검은색(0x000000)을 투명하게 처리
        //  2.4. 경사의 경우 90도 회전(세로방향으로...)
        // 
        // 3. 그리기
        //  3.1. 경/위사가 교차된 지점이 하나의 단위 (Box)
        //  3.2. 원사 이미지에서 Box 크기만큼을 가져와서 뷰어에 출력
        //  3.3. 다음 Box를 그릴때는 Box 폭만큼 이동한 이미지
        //  3.4. 원사 이미지 총 길이의 반(1000/2)을 넘어선 경우 원점으로 돌아감
        //   - 0(zero) 에서 넘어선 만큼 더한 위치로 돌아가야함
        //
        ///////////////////////////////////////////////////////////////////////
        private void DrawWeftYarn(Graphics g, int x, int y, int w, int h, int c, int weight, Bitmap bm)
        {
            //double per = 0.0;
            int padding = 0;
            padding = GetPadding(weight, densityY, DEN_MAX_WEFT, h);

            double WeftW2 = (double)bm.Width / 2.0f;
            float SC = w * c;
            int r = (int)(SC / WeftW2);
            int s = (int)(SC - (float)(r * WeftW2));
            Rectangle rectSrc = new Rectangle(
                s,
                0 + 1,              //원사 이미지 테두리 1픽셀을 버림 (투명도를 원사 이미지가 적용안된 것과 맞춤)
                w,
                bm.Height - 2);     //원사 이미지 테두리 1픽셀을 버림
            Rectangle rectDest = new Rectangle(
                x,
                y + padding,
                w,
                h - (padding * 2));
            g.DrawImage(bm, rectDest, rectSrc, GraphicsUnit.Pixel);
        }

        private void DrawWarpYarn(Graphics g, int x, int y, int w, int h, int c, int weight, Bitmap bm)
        {
            //double per = 0.0;
            int padding = 0;
            padding = GetPadding(weight, densityY, DEN_MAX_WEFT, w);

            double WarpH2 = (double)bm.Height / 2.0f;
            float SC = w * c;
            int r = (int)(SC / WarpH2);
            int s = (int)(SC - (float)(r * WarpH2));

            Rectangle rectSrc = new Rectangle(
                0 + 1,          //원사 이미지 테두리 1픽셀을 버림
                s,
                bm.Width - 2,   //원사 이미지 테두리 1픽셀을 버림
                h);
            Rectangle rectDest = new Rectangle(
                x + padding,
                y,
                w - (padding * 2),
                h);
            g.DrawImage(bm, rectDest, rectSrc, GraphicsUnit.Pixel);
        }

        ///////////////////////////////////////////////////////////////////////
        //
        // **************** 원사 마이크로 이미지 적용 방법 2 ****************
        //
        // 1. 원사 마이크로 이미지 제작 (디자인툴 이용)
        //  1.1. 원사 이미지 제작 ( 174 x 14 )
        //  - 색상을 변경 적용해야 하는 부분은 투명으로 처리
        //  - 실의 무늬를 표현하는 색은 검은색(0x000000)이 아닌 색을 선택
        //  - 실의 바깥 부분은 검은색(0x000000)으로 칠함.
        //  1.2. Visual Studio 프로젝트 리소스에 추가
        //
        // [런타임]
        // 2. 원사 비트맵 이미지 생성
        //  2.1. 사용자의 경/위사 색상을 원사 비트맵의 투명 부분에 적용
        //  2.2. 검은색(0x000000)을 투명하게 처리
        //  2.3. 경사의 경우 90도 회전(세로방향으로...)
        // 
        // 3. 그리기
        //  3.1. 경/위사가 교차된 지점이 하나의 단위 (Box)
        //  3.2. 원사 이미지에서 Box 크기만큼을 가져와서 뷰어에 출력
        //  3.3. 다음 Box를 그릴때는 Box 폭만큼 이동하여 이미지 복사 및 출력
        //  3.4. 만약 남은 원사의 이미지보다 Box 크기가 큰 경우 
        //   - 남은 이미지 복사 및 출력
        //   - 0(zero) 위치로 돌아가서 필요한 만큼 더 복사하여 출력
        //
        ///////////////////////////////////////////////////////////////////////
        private void DrawWeftYarn2(Graphics g, int x, int y, int w, int h, int c, int weight, Bitmap bm)
        {
            //double per = 0.0;
            int padding = 0;
            padding = GetPadding(weight, densityY, DEN_MAX_WEFT, h);

            double WeftW2 = (double)bm.Width / 2.0f;
            float SC = w * c;
            int r = (int)(SC / WeftW2);
            int s = (int)(SC - (float)(r * WeftW2));
            if ((s + w) > WeftW2)
            {
                int w1 = (int)WeftW2 - s;   // 이미지 크기 만큼 그림
                int w2 = w - w1;            // 0 부터 남은 길이 만큼 그림

                // 이미지 크기 만큼 그림
                Rectangle rectSrc = new Rectangle(
                    s,
                    0 + 1,              //원사 이미지 테두리 1픽셀을 버림 (투명도를 원사 이미지가 적용안된 것과 맞춤)
                    w1,
                    bm.Height - 2);     //원사 이미지 테두리 1픽셀을 버림
                Rectangle rectDest = new Rectangle(
                    x,
                    y + padding,
                    w1,
                    h - (padding * 2));
                g.DrawImage(bm, rectDest, rectSrc, GraphicsUnit.Pixel);


                // 0 부터 남은 길이 만큼 그림
                rectSrc = new Rectangle(
                    0,
                    0 + 1,
                    w2,
                    bm.Height - 2);
                rectDest = new Rectangle(
                    x + w1,
                    y + padding,
                    w2,
                    h - (padding * 2));
                g.DrawImage(bm, rectDest, rectSrc, GraphicsUnit.Pixel);
            }
            else
            {
                Rectangle rectSrc = new Rectangle(
                    s,
                    0 + 1,              //원사 이미지 테두리 1픽셀을 버림 (투명도를 원사 이미지가 적용안된 것과 맞춤)
                    w,
                    bm.Height - 2);     //원사 이미지 테두리 1픽셀을 버림
                Rectangle rectDest = new Rectangle(
                    x,
                    y + padding,
                    w,
                    h - (padding * 2));
                g.DrawImage(bm, rectDest, rectSrc, GraphicsUnit.Pixel);
            }
        }
        private void DrawWeftYarn2(Graphics g, Graphics gRough, int x, int y, int w, int h, int c, int weight, Bitmap bm, Bitmap bmShiny)
        {
            //double per = 0.0;
            int padding = 0;
            padding = GetPadding(weight, densityY, DEN_MAX_WEFT, h);

            double WeftW2 = (double)bm.Width / 2.0f;
            float SC = w * c;
            int r = (int)(SC / WeftW2);
            int s = (int)(SC - (float)(r * WeftW2));
            if ((s + w) > WeftW2)
            {
                int w1 = (int)WeftW2 - s;   // 이미지 크기 만큼 그림
                int w2 = w - w1;            // 0 부터 남은 길이 만큼 그림

                // 이미지 크기 만큼 그림
                Rectangle rectSrc = new Rectangle(
                    s,
                    0 + 1,              //원사 이미지 테두리 1픽셀을 버림 (투명도를 원사 이미지가 적용안된 것과 맞춤)
                    w1,
                    bm.Height - 2);     //원사 이미지 테두리 1픽셀을 버림
                Rectangle rectDest = new Rectangle(
                    x,
                    y + padding,
                    w1,
                    h - (padding * 2));
                g.DrawImage(bm, rectDest, rectSrc, GraphicsUnit.Pixel);
                gRough.DrawImage(bmShiny, rectDest, rectSrc, GraphicsUnit.Pixel);


                // 0 부터 남은 길이 만큼 그림
                rectSrc = new Rectangle(
                    0,
                    0 + 1,
                    w2,
                    bm.Height - 2);
                rectDest = new Rectangle(
                    x + w1,
                    y + padding,
                    w2,
                    h - (padding * 2));
                g.DrawImage(bm, rectDest, rectSrc, GraphicsUnit.Pixel);
                gRough.DrawImage(bmShiny, rectDest, rectSrc, GraphicsUnit.Pixel);
            }
            else
            {
                Rectangle rectSrc = new Rectangle(
                    s,
                    0 + 1,              //원사 이미지 테두리 1픽셀을 버림 (투명도를 원사 이미지가 적용안된 것과 맞춤)
                    w,
                    bm.Height - 2);     //원사 이미지 테두리 1픽셀을 버림
                Rectangle rectDest = new Rectangle(
                    x,
                    y + padding,
                    w,
                    h - (padding * 2));
                g.DrawImage(bm, rectDest, rectSrc, GraphicsUnit.Pixel);
                gRough.DrawImage(bmShiny, rectDest, rectSrc, GraphicsUnit.Pixel);
            }
        }
        private void DrawWeftYarn3(Graphics g, Graphics gRough, int x, int y, int w, int h, int c, int weight, Bitmap bm, Bitmap bmShiny)
        {
            //double per = 0.0;
            int padding = 0;
            padding = GetPadding(weight, densityY, DEN_MAX_WEFT, h);

            int WeftW2 = bm.Width;

            // Box 폭 => 원본의 폭(비율)
            int src_w = (int)((w * bm.Height) / h);

            ///////////////////////////////////////////////////////////////////
            // 시작 - 시작점계산 (원본)
            ///////////////////////////////////////////////////////////////////
            float SC = src_w * c;
            int r = (int)(SC / WeftW2);
            int s = (int)(SC - (float)(r * WeftW2));
            ///////////////////////////////////////////////////////////////////
            // 끝 - 시작점계산 (원본)
            ///////////////////////////////////////////////////////////////////


            if ((s + src_w) > WeftW2)
            {
                ///////////////////////////////////////////////////////////////
                // 시작 - 남은 영역만큼 끝까지 그림
                ///////////////////////////////////////////////////////////////
                int temp_src_w = WeftW2 - s;
                int temp_w_1 = (int)((temp_src_w * w) / src_w);

                // 이미지 크기 만큼 그림
                Rectangle rectSrc = new Rectangle(
                    s,
                    0 + 1,              //원사 이미지 테두리 1픽셀을 버림 (투명도를 원사 이미지가 적용안된 것과 맞춤)
                    temp_src_w,
                    bm.Height - 2);     //원사 이미지 테두리 1픽셀을 버림
                Rectangle rectDest = new Rectangle(
                    x,
                    y + padding,
                    temp_w_1,
                    h - (padding * 2));
                g.DrawImage(bm, rectDest, rectSrc, GraphicsUnit.Pixel);
                gRough.DrawImage(bmShiny, rectDest, rectSrc, GraphicsUnit.Pixel);
                //Trace.WriteLine("* x: " + x + ", s:"+s+", w3:" + temp_w_1);
                ///////////////////////////////////////////////////////////////
                // 끝 - 남은 영역만큼 끝까지 그림
                ///////////////////////////////////////////////////////////////


                ///////////////////////////////////////////////////////////////
                // 시작 - 0 부터 모자란 만큼 그림
                ///////////////////////////////////////////////////////////////
                temp_src_w = src_w - temp_src_w;

                int temp_w_2 = w - temp_w_1;

                // 0 부터 남은 길이 만큼 그림
                rectSrc = new Rectangle(
                    0,
                    0 + 1,
                    temp_src_w,
                    bm.Height - 2);
                rectDest = new Rectangle(
                    x + temp_w_1,
                    y + padding,
                    temp_w_2,
                    h - (padding * 2));
                g.DrawImage(bm, rectDest, rectSrc, GraphicsUnit.Pixel);
                gRough.DrawImage(bmShiny, rectDest, rectSrc, GraphicsUnit.Pixel);
                //Trace.WriteLine("* x: "+(x+ temp_w_1) +", s:0, w3:" + temp_w_2);
                ///////////////////////////////////////////////////////////////
                // 끝 - 0 부터 모자란 만큼 그림
                ///////////////////////////////////////////////////////////////

            }
            else
            {
                Rectangle rectSrc = new Rectangle(
                    s,
                    0 + 1,              //원사 이미지 테두리 1픽셀을 버림 (투명도를 원사 이미지가 적용안된 것과 맞춤)
                    src_w,
                    bm.Height - 2);     //원사 이미지 테두리 1픽셀을 버림
                Rectangle rectDest = new Rectangle(
                    x,
                    y + padding,
                    w,
                    h - (padding * 2));
                g.DrawImage(bm, rectDest, rectSrc, GraphicsUnit.Pixel);
                gRough.DrawImage(bmShiny, rectDest, rectSrc, GraphicsUnit.Pixel);
                //Trace.WriteLine("x:"+x+", s:"+s+", w3:"+ w);
            }
        }
        private void DrawWarpYarn2(Graphics g, int x, int y, int w, int h, int c, int weight, Bitmap bm)
        {
            //double per = 0.0;
            int padding = 0;
            padding = GetPadding(weight, densityY, DEN_MAX_WEFT, w);

            double WarpH2 = (double)bm.Height / 2.0f;
            float SC = w * c;
            int r = (int)(SC / WarpH2);
            int s = (int)(SC - (float)(r * WarpH2));

            if ((s + w) > WarpH2)
            {

                int h1 = (int)WarpH2 - s;   // 이미지 크기 만큼 그림
                int h2 = w - h1;            // 0 부터 남은 길이 만큼 그림

                Rectangle rectSrc = new Rectangle(
                    0 + 1,          //원사 이미지 테두리 1픽셀을 버림
                    s,
                    bm.Width - 2,   //원사 이미지 테두리 1픽셀을 버림
                    h1);
                Rectangle rectDest = new Rectangle(
                    x + padding,
                    y,
                    w - (padding * 2),
                    h1);
                g.DrawImage(bm, rectDest, rectSrc, GraphicsUnit.Pixel);


                rectSrc = new Rectangle(
                    0 + 1,
                    s,
                    bm.Width - 2,
                    h2);
                rectDest = new Rectangle(
                    x + padding,
                    y + h1,
                    w - (padding * 2),
                    h2);
                g.DrawImage(bm, rectDest, rectSrc, GraphicsUnit.Pixel);
            }
            else
            {
                Rectangle rectSrc = new Rectangle(
                    0 + 1,          //원사 이미지 테두리 1픽셀을 버림
                    s,
                    bm.Width - 2,   //원사 이미지 테두리 1픽셀을 버림
                    h);
                Rectangle rectDest = new Rectangle(
                    x + padding,
                    y,
                    w - (padding * 2),
                    h);
                g.DrawImage(bm, rectDest, rectSrc, GraphicsUnit.Pixel);
            }
        }

        private void DrawWarpYarn2(Graphics g, Graphics gRough, int x, int y, int w, int h, int c, int weight, Bitmap bm, Bitmap bmShiny)
        {
            //double per = 0.0;
            int padding = 0;
            padding = GetPadding(weight, densityY, DEN_MAX_WEFT, w);

            double WarpH2 = (double)bm.Height / 2.0f;
            float SC = w * c;
            int r = (int)(SC / WarpH2);
            int s = (int)(SC - (float)(r * WarpH2));

            if ((s + w) > WarpH2)
            {

                int h1 = (int)WarpH2 - s;   // 이미지 크기 만큼 그림
                int h2 = w - h1;            // 0 부터 남은 길이 만큼 그림

                Rectangle rectSrc = new Rectangle(
                    0 + 1,          //원사 이미지 테두리 1픽셀을 버림
                    s,
                    bm.Width - 2,   //원사 이미지 테두리 1픽셀을 버림
                    h1);
                Rectangle rectDest = new Rectangle(
                    x + padding,
                    y,
                    w - (padding * 2),
                    h1);
                g.DrawImage(bm, rectDest, rectSrc, GraphicsUnit.Pixel);
                gRough.DrawImage(bmShiny, rectDest, rectSrc, GraphicsUnit.Pixel);


                rectSrc = new Rectangle(
                    0 + 1,
                    s,
                    bm.Width - 2,
                    h2);
                rectDest = new Rectangle(
                    x + padding,
                    y + h1,
                    w - (padding * 2),
                    h2);
                g.DrawImage(bm, rectDest, rectSrc, GraphicsUnit.Pixel);
                gRough.DrawImage(bmShiny, rectDest, rectSrc, GraphicsUnit.Pixel);
            }
            else
            {
                Rectangle rectSrc = new Rectangle(
                    0 + 1,          //원사 이미지 테두리 1픽셀을 버림
                    s,
                    bm.Width - 2,   //원사 이미지 테두리 1픽셀을 버림
                    h);
                Rectangle rectDest = new Rectangle(
                    x + padding,
                    y,
                    w - (padding * 2),
                    h);
                g.DrawImage(bm, rectDest, rectSrc, GraphicsUnit.Pixel);
                gRough.DrawImage(bmShiny, rectDest, rectSrc, GraphicsUnit.Pixel);
            }
        }

        private void DrawWarpYarn3(Graphics g, Graphics gRough, int x, int y, int w, int h, int c, int weight, Bitmap bm, Bitmap bmShiny)
        {
            //double per = 0.0;
            int padding = 0;
            padding = GetPadding(weight, densityY, DEN_MAX_WEFT, w);

            int WarpH2 = bm.Height;

            // Box 높이 vs 원본의 높이(비율)
            int src_h = (int)((h * bm.Width) / w);

            ///////////////////////////////////////////////////////////////////
            // 시작 - 시작점계산 (원본)
            ///////////////////////////////////////////////////////////////////
            float SC = src_h * c;
            int r = (int)(SC / WarpH2);
            int s = (int)(SC - (float)(r * WarpH2));
            ///////////////////////////////////////////////////////////////////
            // 끝 - 시작점계산 (원본)
            ///////////////////////////////////////////////////////////////////

            if ((s + src_h) > WarpH2)
            {
                ///////////////////////////////////////////////////////////////
                // 시작 - 남은 영역만큼 끝까지 그림
                ///////////////////////////////////////////////////////////////
                int temp_src_h = WarpH2 - s;
                int temp_h_1 = (int)((temp_src_h * w) / src_h);

                Rectangle rectSrc = new Rectangle(
                    0 + 1,          //원사 이미지 테두리 1픽셀을 버림
                    s,
                    bm.Width - 2,   //원사 이미지 테두리 1픽셀을 버림
                    temp_src_h);
                Rectangle rectDest = new Rectangle(
                    x + padding,
                    y,
                    w - (padding * 2),
                    temp_h_1);
                g.DrawImage(bm, rectDest, rectSrc, GraphicsUnit.Pixel);
                gRough.DrawImage(bmShiny, rectDest, rectSrc, GraphicsUnit.Pixel);
                ///////////////////////////////////////////////////////////////
                // 끝 - 남은 영역만큼 끝까지 그림
                ///////////////////////////////////////////////////////////////

                ///////////////////////////////////////////////////////////////
                // 시작 - 0 부터 모자란 만큼 그림
                ///////////////////////////////////////////////////////////////
                temp_src_h = src_h - temp_src_h;

                int temp_h_2 = w - temp_h_1;

                rectSrc = new Rectangle(
                    0 + 1,
                    0,
                    bm.Width - 2,
                    temp_src_h);
                rectDest = new Rectangle(
                    x + padding,
                    y + temp_h_1,
                    w - (padding * 2),
                    temp_h_2);
                g.DrawImage(bm, rectDest, rectSrc, GraphicsUnit.Pixel);
                gRough.DrawImage(bmShiny, rectDest, rectSrc, GraphicsUnit.Pixel);
                ///////////////////////////////////////////////////////////////
                // 끝 - 0 부터 모자란 만큼 그림
                ///////////////////////////////////////////////////////////////
            }
            else
            {
                Rectangle rectSrc = new Rectangle(
                    0 + 1,          //원사 이미지 테두리 1픽셀을 버림
                    s,
                    bm.Width - 2,   //원사 이미지 테두리 1픽셀을 버림
                    src_h);
                Rectangle rectDest = new Rectangle(
                    x + padding,
                    y,
                    w - (padding * 2),
                    h);
                g.DrawImage(bm, rectDest, rectSrc, GraphicsUnit.Pixel);
                gRough.DrawImage(bmShiny, rectDest, rectSrc, GraphicsUnit.Pixel);
            }
        }







        public void Export2DDImage(string diffPath)
        {
            if (this.canvas != null && this.canvas.Width > 0)
            {
                //diffFilePath;
                //normFilePath;

                string parentPath = Path.GetDirectoryName(diffPath);
                string fileName = Path.GetFileNameWithoutExtension(diffPath);

                string destDiffFile = parentPath + "\\" + fileName + ".png";
                string destNormFile = parentPath + "\\" + fileName + "_N.png";
                System.IO.File.Copy(diffFilePath, destDiffFile, true);
                System.IO.File.Copy(normFilePath, destNormFile, true);
            }
        }



        //Bitmap flag = null;
        Bitmap m_bmp_image;
        Bitmap m_bmp_nmap;
        Bitmap m_bmp_nmap_blur;
        //Bitmap m_bmp_channel;
        string diffFilePath = "";
        string normFilePath = "";
        string rougFilePath = "";
        public string GetDiffFilePath()
        {
            return this.diffFilePath;
        }
        public string GetNormFilePath()
        {
            return this.normFilePath;
        }


        public void Generate3DImage(Bitmap bm)
        {

            if (bm == null || bm.Width <= 0)
            {
                return;
            }

            //flag.MakeTransparent(Color.Transparent);

            diffFilePath = MainForm.WWWROOT_PATH + "\\diff_" + idx + ".png";
            normFilePath = MainForm.WWWROOT_PATH + "\\norm_" + idx + ".png";
            //Trace.WriteLine("Create Diff File : " + diffFilePath + ", " + normFilePath);

            //int x = 0;
            //int y = 0;
            int w = bm.Width;
            int h = bm.Height;

            bm.Save(diffFilePath, ImageFormat.Png);
            //if (IsNormalMap == true)
            {
                m_bmp_image = new Bitmap(bm);

                m_bmp_nmap = new Bitmap(m_bmp_image.Width, m_bmp_image.Height);
                m_bmp_nmap_blur = new Bitmap(m_bmp_image.Width, m_bmp_image.Height);
                //m_bmp_channel = new Bitmap(m_bmp_image.Width, m_bmp_image.Height);
                GenerateNormalMap();
                BoxBlur(m_bmp_nmap, m_bmp_nmap_blur);
                BoxBlur(m_bmp_nmap_blur, m_bmp_nmap);
                m_bmp_nmap.Save(normFilePath, ImageFormat.Png);

                m_bmp_image.Dispose();
                m_bmp_nmap.Dispose();
                m_bmp_nmap_blur.Dispose();
                //m_bmp_channel.Dispose();

                m_bmp_image = null;
                m_bmp_nmap = null;
                m_bmp_nmap_blur = null;
                //m_bmp_channel = null;
            }

        }

        public void Generate3DImageRough(Bitmap bm)
        {

            if (bm == null || bm.Width <= 0)
            {
                return;
            }

            //flag.MakeTransparent(Color.Transparent);
            rougFilePath = MainForm.WWWROOT_PATH + "\\roug_" + idx + ".png";
            //Trace.WriteLine("Create Diff File : " + diffFilePath + ", " + normFilePath);

            //int x = 0;
            //int y = 0;
            int w = bm.Width;
            int h = bm.Height;

            bm.Save(rougFilePath, ImageFormat.Png);
        }
        public void Generate3DImageTest()
        {

            if (canvas == null || canvas.Width <= 0)
            {
                return;
            }
            Stopwatch stopwatch = new Stopwatch();


            //flag.MakeTransparent(Color.Transparent);

            diffFilePath = MainForm.WWWROOT_PATH + "\\diff_" + idx + ".png";
            normFilePath = MainForm.WWWROOT_PATH + "\\norm_" + idx + ".png";
            //Trace.WriteLine("Create Diff File : " + diffFilePath + ", " + normFilePath);

            //int x = 0;
            //int y = 0;
            int w = canvas.Width;
            int h = canvas.Height;

            canvas.Save(diffFilePath, ImageFormat.Png);
            stopwatch.Start();
            GenerateNormalMapTest(diffFilePath, normFilePath);//노멀맵 생성시 속도 향상 대신 품질 떨어짐


            stopwatch.Stop();
            Trace.WriteLine("Generate3D NormalMap Elapsed Time is : " + stopwatch.ElapsedMilliseconds);
        }

        unsafe private void GenerateNormalMap()
        {
            int x = 0, y = 0, kx = 0, ky = 0, src_stride = 0, dst_stride = 0, sumX = 128, sumY = 128, sumZ = 0;
            PixelData* p_src = null, p_dst = null;
            int[,] kernelX = new int[,] { { 1, 2, 1 }, { 0, 0, 0 }, { -1, -2, -1 } };
            int[,] kernelY = new int[,] { { 1, 0, -1 }, { 2, 0, -2 }, { 1, 0, -1 } };
            int[,] offsetX = new int[,] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };
            int[,] offsetY = new int[,] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };

            System.Drawing.Imaging.BitmapData bd_src = m_bmp_image.LockBits(new Rectangle(0, 0, m_bmp_image.Width, m_bmp_image.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            System.Drawing.Imaging.BitmapData bd_dst = m_bmp_nmap.LockBits(new Rectangle(0, 0, m_bmp_nmap.Width, m_bmp_nmap.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            p_src = (PixelData*)bd_src.Scan0.ToPointer();
            p_dst = (PixelData*)bd_dst.Scan0.ToPointer();

            src_stride = bd_src.Stride / 4;
            dst_stride = bd_dst.Stride / 4;

            for (y = 0; y < bd_src.Height; y++)
            {
                // Set Y offsets for image boundaries
                if (y == 0)
                {
                    offsetY[0, 0] = (bd_src.Height - 1) * src_stride;
                    offsetY[1, 0] = (bd_src.Height - 1) * src_stride;
                    offsetY[2, 0] = (bd_src.Height - 1) * src_stride;
                    offsetY[0, 2] = 0;
                    offsetY[1, 2] = 0;
                    offsetY[2, 2] = 0;
                }
                else if (y == bd_src.Height - 1)
                {
                    offsetY[0, 0] = 0;
                    offsetY[1, 0] = 0;
                    offsetY[2, 0] = 0;
                    offsetY[0, 2] = -((bd_src.Height - 1) * src_stride);
                    offsetY[1, 2] = -((bd_src.Height - 1) * src_stride);
                    offsetY[2, 2] = -((bd_src.Height - 1) * src_stride);
                }
                else if (y == 1 || y == bd_src.Height - 2)
                {
                    offsetY[0, 0] = 0;
                    offsetY[1, 0] = 0;
                    offsetY[2, 0] = 0;
                    offsetY[0, 2] = 0;
                    offsetY[1, 2] = 0;
                    offsetY[2, 2] = 0;
                }

                for (x = 0; x < bd_src.Width; x++)
                {
                    (p_dst + (y * dst_stride) + x)->a = 255;

                    // Set X offsets for image boundaries
                    if (x == 0)
                    {
                        offsetX[0, 0] = bd_src.Width - 1;
                        offsetX[0, 1] = bd_src.Width - 1;
                        offsetX[0, 2] = bd_src.Width - 1;
                        offsetX[2, 0] = 0;
                        offsetX[2, 1] = 0;
                        offsetX[2, 2] = 0;
                    }
                    else if (x == bd_src.Width - 1)
                    {
                        offsetX[0, 0] = 0;
                        offsetX[0, 1] = 0;
                        offsetX[0, 2] = 0;
                        offsetX[2, 0] = -bd_src.Width;
                        offsetX[2, 1] = -bd_src.Width;
                        offsetX[2, 2] = -bd_src.Width;
                    }
                    else if (x == 1 || x == bd_src.Width - 2)
                    {
                        offsetX[0, 0] = 0;
                        offsetX[0, 1] = 0;
                        offsetX[0, 2] = 0;
                        offsetX[2, 0] = 0;
                        offsetX[2, 1] = 0;
                        offsetX[2, 2] = 0;
                    }

                    sumX = 128;
                    for (kx = -1; kx <= 1; kx++)
                        for (ky = -1; ky <= 1; ky++)
                            sumX += kernelX[kx + 1, ky + 1] * (((p_src + (y * src_stride) + x + (ky * src_stride) + kx + offsetX[kx + 1, ky + 1] + offsetY[kx + 1, ky + 1])->r + (p_src + (y * src_stride) + x + (ky * src_stride) + kx + offsetX[kx + 1, ky + 1] + offsetY[kx + 1, ky + 1])->g + (p_src + (y * src_stride) + x + (ky * src_stride) + kx + offsetX[kx + 1, ky + 1] + offsetY[kx + 1, ky + 1])->b) / 3);

                    sumY = 128;
                    for (kx = -1; kx <= 1; kx++)
                        for (ky = -1; ky <= 1; ky++)
                            sumY += kernelY[kx + 1, ky + 1] * (((p_src + (y * src_stride) + x + (ky * src_stride) + kx + offsetX[kx + 1, ky + 1] + offsetY[kx + 1, ky + 1])->r + (p_src + (y * src_stride) + x + (ky * src_stride) + kx + offsetX[kx + 1, ky + 1] + offsetY[kx + 1, ky + 1])->g + (p_src + (y * src_stride) + x + (ky * src_stride) + kx + offsetX[kx + 1, ky + 1] + offsetY[kx + 1, ky + 1])->b) / 3);

                    // Assign clamped X sum to R channel
                    if (sumX < 0)
                        (p_dst + (y * dst_stride) + x)->r = 0;
                    else if (sumX > 255)
                        (p_dst + (y * dst_stride) + x)->r = 255;
                    else
                        (p_dst + (y * dst_stride) + x)->r = (Byte)sumX;

                    // Assign clamped Y sum to G channel
                    if (sumY < 0)
                        (p_dst + (y * dst_stride) + x)->g = 0;
                    else if (sumY > 255)
                        (p_dst + (y * dst_stride) + x)->g = 255;
                    else
                        (p_dst + (y * dst_stride) + x)->g = (Byte)sumY;

                    // Calculate and assign B channel data
                    sumZ = ((Math.Abs(sumX - 128) + Math.Abs(sumY - 128)) / 4);
                    if (sumZ < 0)
                        sumZ = 0;
                    if (sumZ > 64)
                        sumZ = 64;
                    (p_dst + (y * dst_stride) + x)->b = (Byte)(255 - (Byte)sumZ);
                }
            }

            m_bmp_image.UnlockBits(bd_src);
            m_bmp_nmap.UnlockBits(bd_dst);
        }
        unsafe private void BoxBlur(Bitmap bmpSrc, Bitmap bmpDst)
        {
            int x = 0, y = 0, kx = 0, ky = 0, src_stride = 0, dst_stride = 0;
            float sum = 0f;
            PixelData* p_src = null, p_dst = null;
            float[,] kernel = new float[,] { { 1f / 9f, 1f / 9f, 1f / 9f }, { 1f / 9f, 1f / 9f, 1f / 9f }, { 1f / 9f, 1f / 9f, 1f / 9f } };
            int[,] offsetX = new int[,] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };
            int[,] offsetY = new int[,] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };

            System.Drawing.Imaging.BitmapData bd_src = bmpSrc.LockBits(new Rectangle(0, 0, bmpSrc.Width, bmpSrc.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            System.Drawing.Imaging.BitmapData bd_dst = bmpDst.LockBits(new Rectangle(0, 0, bmpDst.Width, bmpDst.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            p_src = (PixelData*)bd_src.Scan0.ToPointer();
            p_dst = (PixelData*)bd_dst.Scan0.ToPointer();

            src_stride = bd_src.Stride / 4;
            dst_stride = bd_dst.Stride / 4;

            for (y = 0; y < bd_src.Height; y++)
            {
                // Set Y offsets for image boundaries
                if (y == 0)
                {
                    offsetY[0, 0] = (bd_src.Height - 1) * src_stride;
                    offsetY[1, 0] = (bd_src.Height - 1) * src_stride;
                    offsetY[2, 0] = (bd_src.Height - 1) * src_stride;
                    offsetY[0, 2] = 0;
                    offsetY[1, 2] = 0;
                    offsetY[2, 2] = 0;
                }
                else if (y == bd_src.Height - 1)
                {
                    offsetY[0, 0] = 0;
                    offsetY[1, 0] = 0;
                    offsetY[2, 0] = 0;
                    offsetY[0, 2] = -((bd_src.Height - 1) * src_stride);
                    offsetY[1, 2] = -((bd_src.Height - 1) * src_stride);
                    offsetY[2, 2] = -((bd_src.Height - 1) * src_stride);
                }
                else if (y == 1 || y == bd_src.Height - 2)
                {
                    offsetY[0, 0] = 0;
                    offsetY[1, 0] = 0;
                    offsetY[2, 0] = 0;
                    offsetY[0, 2] = 0;
                    offsetY[1, 2] = 0;
                    offsetY[2, 2] = 0;
                }

                for (x = 0; x < bd_src.Width; x++)
                {
                    (p_dst + (y * dst_stride) + x)->a = 255;

                    // Set X offsets for image boundaries
                    if (x == 0)
                    {
                        offsetX[0, 0] = bd_src.Width - 1;
                        offsetX[0, 1] = bd_src.Width - 1;
                        offsetX[0, 2] = bd_src.Width - 1;
                        offsetX[2, 0] = 0;
                        offsetX[2, 1] = 0;
                        offsetX[2, 2] = 0;
                    }
                    else if (x == bd_src.Width - 1)
                    {
                        offsetX[0, 0] = 0;
                        offsetX[0, 1] = 0;
                        offsetX[0, 2] = 0;
                        offsetX[2, 0] = -bd_src.Width;
                        offsetX[2, 1] = -bd_src.Width;
                        offsetX[2, 2] = -bd_src.Width;
                    }
                    else if (x == 1 || x == bd_src.Width - 2)
                    {
                        offsetX[0, 0] = 0;
                        offsetX[0, 1] = 0;
                        offsetX[0, 2] = 0;
                        offsetX[2, 0] = 0;
                        offsetX[2, 1] = 0;
                        offsetX[2, 2] = 0;
                    }

                    // Calculate R sum
                    sum = 0f;
                    for (kx = -1; kx <= 1; kx++)
                        for (ky = -1; ky <= 1; ky++)
                            sum += kernel[kx + 1, ky + 1] * (p_src + (y * src_stride) + x + (ky * src_stride) + kx + offsetX[kx + 1, ky + 1] + offsetY[kx + 1, ky + 1])->r;
                    if (sum < 0)
                        sum = 0;
                    if (sum > 255)
                        sum = 255;
                    (p_dst + (y * dst_stride) + x)->r = (Byte)sum;

                    // Calculate G sum
                    sum = 0f;
                    for (kx = -1; kx <= 1; kx++)
                        for (ky = -1; ky <= 1; ky++)
                            sum += kernel[kx + 1, ky + 1] * (p_src + (y * src_stride) + x + (ky * src_stride) + kx + offsetX[kx + 1, ky + 1] + offsetY[kx + 1, ky + 1])->g;
                    if (sum < 0)
                        sum = 0;
                    if (sum > 255)
                        sum = 255;
                    (p_dst + (y * dst_stride) + x)->g = (Byte)sum;

                    // Calculate B sum
                    sum = 0f;
                    for (kx = -1; kx <= 1; kx++)
                        for (ky = -1; ky <= 1; ky++)
                            sum += kernel[kx + 1, ky + 1] * (p_src + (y * src_stride) + x + (ky * src_stride) + kx + offsetX[kx + 1, ky + 1] + offsetY[kx + 1, ky + 1])->b;
                    if (sum < 0)
                        sum = 0;
                    if (sum > 255)
                        sum = 255;
                    (p_dst + (y * dst_stride) + x)->b = (Byte)sum;
                }
            }

            bmpSrc.UnlockBits(bd_src);
            bmpDst.UnlockBits(bd_dst);
        }

        // 원본 이미지를 반전하여 길이 방향으로 추가
        // 필요없는 코드
        private Bitmap CreateYarnImage(Bitmap src)
        {
            Bitmap temp = new Bitmap(src.Width * 2, src.Height);
            Bitmap mirrored = new Bitmap(src);
            mirrored.RotateFlip(RotateFlipType.RotateNoneFlipX);

            Graphics g = Graphics.FromImage(temp);
            g.DrawImage(src, 0, 0, src.Width, src.Height);
            g.DrawImage(mirrored, src.Width, 0, src.Width, src.Height);
            mirrored.Dispose();
            return temp;
        }



        ///////////////////////////////////////////////////////////////////////
        //
        ///////////////////////////////////////////////////////////////////////
        float GetLuminance(float[] v)
        {
            return v[0] * 0.212f + v[1] * 0.716f + v[2] * 0.072f;
        }

        float[] ColorToFloats(Color color)
        {
            float[] colorF = new float[3];

            colorF[0] = (float)color.R / 255.0f;
            colorF[1] = (float)color.G / 255.0f;
            colorF[2] = (float)color.B / 255.0f;

            return colorF;
        }
        static float[] ColorToFloats(byte b, byte g, byte r)
        {
            float[] colorF = new float[3];

            colorF[0] = (float)r / 255.0f;
            colorF[1] = (float)g / 255.0f;
            colorF[2] = (float)b / 255.0f;

            return colorF;
        }

        Color FloatsToColor(float[] v)
        {
            byte r = (byte)(255.0f * v[0]);
            byte g = (byte)(255.0f * v[1]);
            byte b = (byte)(255.0f * v[2]);

            return Color.FromArgb(255, r, g, b);
        }

        float[] Add(float[] v1, float[] v2)
        {
            float[] v = new float[3];

            v[0] = v1[0] + v2[0];
            v[1] = v1[1] + v2[1];
            v[2] = v1[2] + v2[2];

            return v;
        }

        float[] Cross(float[] v1, float[] v2)
        {
            float[] v = new float[3];

            v[0] = v1[1] * v2[2] - v1[2] * v2[1];
            v[1] = v1[2] * v2[0] - v1[0] * v2[2];
            v[2] = v1[0] * v2[1] - v1[1] * v2[0];

            return v;
        }

        float[] Normalize(float[] v)
        {
            float length = (float)Math.Sqrt(v[0] * v[0] + v[1] * v[1] + v[2] * v[2]);
            float[] v2 = new float[3];

            v2[0] = v[0] / length;
            v2[1] = v[1] / length;
            v2[2] = v[2] / length;

            return v2;
        }

        int Clamp(int value, int max)
        {
            if (value < 0)
                value = 0;
            if (value > max)
                value = max;

            return value;
        }

        unsafe public void GenerateNormalMapTest(string src, string dest)
        {
            if (canvas == null)
            {
                return;
            }


            Bitmap colorBitmap = canvas;
            int W = colorBitmap.Width;
            int H = colorBitmap.Height;
            Bitmap grayBitmap = new Bitmap(W, H);

            BitmapData cBd = colorBitmap.LockBits(
                    new System.Drawing.Rectangle(0, 0, W, H),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            int cStride = cBd.Stride;
            int cOffset = cStride - W * Bitmap.GetPixelFormatSize(colorBitmap.PixelFormat) / 8;

            BitmapData gBd = grayBitmap.LockBits(
                    new System.Drawing.Rectangle(0, 0, W, H),
                    System.Drawing.Imaging.ImageLockMode.WriteOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            int gStride = gBd.Stride;
            int gOffset = gStride - W * Bitmap.GetPixelFormatSize(grayBitmap.PixelFormat) / 8;

            byte* cPtr = (byte*)cBd.Scan0;
            byte* gPtr = (byte*)gBd.Scan0;


            for (int y = 0; y < H; y++)
            {
                for (int x = 0; x < W; x++)
                {

                    byte b = cPtr[0];
                    byte g = cPtr[1];
                    byte r = cPtr[2];
                    byte a = cPtr[3];

                    //gPtr[0] = b;
                    //gPtr[1] = g;
                    //gPtr[2] = r;
                    //gPtr[3] = a;


                    float[] centerColorF = ColorToFloats(b, g, r);

                    int kx = Clamp(x - 1, W - 1);
                    int ky = 0;
                    kx = x - kx;
                    byte lb = (cPtr - (kx * 4))[0];
                    byte lg = (cPtr - (kx * 4))[1];
                    byte lr = (cPtr - (kx * 4))[2];
                    byte la = (cPtr - (kx * 4))[3];
                    float[] leftColorF = ColorToFloats(lb, lg, lr);

                    kx = Clamp(x + 1, W - 1);
                    ky = 0;
                    kx = kx - x;
                    byte rb = (cPtr + (kx * 4))[0];
                    byte rg = (cPtr + (kx * 4))[1];
                    byte rr = (cPtr + (kx * 4))[2];
                    byte ra = (cPtr + (kx * 4))[3];
                    float[] rightColorF = ColorToFloats(rb, rg, rr);

                    kx = 0;
                    ky = Clamp(y - 1, H - 1);
                    ky = y - ky;
                    byte bb = (cPtr - (ky * cStride))[0];
                    byte bg = (cPtr - (ky * cStride))[1];
                    byte br = (cPtr - (ky * cStride))[2];
                    byte ba = (cPtr - (ky * cStride))[3];
                    float[] bottomColorF = ColorToFloats(bb, bg, br);

                    kx = 0;
                    ky = Clamp(y + 1, H - 1);
                    ky = ky - y;
                    byte tb = (cPtr + (ky * cStride))[0];
                    byte tg = (cPtr + (ky * cStride))[1];
                    byte tr = (cPtr + (ky * cStride))[2];
                    byte ta = (cPtr + (ky * cStride))[3];
                    float[] topColorF = ColorToFloats(tb, tg, tr);


                    float scale = 5;//scale

                    float centerL = scale * GetLuminance(centerColorF);
                    float leftL = scale * GetLuminance(leftColorF);
                    float rightL = scale * GetLuminance(rightColorF);
                    float bottomL = scale * GetLuminance(bottomColorF);
                    float topL = scale * GetLuminance(topColorF);

                    float[] tangent1 = new float[3] { 1.0f, 0.0f, centerL - leftL };
                    float[] tangent2 = new float[3] { 1.0f, 0.0f, rightL - centerL };
                    float[] bitangent1 = new float[3] { 0.0f, 1.0f, centerL - bottomL };
                    float[] bitangent2 = new float[3] { 0.0f, 1.0f, topL - centerL };

                    tangent1 = Normalize(tangent1);
                    tangent2 = Normalize(tangent2);
                    bitangent1 = Normalize(bitangent1);
                    bitangent2 = Normalize(bitangent2);
                    float[] tangent = Add(tangent1, tangent2);
                    float[] bitangent = Add(bitangent1, bitangent2);

                    float[] normal = Cross(tangent, bitangent);
                    normal = Normalize(normal);

                    normal[0] = 0.5f * normal[0] + 0.5f;
                    normal[1] = 0.5f * normal[1] + 0.5f;
                    normal[2] = 0.5f * normal[2] + 0.5f;


                    gPtr[0] = (byte)(255.0f * normal[2]);
                    gPtr[1] = (byte)(255.0f * normal[1]);
                    gPtr[2] = (byte)(255.0f * normal[0]);
                    gPtr[3] = 255;


                    cPtr += 4;
                    gPtr += 4;
                }

                cPtr += cOffset;
                gPtr += gOffset;
            }


            colorBitmap.UnlockBits(cBd);
            grayBitmap.UnlockBits(gBd);

            grayBitmap.Save(dest);
            grayBitmap.Dispose();
        }
    }





    struct PixelData
    {
        public Byte b;
        public Byte g;
        public Byte r;
        public Byte a;
    }


    struct BoxData
    {
        public int Data;
        public string warpKey;
        public string weftKey;
        public Color warpColor;
        public Color weftColor;
        public int warpWeight;
        public int weftWeight;

        public Color warpShinyColor;
        public Color weftShinyColor;
        public string warpShinyKey;
        public string weftShinyKey;
    }
}
