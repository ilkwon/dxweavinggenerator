using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeavingGenerator
{
    public class CloFabric
    {
        public CloFabric() { }

        public int Search(byte[] src, int off, byte[] pattern)
        {
            if (src == null || pattern == null)
                return -1;

            int c = src.Length - pattern.Length + 1;
            int j;
            for (int i = 0; i < c; i++)
            {
                if (i < off)
                {
                    continue;
                }
                if (src[i] != pattern[0]) continue;
                for (j = pattern.Length - 1; j >= 1 && src[i + j] == pattern[j]; j--) ;
                if (j == 0) return i;
            }
            return -1;
        }
        public byte[] cvtStringToByte(string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }
        public string Byte2Hex(byte[] binary)
        {
            StringBuilder builder = new StringBuilder();
            foreach (byte num in binary)
            {
                if (num > 15)
                {
                    builder.AppendFormat("{0:X}", num);
                }
                else
                {
                    builder.AppendFormat("0{0:X}", num);
                }
            }
            return builder.ToString();
        }
        public void SetV2Size(byte[] bytes, int w, int h)
        {
            float fw = (float)(w / 4.72);
            float fh = (float)(h / 4.72);

            byte[] bfw = System.BitConverter.GetBytes(fw);
            byte[] bfh = System.BitConverter.GetBytes(fh);
            //Array.Reverse(bfw);
            //Array.Reverse(bfh);
            log(Byte2Hex(bfw));
            log(Byte2Hex(bfh));


            int pos = 0;
            int n = 0;
            while (pos > -1)
            {
                pos = Search(bytes, pos + 1, cvtStringToByte("v2Size"));
                if (pos > -1)
                {
                    log("POS[" + (n++) + "]  : " + (pos));
                    bytes[pos + 6 + 0] = bfw[0];
                    bytes[pos + 6 + 1] = bfw[1];
                    bytes[pos + 6 + 2] = bfw[2];
                    bytes[pos + 6 + 3] = bfw[3];

                    bytes[pos + 6 + 4] = bfh[0];
                    bytes[pos + 6 + 5] = bfh[1];
                    bytes[pos + 6 + 6] = bfh[2];
                    bytes[pos + 6 + 7] = bfh[3];
                }
            }

            pos = 0;
            n = 0;
            while (pos > -1)
            {
                pos = Search(bytes, pos + 1, cvtStringToByte("v2PhysicalSize"));
                if (pos > -1)
                {
                    log("POS[" + (n++) + "]  : " + (pos));
                    bytes[pos + 14 + 0] = bfw[0];
                    bytes[pos + 14 + 1] = bfw[1];
                    bytes[pos + 14 + 2] = bfw[2];
                    bytes[pos + 14 + 3] = bfw[3];

                    bytes[pos + 14 + 4] = bfh[0];
                    bytes[pos + 14 + 5] = bfh[1];
                    bytes[pos + 14 + 6] = bfh[2];
                    bytes[pos + 14 + 7] = bfh[3];
                }
            }
        }
        public void log(string txt)
        {
            Trace.WriteLine(txt);
        }
        public void SetProperty(byte[] bytes, string name, float data)
        {
            byte[] bf = System.BitConverter.GetBytes(data);

            int pos = Search(bytes, 0, cvtStringToByte(name));
            if (pos < 0)
            {
                return;
            }

            int len = name.Length;
            bytes[pos + len + 0] = bf[0];
            bytes[pos + len + 1] = bf[1];
            bytes[pos + len + 2] = bf[2];
            bytes[pos + len + 3] = bf[3];
            log(name + " : " + data + " -> " + Byte2Hex(bf));
        }



        string cloFabFilePath    = "";
        string diffFilePath = "";
        string normFilePath = "";
        string exportFilePath = "";
        int imgWidth = 0;
        int imgHeight = 0;
        /**
         * 이름                             | 이름                  | 변수명                   | 기본값   | 최대값
         * ----------------------------------------------------------------------------------------------------------
         * Stretch-Weft                     스트레치 위사	        fSuK	                    1000 	    10000000 
         * Stretch-Warp	                    스트레치 경사	        fSvK	                    1000 	    10000000 
         * Shear (Right)	                스트레치 전단	        fHK	                        1000 	    10000000 
         * Shear (Left)	                    스트레치 전단	        fLeftShearK	                1000 	    10000000 
         * Bending-Weft	                    굽힘 강도 위사	        fBuK	                    0 	        2000000 
         * Bending-Warp	                    굽힘 강도 경사	        fBvK	                    0 	        2000000 
         * Bending-Bias (Right)	            굽힘 조정 	            fBhK	                    0 	        2000000 
         * Bending-Bias (Left)	            굽힘 조정	            fBLeftShearK	            0 	        2000000 
         * Buckling Ratio-Weft	            좌굴 비율 조정 위사	    fBuLR	                    0.00 	    1.00 
         * Buckling Ratio-Warp	            좌굴 비율 조정 경사	    fBvLR	                    0.00 	    1.00 
         * Buckling Ratio-Bias (Right)	    바이어스 좌굴 비율 조정 fBhLR	                    0.00 	    1.00 
         * Buckling Ratio-Bias (Left)	    바이어스 좌굴 비율 조정 fBLeftShearLR	            0.00 	    1.00 
         * Buckling Stiffness-Weft	        좌굴 강성 조정 위사	    fBucklingStiffnessU	        0.00 	    1.00 
         * Buckling Stiffness-Warp	        좌굴 강성 조정 경사	    fBucklingStiffnessV	        0.00 	    1.00 
         * Buckling Stiffness-Bias (Right)	바이어스 좌굴 강성 조정	fBucklingStiffnessH	        0.00 	    1.00 
         * Buckling Stiffness-Bias (Left)	바이어스 좌굴 강성 조정	fBucklingStiffnessLeftShear	0.00 	    1.00 
         * Internal Damping	                내부 댐핑 조정	        fIDS	                    0.000000 	0.010000 
         * Density	                        밀도	                fDensity	                30.00 	    2100.00 
         * Friction	                        마찰계수	            fFriction	                0.00 	    1.00 
         */
        float bendingWarp = 0;
        float bendingWeft = 0;
        float internalDamping = 0;
        float friction = 0;
        float density = 0;
        float stretchWeft = 0;
        float stretchWarp = 0;
        float bucklingStiffnessWeft = 0;
        float bucklingStiffnessWarp = 0;

        public void setFabricFilePath(string file)
        {
            cloFabFilePath = file;
        }
        public void setDiffFilePath(string file)
        {
            diffFilePath = file;
            System.Drawing.Image img = System.Drawing.Image.FromFile(diffFilePath);
            imgWidth = img.Width;
            imgHeight = img.Height;
        }
        public void setNormFilePath(string file)
        {
            normFilePath = file;
        }
        public void setExportFilePath(string file)
        {
            exportFilePath = file;
        }
        public void setPhysicalProperty(int bendingWeft,
            int bendingWarp,
            int internalDamping,
            int friction,
            int density,
            int stretchWeft,
            int stretchWarp,
            int bucklingStiffnessWeft,
            int bucklingStiffnessWarp)
        {
            //퍼센트를 물성 값으로 변환
            this.bendingWeft            = (float)((2000000 / 100) * bendingWeft);
            this.bendingWarp            = (float)((2000000 / 100) * bendingWarp);
            this.internalDamping        = (float)((0.010000 / 100) * internalDamping);
            this.friction               = (float)((1.00 / 100) * friction);
            this.density                = (float)((2070.00 / 100) * density); // 30 ~ 2100
            this.stretchWeft            = (float)((9999000 / 100) * stretchWeft); // 1000 ~ 10000000
            this.stretchWarp            = (float)((9999000 / 100) * stretchWarp); // 1000 ~ 10000000
            this.bucklingStiffnessWeft  = (float)((1.00 / 100) * bucklingStiffnessWeft);
            this.bucklingStiffnessWarp  = (float)((1.00 / 100) * bucklingStiffnessWarp);

            //최소값 설정
            this.density = this.density + 30;
            // 밀도는 x 0.000001 을 해야함
            this.density = (float)(this.density * 0.000001);

            this.stretchWeft = this.stretchWeft + 1000;
            this.stretchWarp = this.stretchWarp + 1000;
        }
        public string exportFabric()
        {
            log("cloFabFilePath : " + cloFabFilePath);
            log("diffFilePath : " + diffFilePath);
            log("normFilePath : " + normFilePath);
            log("imgWidth : " + imgWidth);
            log("imgHeight : " + imgHeight);


            byte[] bytes = File.ReadAllBytes(cloFabFilePath);

            ///////////////////////////////////////////////////////////////////
            /// 파일 사이즈 px
            ///////////////////////////////////////////////////////////////////
            SetV2Size(bytes, imgWidth, imgHeight);

            ///////////////////////////////////////////////////////////////////
            /// Stretch
            ///////////////////////////////////////////////////////////////////
            SetProperty(bytes, "fSuK", stretchWeft);   // 스트레치 위사
            SetProperty(bytes, "fSvK", stretchWarp);   // 스트레치 경사
            SetProperty(bytes, "fHK", 1000);    // 스트레치 전단 Right
            SetProperty(bytes, "fLeftShearK", 1000); // 스트레치 전단 Left

            ///////////////////////////////////////////////////////////////////
            /// Bending
            ///////////////////////////////////////////////////////////////////
            SetProperty(bytes, "fBuK", bendingWeft);      // 굽힘 강도 위사
            SetProperty(bytes, "fBvK", bendingWarp);    // 굽힘 강도 경사
            SetProperty(bytes, "fBhK", 0);    // 굽힘 조정 Right
            SetProperty(bytes, "fBLeftShearK", 0);   // 굽힘 조정 Left

            ///////////////////////////////////////////////////////////////////
            /// Buckling Ratio
            ///////////////////////////////////////////////////////////////////
            SetProperty(bytes, "fBuLR", 0.00f);     // 좌굴 비율 조정 위사
            SetProperty(bytes, "fBvLR", 0.00f);     // 좌굴 비율 조정 경사
            SetProperty(bytes, "fBhLR", 0.00f);     // 바이어스 좌굴 비율 조정 Right
            SetProperty(bytes, "fBLeftShearLR", 0.00f); // 바이어스 좌굴 비율 조정 Left

            ///////////////////////////////////////////////////////////////////
            /// Buckling Stiffness
            ///////////////////////////////////////////////////////////////////
            SetProperty(bytes, "fBucklingStiffnessU", bucklingStiffnessWeft);   // 좌굴 강성 조정 위사
            SetProperty(bytes, "fBucklingStiffnessV", bucklingStiffnessWarp);   // 좌굴 강성 조정 경사
            SetProperty(bytes, "fBucklingStiffnessH", 0.00f);   // 바이어스 좌굴 강성 조정 Right
            SetProperty(bytes, "fBucklingStiffnessLeftShear", 0.00f); // 바이어스 좌굴 강성 조정 Left

            ///////////////////////////////////////////////////////////////////
            /// Internal Damping
            ///////////////////////////////////////////////////////////////////
            SetProperty(bytes, "fIDS", internalDamping);  // 내부 댐핑 조정

            ///////////////////////////////////////////////////////////////////
            /// Internal Damping
            ///////////////////////////////////////////////////////////////////
            SetProperty(bytes, "fDensity", density);  // 밀도

            ///////////////////////////////////////////////////////////////////
            /// Friction
            ///////////////////////////////////////////////////////////////////
            SetProperty(bytes, "fFriction", friction);  // 마찰계수

            string aFileTemp1 = exportFilePath + "\\FABRIC.fab";
            File.WriteAllBytes(aFileTemp1, bytes);

            System.IO.File.Copy(diffFilePath, exportFilePath + "\\diff.png", true);
            System.IO.File.Copy(normFilePath, exportFilePath + "\\norm.png", true);

            string aFileTemp2 = exportFilePath + "\\diff.png";
            string aFileTemp3 = exportFilePath + "\\norm.png";

            string saveZipPath = exportFilePath + "\\FABRIC.ZFAB";
            using (var fs = new FileStream(saveZipPath, FileMode.Create, FileAccess.ReadWrite))
            {
                using (var za = new ZipArchive(fs, ZipArchiveMode.Create))
                {
                    za.CreateEntryFromFile(aFileTemp1, Path.GetFileName(aFileTemp1));
                    za.CreateEntryFromFile(aFileTemp2, Path.GetFileName(aFileTemp2));
                    za.CreateEntryFromFile(aFileTemp3, Path.GetFileName(aFileTemp3));
                }
            }

            System.IO.File.Delete(aFileTemp1);
            System.IO.File.Delete(aFileTemp2);
            System.IO.File.Delete(aFileTemp3);

            return saveZipPath;
        }
    }
}
