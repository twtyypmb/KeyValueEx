using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace KeyValueLibary
{
    /// <summary>
    /// 文件名：KeyValueSerializer
    /// 作者：Bright
    /// 日期：2017/4/6 22:57:51
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
    internal class KeyValueConverter : JavaScriptConverter
    {
        public override IDictionary<string, object> Serialize( object obj, JavaScriptSerializer serializer )
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            if( obj is KeyValueEx )
            {
                KeyValueEx se = obj as KeyValueEx;
                foreach( var item in se.Keys )
                {
                    if( se[item] is StringEx )
                    {
                        dic[item] = se[item].ToString();
                    }
                    else if( se[item] is KeyValueEx )
                    {
                        dic[item] = Serialize( se[item], serializer );
                    }
                    else if( se[item] is StringExList )
                    {
                        StringExList temp_list = se[item] as StringExList;
                        if( temp_list != null )
                        {
                            dic[item] = ( from p in temp_list
                                          select p.Str ); 
                        }
                        
                    }
                    else if( se[item] is KeyValueExList )
                    {
                        KeyValueExList temp_list = se[item] as KeyValueExList;
                        if( temp_list != null )
                        {
                            dic[item] = ( from p in temp_list
                                          select p );
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return dic;
        }

        public override object Deserialize( IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer )
        {
            return null;
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return new Type[] { typeof( KeyValueEx ) };
            }
        }
    }
}
