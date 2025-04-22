using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeavingGenerator
{
    [DefaultPropertyAttribute("Name")]
    internal class PropertyWeaving
    {
        private string _name;
        private string _pattern;
        private int _warpLength = 20;
        private int _weftLength = 20;
        private int _scale = 2;
        /*
        private DateTime _dateOfCreate;
        private Color _color;
        */
        public PropertyWeaving()
        {

        }

        [CategoryAttribute("직물 기본 정보"), 
            DisplayNameAttribute("이름"), 
            DescriptionAttribute("Name of the customer")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [CategoryAttribute("직물 기본 정보"),
            DisplayNameAttribute("패턴"),
            DescriptionAttribute("패턴")]
        [TypeConverter(typeof(PatternConverter))]
        public string Pattern
        {
            get { return _pattern; }
            set { _pattern = value; }
        }

        [CategoryAttribute("직물 기본 정보"),
            DisplayNameAttribute("경도"),
            DescriptionAttribute("경도 설정")]
        public int WarpLength
        {
            get { return _warpLength; }
            set { _warpLength = value; }
        }

        [CategoryAttribute("직물 기본 정보"),
            DisplayNameAttribute("위도"),
            DescriptionAttribute("위도 설정")]
        public int WeftLength
        {
            get { return _weftLength; }
            set { _weftLength = value; }
        }

        [CategoryAttribute("직물 기본 정보"),
            DisplayNameAttribute("확대/축소"),
            DescriptionAttribute("확대/축소")]
        public int Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }
        /*
        [CategoryAttribute("ID Settings"), DescriptionAttribute("Name of the customer")]
        public DateTime DateOfCreate
        {
            get { return _dateOfCreate; }
            set { _dateOfCreate = value; }
        }

        [CategoryAttribute("Marketting Settings"), DescriptionAttribute("ZZZZ")]
        public Color ThreadColor
        {
            get { return _color; }
            set { _color = value; }
        }
        */
    }
    class PatternConverter : StringConverter
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////// Field
        ////////////////////////////////////////////////////////////////////////////////////////// Private

        #region Field

        /// <summary>
        /// 주소 배열
        /// </summary>
        private string[] addressArray = new string[]
        {
            "Plain weave",
            "Twill",
            "Satin"
        };

        #endregion

        //////////////////////////////////////////////////////////////////////////////////////////////////// Method
        ////////////////////////////////////////////////////////////////////////////////////////// Public

        #region 표준 값 지원 여부 구하기 - GetStandardValuesSupported(context)

        /// <summary>
        /// 표준 값 지원 여부 구하기
        /// </summary>
        /// <param name="context">컨텍스트</param>
        /// <returns>표준 값 지원 여부</returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        #endregion
        #region 표준 값 단독 여부 구하기 - GetStandardValuesExclusive(context)

        /// <summary>
        /// 표준 값 단독 여부 구하기
        /// </summary>
        /// <param name="context">컨텍스트</param>
        /// <returns>표준 값 단독 여부</returns>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        #endregion
        #region 표준 값 구하기 - GetStandardValues(context)

        /// <summary>
        /// 표준 값 구하기
        /// </summary>
        /// <param name="context">컨텍스트</param>
        /// <returns>표준 값</returns>
        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(this.addressArray);
        }

        #endregion
    }
}
