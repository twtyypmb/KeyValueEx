using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyValueLibary
{
    /// <summary>
    /// 文件名：IAttr
    /// 作者：Bright
    /// 日期：2017/4/6 21:12:00
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
    /// 属性接口
    /// </summary>
    internal interface IAttr
    {
        /// <summary>
        /// 获取该字典对象所对应的xmlnode的属性(XmlAttribute)
        /// </summary>
        /// <returns></returns>
        Dictionary<string, string> Attr
        {
            get;
        }
    }
}
