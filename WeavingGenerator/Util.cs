using DevExpress.Export.Xl;
using DevExpress.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace WeavingGenerator
{
    internal class Util
    {

        public static string ToDateHuman(string date)
        {
            if (string.IsNullOrEmpty(date)) return "";
            DateTime dt = DateTime.ParseExact(date, "yyyyMMddHHmmss", null);
            return dt.ToString();
        }

        public static int ToInt(string v)
        {
            return Util.ToInt(v, 0);
        }
        public static int ToInt(string v, int initData)
        {
            if (string.IsNullOrEmpty(v)) return initData;

            int i = 0;
            bool result = int.TryParse(v, out i);
            if (result == false) return initData;

            return i;
        }

        public static Color ToColor(string hexColor)
        {
            if (string.IsNullOrEmpty(hexColor)) return Color.Black;
            if(hexColor.Length < 6) return Color.Black;
            int r = Convert.ToInt32(hexColor.Substring(0, 2), 16);
            int g = Convert.ToInt32(hexColor.Substring(2, 2), 16);
            int b = Convert.ToInt32(hexColor.Substring(4, 2), 16);
            return Color.FromArgb(r, g, b);
        }

        public static string ToHexColor(Color c)
        {
            string hex = c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
            return hex;
        }
        public static string JObjectToString(JObject obj, string name)
        {
            if (obj == null) return "";

            Object temp = obj[name];
            if (temp == null) return "";

            string str = temp.ToString();
            return str;
        }
        public static int JObjectToInt(JObject obj, string name)
        {
            return JObjectToInt(obj, name, 0);
        }
        public static int JObjectToInt(JObject obj, string name, int initData)
        {
            if (obj == null) return initData;

            Object temp = Util.JObjectToString(obj, name);
            if (temp == null) return initData;

            string str = (string)temp;
            if (string.IsNullOrEmpty(str)) return initData;

            return Convert.ToInt32(str);
        }

        public static System.Drawing.Image GetYarnResource(string strResName)
        {

            Assembly assembly = Assembly.GetExecutingAssembly();
            string strBaseName = assembly.GetName().Name + "." + "Properties.Resources";
            // strBaseName = "csharp_ResourceTest.Properties.Resources";
            ResourceManager rm = new ResourceManager(strBaseName, assembly);

            string temp = strResName.ToUpper();
            if (string.Equals(temp, "FILAMENT"))
            {
                return (System.Drawing.Image)rm.GetObject("FILAMENT");
            }
            else if (string.Equals(temp, "DTY"))
            {
                return (System.Drawing.Image)rm.GetObject("DTY");
            }
            else if (string.Equals(temp, "ATY"))
            {
                return (System.Drawing.Image)rm.GetObject("ATY");
            }
            else if (string.Equals(temp, "ITY"))
            {
                return (System.Drawing.Image)rm.GetObject("ITY");
            }
            else if (string.Equals(temp, "ITY_S"))
            {
                return (System.Drawing.Image)rm.GetObject("ITY_S");
            }
            else if (string.Equals(temp, "ITY_Z"))
            {
                return (System.Drawing.Image)rm.GetObject("ITY_Z");
            }
            else
            {
                return (System.Drawing.Image)rm.GetObject("yarn02");
            }
        }
        public static System.Drawing.Image GetYarnBitmapResource(string strResName)
        {

            Assembly assembly = Assembly.GetExecutingAssembly();
            string strBaseName = assembly.GetName().Name + "." + "Properties.Resources";
            // strBaseName = "csharp_ResourceTest.Properties.Resources";
            ResourceManager rm = new ResourceManager(strBaseName, assembly);

            string temp = strResName.ToUpper();
            if (string.Equals(temp, "FILAMENT"))
            {
                return (System.Drawing.Image)rm.GetObject("FILAMENT_1");
            }
            else if (string.Equals(temp, "DTY"))
            {
                return (System.Drawing.Image)rm.GetObject("DTY_1");
            }
            else if (string.Equals(temp, "ATY"))
            {
                return (System.Drawing.Image)rm.GetObject("ATY_1");
            }
            else if (string.Equals(temp, "ITY"))
            {
                return (System.Drawing.Image)rm.GetObject("ITY_1");
            }
            else
            {
                return (System.Drawing.Image)rm.GetObject("DEFAULT_1");
            }
        }

        public static string GenerateUUID()
        {
            Guid uudi = Guid.NewGuid();
            string strUUID = uudi.ToString();
            Trace.WriteLine("strUUID : " + strUUID);
            return strUUID;
        }

        public static int ToDenier(string v, string unit)
        {
            int denier = 0;
            if(string.IsNullOrEmpty(v))
            {
                return 0;
            }
            if(unit == "Dtex")
            {
                int dtex = ToInt(v, 0);
                if (dtex == 0)
                {
                    return 0;
                }
                denier = (int)(0.9 * dtex);
                return denier;
            }
            else if (unit == "Ne")
            {
                int ne = ToInt(v, 0);
                if (ne == 0)
                {
                    return 0;
                }
                denier = (int)(5315 / ne);
                return denier;
            }
            else if (unit == "Nm")
            {
                int nm = ToInt(v, 0);
                if (nm == 0)
                {
                    return 0;
                }
                denier = (int)(9000 / nm);
                return denier;
            }
            else if (unit == "Lea")
            {
                int lea = ToInt(v, 0);
                if (lea == 0)
                {
                    return 0;
                }
                denier = (int)(14882 / lea);
                return denier;
            }
            else
            {
                denier = ToInt(v, 0);
                return denier;
            }
        }

        public static string EscapeStringValue(string value)
        {
            const char BACK_SLASH = '\\';
            const char SLASH = '/';
            const char DBL_QUOTE = '"';

            var output = new StringBuilder(value.Length);
            foreach (char c in value)
            {
                switch (c)
                {
                    case SLASH:
                        output.AppendFormat("{0}{1}", BACK_SLASH, SLASH);
                        break;

                    case BACK_SLASH:
                        output.AppendFormat("{0}{0}", BACK_SLASH);
                        break;

                    case DBL_QUOTE:
                        output.AppendFormat("{0}{1}", BACK_SLASH, DBL_QUOTE);
                        break;

                    default:
                        output.Append(c);
                        break;
                }
            }

            return output.ToString();
        }

        public static string Base64Encode(string data)
        {
            if(data == null || data.Length == 0)
            {
                return "";
            }
            try
            {
                byte[] encData_byte = new byte[data.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(data);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception e)
            {
                throw new Exception("Error in Base64Encode: " + e.Message);
            }
        }
        public static string Base64Decode(string data)
        {
            if (data == null || data.Length == 0)
            {
                return "";
            }
            try
            {
                System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
                System.Text.Decoder utf8Decode = encoder.GetDecoder();

                byte[] todecode_byte = Convert.FromBase64String(data);
                int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
                char[] decoded_char = new char[charCount];
                utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
                string result = new String(decoded_char);
                return result;
            }
            catch (Exception e)
            {
                throw new Exception("Error in Base64Decode: " + e.Message);
            }
        }




        ///////////////////////////////////////////////////////////////////////
        // 시작 - Database 입력시 특수 문자 입력
        ///////////////////////////////////////////////////////////////////////
        public static string AddSlashes(string InputTxt)
        {
            // \047 single quote
            
            string Result = InputTxt;
            try
            {
                Result = InputTxt.Replace("'", "''");
            }
            catch (Exception Ex)
            {
                // handle any exception here
                Console.WriteLine(Ex.Message);
            }
            return Result;
        }
        public static string StripSlashes(string InputTxt)
        {
            // \047 single quote

            string Result = InputTxt;

            try
            {
                Result = InputTxt.Replace("''", "'");
            }
            catch (Exception Ex)
            {
                // handle any exception here
                Console.WriteLine(Ex.Message);
            }
            return Result;
        }
        ///////////////////////////////////////////////////////////////////////
        // 끝 - Database 입력시 특수 문자 입력
        ///////////////////////////////////////////////////////////////////////


    }
}
