using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyValueLibary
{
    /// <summary>
    /// 文件名：StringEx
    /// 作者：Bright
    /// 日期：2017/4/6 21:12:39
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
    /// <summary>
    /// 保存xmlnode值得类，xml转换为字典对象中的叶子对象
    /// </summary>
    public class StringEx : IAttr
    {
        public string Str
        {
            get;
            set;
        }

        private Dictionary<string, string> attr = new Dictionary<string, string>();
        public Dictionary<string, string> Attr
        {
            get
            {
                return attr;
            }

        }

        public StringEx( string s = null )
        {
            Str = s;
        }

        public override string ToString()
        {
            return Str;
        }

        public Dictionary<string, string> GetAttr()
        {
            return attr;

        }
    }

}
