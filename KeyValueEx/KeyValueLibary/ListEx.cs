using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyValueLibary
{
    /// <summary>
    /// 文件名：ListEx
    /// 作者：Bright
    /// 日期：2017/5/24 11:56:13
    /// 修改记录：
    ///         R1:
    ///             作者   
    ///             日期     
    ///             原因
    ///         R2:
    ///             作者   
    ///             日期     
    ///             原因
    /// </summary>
    public class ListEx<T> : List<T>, IAttr
    {
        private string son_name = "Item";
        public string SonName
        {
            get
            {
                return son_name;
            }
            set
            {
                son_name = value;
            }
        }

        private Dictionary<string, string> attr = new Dictionary<string, string>();
        public Dictionary<string, string> Attr
        {
            get
            {
                return attr;
            }

        }

        public ListEx()
            : base()
        {
        }

        public ListEx( string _name )
            : base()
        {
            son_name = _name;
        }

    }
}
