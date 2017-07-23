using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web.Script.Serialization;
using System.IO;
using System.Text.RegularExpressions;

namespace KeyValueLibary
{

    /// <summary>
    /// 用与xml和json字符串的操作实体类
    /// </summary>
    public class KeyValueEx : Dictionary<string,object>,ICloneable,IAttr
    {
        private char sqot = '"';

        /// <summary>
        /// json引号
        /// </summary>
        public char Sqot
        {
            get
            {
                return sqot;
            }
            set
            {
                sqot = value;
            }
        }

        private Dictionary<string, string> attr = new Dictionary<string, string>();

        /// <summary>
        /// 记录xml节点属性
        /// </summary>
        public Dictionary<string, string> Attr
        {
            get
            {
                return attr;
            }

        }

        public KeyValueEx()
            : base()
        {
            ListName = "List";
        }

        /// <summary>
        /// 列表名称，默认为List
        /// </summary>
        public string ListName
        {
            get;
            set;
        }

        /// <summary>
        /// 转换成XmlDocument对象
        /// </summary>
        /// <returns></returns>
        public XmlDocument ToXmlDocument()
        {
            XmlDocument xd = new XmlDocument();
            KeyValueExToXmlDocument( xd, this );
            return xd;
        }


        /// <summary>
        /// 转换成xml字符串
        /// </summary>
        /// <returns></returns>
        public string ToXmlString()
        {
            return XmlDocumentToString( ToXmlDocument() );
        }


        /// <summary>
        /// 只有简单类型、KeyValueEx、ValueList、ValueListEx赋值有效
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public new object this[string s]
        {
            get
            {
                return base[s];
            }
            set
            {
                decimal sdf = 0m;
                if( value == null )
                {

                    base[s] = new StringEx( null );
                    return;
                }

                if( value is string )
                {

                    base[s] = new StringEx( value + "" );
                    return;
                }

                if( decimal.TryParse( value + "", out sdf ) )
                {
                    base[s] = new StringEx( sdf + "" );
                    return;
                }

                if( value is StringExList || value is KeyValueExList || value is KeyValueEx )
                {
                    base[s] = value;
                }
            }
        }

        /// <summary>
        /// 根据键获取值，当获取错误时返回null
        /// </summary>
        /// <param name="o"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object GetValueByKey(object o,string key)
        {
            try
            {
                return (o as KeyValueEx)[key];
            }
            catch( Exception )
            {
                return null;
            }

        }

        //public void SetKeyValue(string key,string value)
        //{
        //    base[key] = value;
        //}

        //public KeyValueEx SetKeyValue( string key )
        //{
        //    KeyValueEx lex = new KeyValueEx();
        //    base[key] = lex;
        //    return lex;
        //}

        //public ValueListEx SetKeyList( string key )
        //{
        //    ValueListEx lex = new ValueListEx();
        //    base[key] = lex;
        //    return lex;
        //}

        #region json相关转换

        public static string ConvertObjectToJsonString( object obj )
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            jss.RegisterConverters( new JavaScriptConverter[] { new KeyValueConverter() } );

            return jss.Serialize( obj );
        }

        /// <summary>
        /// 转换成json字符串，输出是xml的特性将会被忽略，列表节点的子节点名将会被忽略
        /// <List attr="attr"><item>123</item><item>456</item></List> ----> "List":["123","456"]
        /// </summary>
        /// <returns></returns>
        public string ToJsonString()
        {
            int i = 0;
            string temp_node = null;

            // 同样的是对象转json
            temp_node = ConvertObjectToJsonString( this );

            //temp_node = KeyValueExToJsonStep( this,true );
            return temp_node;

        }


        





        private static string KeyValueExToJsonStep( KeyValueEx xn, bool b_first )
        {




            object temp = null;

            //用于返回的node
            string temp_node = null;
            string xun_node = null;
            decimal sdf = 0m;
            int i = 0;
            foreach( var item in xn )
            {

                if( item.Value is string || decimal.TryParse( item.Value + "", out sdf ) )
                {
                    temp_node = AddJsonNode( temp_node, string.Format( "{0}{1}{0}:{0}{2}{0}", xn.Sqot, item.Key.Replace( xn.Sqot + "", "\\" + xn.Sqot ), item.Value.ToString().Replace( xn.Sqot + "", "\\" + xn.Sqot ) ) );
                }
                else if( item.Value is KeyValueEx )
                {
                    temp_node = AddJsonNode( temp_node, string.Format( "{0}{1}{0}:{2}", xn.Sqot, item.Key.Replace( xn.Sqot + "", "\\" + xn.Sqot ), KeyValueExToJsonStep( item.Value as KeyValueEx, true ) ) );
                }
                else if( item.Value is KeyValueExList )
                {
                    xun_node = null;
                    int j = 0;
                    foreach( var item1 in item.Value as KeyValueExList )
                    {
                        if( j++ == 0 )
                        {
                            xun_node = KeyValueExToJsonStep( item1, true );
                        }
                        else
                        {
                            xun_node += "," + KeyValueExToJsonStep( item1, true );
                        }

                    }

                    temp_node = AddJsonNode( temp_node, string.Format( "{0}{1}{0}:[{2}]", xn.Sqot, item.Key.Replace( xn.Sqot + "", "\\" + xn.Sqot ), xun_node ) );
                }
                else if( item.Value is StringExList )
                {
                    xun_node = null;
                    int j = 0;
                    foreach( var item2 in item.Value as StringExList )
                    {
                        if( j++ == 0 )
                        {
                            xun_node = string.Format( "{0}{1}{0}", xn.Sqot, item2.ToString().Replace( xn.Sqot + "", "\\" + xn.Sqot ) );
                        }
                        else
                        {
                            xun_node += "," + string.Format( "{0}{1}{0}", xn.Sqot, item2.ToString().Replace( xn.Sqot + "", "\\" + xn.Sqot ) );
                        }

                    }

                    temp_node = AddJsonNode( temp_node, string.Format( "{0}{1}{0}:[{2}]", xn.Sqot, item.Key.Replace( xn.Sqot + "", "\\" + xn.Sqot ), xun_node ) );
                }


            }


            return "{" + temp_node + "}";

        }



        private static string AddJsonNode( string str, string node )
        {

            if( string.IsNullOrEmpty( str ) )
            {
                return node;
            }
            else
            {
                return str + "," + node;
            }
        }


        /// <summary>
        /// 转换json字符串为字典对象
        /// 只支持标准json，单引号不支持，不支持数字数组[123,245,456]，混合型数组[123,"234"]
        /// 值域是数字，会自动转成字符串
        /// 只保证正确的json能转换成功，错误的json不能转换成功，可能会抛异常
        /// </summary>
        /// <param name="json"></param>
        public bool JsonStringToKeyValueEx( string json )
        {
            string temp = json.Trim();
            if( string.IsNullOrEmpty( temp ) || temp[0] != '{' || temp.Last() != '}' )
            {
                return false;
            }
            this.Clear();
            this.attr.Clear();
            int i = 0;
            try
            {
                JsonStringToKeyValueExStep( this, temp, ref i );
            }
            catch( Exception e)
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// 递归转换json字符串为字典对象的单步运算
        /// 单步处理单元是一个json，以大括号开始
        /// </summary>
        /// <param name="kve">用于保存的字典对象</param>
        /// <param name="s">json字符串</param>
        /// <param name="now_index">待转换的字符串处理到何处的索引</param>
        private static void JsonStringToKeyValueExStep( KeyValueEx kve, string s, ref int now_index )
        {
            Stack<char> sc = new Stack<char>();
            string key = null;
            string value = null;

            // 键是否结束
            bool b_key = false;
            bool space_fliter = true;
            int i = now_index;
            for( ; i < s.Length; i++ )
            {
                // 如果栈上没有元素,一直忽略到第一个做大括号为止
                if( sc.Count > 0 )
                {
                    char stack_char = sc.Peek();
                    if( stack_char == '{' )
                    {
                        // 
                        if( s[i] == ' ' )
                        {
                            continue;
                        }

                        // 说明键开始了
                        if( s[i] == '"' )
                        {
                            sc.Push( s[i] );
                            b_key = false;
                            continue;
                        }

                        // 键值对区分的冒号
                        if( s[i] == ':' )
                        {
                            sc.Push( s[i] );
                            continue;
                        }

                        // 本次json未完成的情况下，又有左大括号
                        if( s[i] == '{' )
                        {
                            sc.Pop();
                            // 新的递归
                            kve[key] = new KeyValueEx();
                            JsonStringToKeyValueExStep( kve[key] as KeyValueEx, s, ref i );
                            key = null;
                            continue;
                        }

                        // 本次json未完成的情况下，又有左中括号
                        if( s[i] == '[' )
                        {
                            // 新的递归
                            int j = i;

                            // 试探这个数组
                            for( ; j < s.Length; j++ )
                            {
                                // 这个数组的元素是json
                                if( s[j] == '{' )
                                {
                                    i = j;
                                    KeyValueExList vle_temp = new KeyValueExList();
                                    kve[key] = vle_temp;
                                    for( ; i < s.Length; i++ )
                                    {
                                        // 达到右中括号，表示数组结束
                                        if( s[i] == ']' )
                                        {
                                            break;
                                        }

                                        // List中的json元素解析开始
                                        if( s[i] == '{' )
                                        {
                                            KeyValueEx kve_temp = new KeyValueEx();
                                            JsonStringToKeyValueExStep( kve_temp, s, ref i );
                                            vle_temp.Add( kve_temp );
                                        }
                                    }


                                    break;
                                }

                                // 这个数组的元素是字符串
                                if( s[j] == '"' )
                                {

                                    StringExList vl_temp = new StringExList();
                                    StringToStringExList( s, vl_temp, ref i );
                                    kve[key] = vl_temp;
                                    break;
                                }
                            }

                            // 解析完毕，清空
                            key = null;
                            b_key = false;
                            sc.Pop();
                            continue;
                        }

                        // 本次json结束，大循环退出
                        if( s[i] == '}' )
                        {
                            break;
                        }
                    }
                    else if( stack_char == '"' )
                    {
                        // 是否有转义字符？
                        if( s[i] == '\\' )
                        {
                            sc.Push( s[i] );
                            continue;
                        }

                        // 如果栈上和当前字符都是引号，说明键或值，完成了
                        if( s[i] == '"' )
                        {
                            sc.Pop();
                            if( b_key )
                            {
                                // 如果值是空字符串，value就为null，所以要加个空字符串
                                kve[key] = value+"";
                                key = value = null;
                            }
                            else
                            {
                                b_key = true;
                            }
                            continue;
                        }

                        // 引号中其他字符都是键或值得字符串
                        if( b_key )
                        {
                            value += s[i];
                        }
                        else
                        {
                            key += s[i];
                        }
                    }
                    else if( stack_char == '\\' )
                    {
                        // 转义字符，未完美解决
                        if( b_key )
                        {
                            //value += Convert.ToChar( "\\" + s[i] );
                            //value += char.Parse( "\\" + s[i] );
                            value += s[i];
                        }
                        else
                        {
                            //key += char.Parse( "\\" + s[i] );
                            value += s[i];
                        }
                        sc.Pop();
                    }
                    else if( stack_char == ':' )
                    {
                        
                        // 栈上是冒号，且当前字符是数字，说明值域开始了
                        if( s[i] >= '0' && s[i]<='9' )
                        {
                            sc.Pop();
                            // 以\a来表示数字
                            sc.Push( '\a' );
                            i--;
                            continue;
                        }

                        // 栈上是冒号，且当前字符是'n'，有可能是指null
                        if( s[i] == 'n' )
                        {
                            sc.Pop();
                            // 以\v来表示数字
                            sc.Push( '\v' );
                            i--;
                            continue;
                        }

                        // 栈上是冒号，且当前字符是引号，说明值域开始了
                        if( s[i] == '"' )
                        {
                            sc.Pop();
                            sc.Push( s[i] );
                            continue;
                        }

                        // 栈上是冒号，且当前字符是左大括号，新的子json开始了
                        if( s[i] == '{' )
                        {
                            sc.Pop();
                            // 新的递归
                            kve[key] = new KeyValueEx();
                            JsonStringToKeyValueExStep( kve[key] as KeyValueEx, s, ref i );
                            key = null;
                            continue;
                        }


                        // 栈上是冒号，且当前字符是左大括号，新的子数组开始了
                        if( s[i] == '[' )
                        {
                            // 新的递归
                            int j = i;

                            // 试探这个数组
                            for( ; j < s.Length; j++ )
                            {
                                // 这个数组的元素是json
                                if( s[j] == '{' )
                                {
                                    i = j;
                                    KeyValueExList vle_temp = new KeyValueExList();
                                    kve[key] = vle_temp;
                                    for( ; i < s.Length; i++ )
                                    {
                                        // 到了数组尾部
                                        if( s[i] == ']' )
                                        {
                                            break;
                                        }

                                        if( s[i] == '{' )
                                        {
                                            KeyValueEx kve_temp = new KeyValueEx();
                                            JsonStringToKeyValueExStep( kve_temp, s, ref i );
                                            vle_temp.Add( kve_temp );
                                        }
                                    }


                                    break;
                                }

                                // 这个数组的元素是字符串
                                if( s[j] == '"' )
                                {

                                    StringExList vl_temp = new StringExList();
                                    StringToStringExList( s, vl_temp, ref i );
                                    kve[key] = vl_temp;
                                    break;
                                }
                            }

                            // 清空
                            key = null;
                            b_key = false;
                            sc.Pop();
                            continue;
                        }
                    }
                    // 纯数字的值域
                    else if( stack_char == '\a' )
                    {
                        if( s[i] == ' ' )
                        {
                            continue;
                        }

                        // 如果栈上和当前字符都是\a，说明键或值，完成了
                        if( s[i] == ',' || s[i] == ']' || s[i] == '}' )
                        {
                            double d_temp = 0;
                            try
                            {
                                d_temp = double.Parse( value );
                            }
                            catch( Exception e)
                            {
                                
                                throw e;
                            }

                            sc.Pop();
                            if( b_key )
                            {
                                kve[key] = value;
                                key = value = null;
                            }
                            else
                            {
                                b_key = true;
                            }

                            if( s[i] == ','  )
                            {
                                continue;
                            }

                            if(s[i] == ']' ||s[i] == '}' )
                            {
                                break;
                            }
                        }

                        // 引号中其他字符都是键或值得字符串
                        if( b_key )
                        {
                            value += s[i];
                        }
                        else
                        {
                            key += s[i];
                        }
                    }// null
                    else if( stack_char == '\v' )
                    {
                        if( s[i] == ' ' )
                        {
                            continue;
                        }
                        // 如果栈上和当前字符都是\v，说明键或值，完成了
                        if( s[i] == ',' || s[i] == ']' || s[i] == '}' )
                        {
                            if( value != "null" )
                            {
                                throw new Exception( "构造null时出错" );
                            }
                            sc.Pop();
                            if( b_key )
                            {
                                kve[key] = null;
                                key = value = null;
                            }
                            else
                            {
                                b_key = true;
                            }
                            if( s[i] == ',' )
                            {
                                continue;
                            }

                            if( s[i] == ']' || s[i] == '}' )
                            {
                                break;
                            }
                        }

                        // 引号中其他字符都是键或值得字符串
                        if( b_key )
                        {
                            value += s[i];
                        }
                        else
                        {
                            key += s[i];
                        }
                    }


                }
                else if( s[i] == '{' )
                {
                    sc.Push( s[i] );
                }


            }
            now_index = i;
        }

        /// <summary>
        /// 转换字符串到list数组，格式["123","234","345"]
        /// </summary>
        /// <param name="s">字符串，包括中括号</param>
        /// <param name="vl">存放元素的list</param>
        /// <param name="now_index">当前字符串处理到的索引</param>
        public static void StringToStringExList( string s, StringExList vl, ref int now_index )
        {
            if( now_index >= s.Length )
            {
                return;
            }

            Stack<char> sc = new Stack<char>();
            string value = null;
            int i = now_index;
            for( ; i < s.Length; i++ )
            {
                // 如果栈未空，一直忽略到左中括号为止
                if( sc.Count == 0 && s[i] != '[' )
                {
                    continue;
                }
                else if( sc.Count == 0 && s[i] == '[' )
                {
                    sc.Push( s[i] );
                    continue;
                }
                else
                {
                    if( sc.Peek() == '[' )
                    {
                        // 暂只支持字符串的元素，说明元素开始了
                        if( s[i] == '"' )
                        {
                            sc.Push( s[i] );
                            continue;
                        }

                        if( s[i] == ',' )
                        {
                            sc.Push( s[i] );
                            continue;
                        }

                        // 数组转换结束
                        if( s[i] == ']' )
                        {
                            break;
                        }

                    }
                    else if( sc.Peek() == '"' )
                    {
                        // 转义字符开始
                        if( s[i] == '\\' )
                        {
                            sc.Push( s[i] );
                            continue;
                        }

                        // 元素的字符串结束了
                        if( s[i] == '"' )
                        {
                            vl.Add( new StringEx( value ) );
                            sc.Pop();
                            continue;
                        }

                        value += s[i];



                    }
                    else if( sc.Peek() == ',' )
                    {
                        // 逗号间隔符遇到引号，说明下一个元素开始了
                        if( s[i] == '"' )
                        {
                            sc.Pop();
                            sc.Push( s[i] );
                            value = null;
                            continue;
                        }
                    }
                    else if( sc.Peek() == '\\' )
                    {
                        sc.Pop();
                        value += s[i];
                    }
                    else
                    {
                    }
                }


            }
            now_index = i;
        }
        
        #endregion

        #region ICloneable 成员

        public object Clone()
        {
            KeyValueEx kve = new KeyValueEx();

            CloneStep( this, kve );
            return kve;
        }

        public KeyValueEx DepthClone()
        {
            return Clone() as KeyValueEx;
        }

        private void CloneStep( KeyValueEx kve, KeyValueEx des )
        {
            if( kve == null || des==null )
            {
                return;
            }

            foreach( var item in kve )
            {
                if( item.Value is KeyValueEx )
                {
                    des[item.Key] = new KeyValueEx();
                    CloneStep( item.Value as KeyValueEx, des[item.Key] as KeyValueEx );
                }
                else if( item.Value is KeyValueExList )
                {
                    KeyValueExList vle = item.Value as KeyValueExList;
                    KeyValueExList vle1= new KeyValueExList();
                    des[item.Key] = vle1;
                    foreach( var item1 in vle )
                    {
                        KeyValueEx des1 = new KeyValueEx();
                        CloneStep( item1, des1 );
                        vle1.Add( des1 );
                    }
                }
                else if( item.Value is StringExList )
                {
                    StringExList vl = item.Value as StringExList;
                    StringExList vl1 = new StringExList();
                    des[item.Key] = vl1;
                    foreach( var item2 in vl )
                    {
                        vl1.Add( item2 );
                    }

                }
                else
                {
                    des[item.Key] = item.Value.ToString();
                }
            }

      
        }

        #endregion



        #region xml相关转换

        public static XmlDocument LoadXmlString( string xml_str )
        {
            xml_str = xml_str.Replace( "\r\n", "" ).Replace( "\n", "" ).Replace( "\t", "" );
            Regex rex = new Regex( @"(?<=encoding="")[\w-]+?(?="")|(?<=charset="")[\w-]+?(?="")|(?<=charset=)[\w-]+" );
            Encoding encoding = Encoding.Default;
            Match m = rex.Match( xml_str );
            if( m == null || !m.Success )
            {
                encoding = Encoding.UTF8;
            }
            else
            {
                encoding = Encoding.GetEncoding( m.Value );
            }
            byte[] array = encoding.GetBytes( xml_str );
            MemoryStream stream = new MemoryStream( array );

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;       //忽略注释 
            XmlReader reader = XmlReader.Create( stream, settings );
            XmlDocument xd = new XmlDocument();
            try
            {
                xd.Load( reader );
            }
            catch( Exception e)
            {
                //xd.LoadXml( xml_str );
                return null;
            }

            return xd;

        }

        /// <summary>
        /// XML字符串转换成字典对象
        /// </summary>
        /// <param name="xml_string">xml字符串</param>
        /// <param name="list_name">强制当作列表的节点，多个节点用,隔开</param>
        /// <returns></returns>
        public bool XmlStringToKeyValueEx( string xml_string, string list_name )
        {
            if( string.IsNullOrEmpty( xml_string ) )
            {
                return false;
            }
            XmlDocument xd = LoadXmlString( xml_string );
            if( xd == null|| xd.DocumentElement == null )
            {
                return false;
            }


            return XmlDocumentToKeyValueEx( xd, list_name );
        }




        /// <summary>
        /// xml文档对象转换成不包含xml头部信息的字符串
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        public static string XmlDocumentToString( XmlDocument xmlDoc )
        {
            return XmlDocumentToString( xmlDoc, true );
        }


        /// <summary>
        /// xml文档对象转换成字符串
        /// </summary>
        /// <param name="xmlDoc">文档对象</param>
        /// <param name="NoHead">输出时不包含xml头部信息</param>
        /// <returns></returns>
        public static string XmlDocumentToString( XmlDocument xmlDoc, bool NoHead )
        {
            MemoryStream stream = new MemoryStream();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = NoHead;//这一句表示忽略xml声明
            settings.Indent = true;
            settings.Encoding = Encoding.UTF8;
            XmlWriter writer = XmlWriter.Create( stream, settings );

            xmlDoc.Save( writer );

            StreamReader sr = new StreamReader( stream, System.Text.Encoding.UTF8 );
            stream.Position = 0;
            string xmlString = sr.ReadToEnd();
            sr.Close();
            stream.Close();

            return xmlString;
        }


        /// <summary>
        /// XmlNode下有无重复子节点
        /// </summary>
        /// <param name="xn"></param>
        /// <returns></returns>
        public static bool XmlDocumentNodeHasDuplicateChild( XmlNode xn )
        {
            string s = "";
            return HasXmlNodeDuplicateChild( xn, ref s );
        }


        /// <summary>
        /// 这个节点是不是可以看作List,list_name的处理方式是包含不是全等
        /// </summary>
        /// <param name="xn"></param>
        /// <param name="list_name"></param>
        /// <param name="son_name"></param>
        /// <returns></returns>
        public static bool IsXmlNodeList( XmlNode xn, string list_name, ref string son_name )
        {
            if( xn == null )
            {
                return false;
            }
            string[] list_pre = list_name.Split( ',' );
            bool b_list = false;
            foreach( var item in list_pre )
            {
                if( xn.Name.IndexOf( item ) > -1 )
                {
                    b_list = true;
                }
            }

            // 如果节点名在强制节点名中，那么把这个节点强制看作List，以当前节点的第一个子节点作为子节点名
            // 防止把一个list一个item当作字典处理的这种情况
            if( xn.FirstChild != null && b_list )
            {
                son_name = xn.FirstChild.Name;
                return true;
            }

            return false;
        }


        /// <summary>
        /// XmlNode下有无重复子节点
        /// </summary>
        /// <param name="xn"></param>
        /// <param name="son_name">第一个重复节点的名称</param>
        /// <returns></returns>
        public static bool HasXmlNodeDuplicateChild( XmlNode xn, ref string son_name )
        {

            Dictionary<string, string> temp = new Dictionary<string, string>();

            for( int i = 0; i < xn.ChildNodes.Count; i++ )
            {
                if( temp.ContainsKey( xn.ChildNodes[i].Name ) )
                {
                    son_name = xn.ChildNodes[i].Name;
                    return true;
                }
                // 忽略注释类型
                if( xn.ChildNodes[i].NodeType != XmlNodeType.Comment )
                {
                    temp[xn.ChildNodes[i].Name] = "呵呵";
                }
            }

            return false;
        }




        /// <summary>
        /// xml文档对象转换到Dictionary(string,object)或list
        /// 拥有重复名称子节点的节点会被转换成list，而忽略子节点的名称
        /// </summary>
        /// <param name="xml_doc">xml文档对象</param>
        /// <param name="dso">保存Dictionary的对象</param>
        /// <param name="list_name">列表节点后缀名称，有这个后缀的都会强制被识别为list，多个用,隔开</param>
        public bool XmlDocumentToKeyValueEx( XmlDocument xml_doc, string list_postfix_name = "" )
        {
            if( xml_doc == null || xml_doc.DocumentElement == null  )
            {
                return false;
            }
            this.attr.Clear();
            this.Clear();
            try
            {
                XmlNodeToKeyValueEx( xml_doc.DocumentElement, this, list_postfix_name );
                
            }
            catch( Exception e)
            {
                return false;
            }

            return true;
        }




        /// <summary>
        /// 转换当前节点为字典或list
        /// Item节点还是字典不变，但是Item节点作为value所对应的key是不存在的
        /// 它对应的key作为上一级List的成员保存了起来
        /// 例：
        /// 原来,大括号表示字典类型
        /// List:{Item:{id:1},Item:{id:1}}
        /// 现在，中括号实际上是ListEx类型
        /// List(Item):[{id:1},{id:1}]
        /// </summary>
        /// <param name="xn"></param>
        /// <param name="dso"></param>
        private static void XmlNodeToKeyValueEx( XmlNode xn, object dso, string list_name )
        {
            if( xn == null )
            {
                return;
            }
            XmlNodeList xnl = xn.ChildNodes;
            KeyValueExList kv_list = null;

            KeyValueEx kv = null;

            StringExList sel = null;

            if( dso is KeyValueEx )
            {
                kv = dso as KeyValueEx;
            }

            if( dso is KeyValueExList )
            {
                kv_list = dso as KeyValueExList;
            }

            if( dso is StringExList )
            {
                sel = dso as StringExList;
            }


            IAttr iattr = null;
            object temp = null;


            // 如果没有子节点了，退出递归的条件
            // 后一个条件是为了忽略xml的强制数据条件<![CDATA[...]]>，不把它当作子节点处理
            if( xnl.Count == 0 || xn.InnerText == xn.InnerXml || ( xnl.Count == 1 && ( xnl[0].NodeType == XmlNodeType.Text || xnl[0].NodeType == XmlNodeType.CDATA ) ) )
            {
                XmlElement xe = xn as XmlElement;
                StringEx se = null;
                if( string.IsNullOrEmpty( xe.InnerText ) )
                {
                    if( xn.FirstChild == null && xe!=null&&xe.IsEmpty )
					{
						se = new StringEx(  null  );
					}
					else
					{
						se = new StringEx( "" );
					}
                }
                else
                {
                    se = new StringEx( xe.InnerText );
                }
                

                //如果本节点是字典类型
                if( kv != null )
                {
                    // 添加键值对
                    kv.Add( xn.Name, se );

                }
                //// 如果本节点是list类型，直接添加值，忽略键，在这个版本下不可能出现的键值列表又是stringex
                //else if( kv_list != null )
                //{
                //    kv_list.Add( se );

                //}
                else if( sel != null )
                {
                    sel.Add( se );

                }
                else 
                {
                    throw new Exception( "转换失败" );
                }
                iattr = se as IAttr;
                if( iattr != null && xn.Attributes != null )
                {
                    foreach( XmlAttribute item in xn.Attributes )
                    {
                        iattr.Attr[item.Name] = item.Value;

                    }
                }
                return;
            }
            else
            {
                // 保存子节点(item)的
                string son_name = null;


                // 如果节点名在强制节点名中，那么把这个节点强制看作List，以当前节点的第一个子节点作为子节点名
                // 防止把一个list一个item当作字典处理的这种情况
                // 或者如果有重复的子节点
                if( IsXmlNodeList( xn, list_name, ref son_name ) || HasXmlNodeDuplicateChild( xn, ref son_name ) )
                {
                    //提前处理了字符数组的情况
                    if( xn.FirstChild.FirstChild == null || xn.FirstChild.InnerText == xn.FirstChild.InnerXml || (  xn.FirstChild.NodeType == XmlNodeType.Text ||  xn.FirstChild.NodeType == XmlNodeType.CDATA ))
                    {
                        temp = new StringExList()
                        {
                             SonName = son_name
                        };
                    }
                    else
                    {
                        // 装子节点的对象为list
                        // 实际保存为DictionaryEx<"List", List<object>>
                        temp = new KeyValueExList( son_name );
                    }

                }
                else
                {
                    temp = new KeyValueEx();

                }


                if( kv != null )
                {
                    // 以空字典添加节点
                    kv.Add( xn.Name, temp );
                }
                else if( kv_list != null )
                {

                    kv_list.Add( (KeyValueEx)temp );
                }
                else
                {
                    throw new Exception( "转换失败" );
                }

            }

            iattr = temp as IAttr;
            if( iattr != null )
            {
                foreach( XmlAttribute item in xn.Attributes )
                {
                    iattr.Attr[item.Name] = item.Value;

                }
            }

            // 子节点递归
            for( int i = 0; i < xnl.Count; i++ )
            {
                XmlNodeToKeyValueEx( xnl[i], temp, list_name );
            }
        }

        /// <summary>
        /// 字典转xml，必须保证所有值都是string类型
        /// </summary>
        /// <param name="xml_doc"></param>
        /// <param name="dso"></param>
        public static void KeyValueExToXmlDocument( XmlDocument xml_doc, object dso )
        {
            if( xml_doc == null || dso == null || !( dso is KeyValueEx ) )
            {
                return;
            }


            if( xml_doc.DocumentElement == null )
            {
                string first_key = null;
                foreach( var item in ( dso as KeyValueEx ) )
                {
                    first_key = item.Key;
                    break;
                }
                xml_doc.LoadXml( "<" + first_key + ">" + "</" + first_key + ">" );
                foreach( var item in ( ( dso as KeyValueEx )[first_key] as IAttr ).Attr )
                {
                    XmlAttribute xa = xml_doc.CreateAttribute( item.Key );
                    xa.Value = item.Value + "";
                    xml_doc.DocumentElement.Attributes.Append( xa );
                }
                KeyValueExToXmlStep( xml_doc, xml_doc.DocumentElement, ( dso as KeyValueEx )[first_key] );
            }
            else
            {
                KeyValueExToXmlStep( xml_doc, xml_doc.DocumentElement, dso );
            }

        }



        private static void KeyValueExToXmlStep( XmlDocument xml_doc, XmlNode xn, object dso )
        {
            IAttr attr = null;

            XmlNode temp = null;
            if( dso is KeyValueEx )
            {
                KeyValueEx d = dso as KeyValueEx;



                //if( d.Keys.Count == 1 )
                //{
                //    string key = null;
                //    string val = null;
                //    foreach( var item in d )
                //    {
                //        key = item.Key;
                //        val = item.Value as string;
                //    }

                //    if( val != null )
                //    {
                //        temp = xml_doc.CreateElement( key );
                //        temp.InnerText = val;
                //        xn.AppendChild( temp );
                //        return;
                //    }
                //}


                foreach( var item in dso as KeyValueEx )
                {
                    temp = xml_doc.CreateElement( item.Key );
                    if( item.Value is string || item.Value is decimal || item.Value is StringEx )
                    {
                        // 新建立的元素是自结束的节点，一旦被赋值，就变为结束节点
                        if( item.Value.ToString() != null )
                        {
                            temp.InnerText = item.Value.ToString();
                        }

                    }
                    else
                    {
                        KeyValueExToXmlStep( xml_doc, temp, item.Value );
                    }


                    attr = ( item.Value as IAttr );
                    if( attr != null )
                    {
                        foreach( var item1 in attr.Attr )
                        {
                            XmlAttribute xa = xml_doc.CreateAttribute( item1.Key );
                            xa.Value = item1.Value + "";
                            temp.Attributes.Append( xa );
                        }
                    }

                    xn.AppendChild( temp );

                }

            }
            else if( dso is KeyValueExList )
            {
                KeyValueExList temp_list = dso as KeyValueExList;


                for( int i = 0; i < temp_list.Count; i++ )
                {
                    temp = xml_doc.CreateElement( temp_list.SonName );

                    if( temp_list[i] is string || temp_list[i] is decimal || temp_list[i] is StringEx )
                    {
                        if( temp_list[i].ToString() != null )
                        {
                            temp.InnerText = temp_list[i].ToString();
                        }
                        
                    }
                    else
                    {
                        KeyValueExToXmlStep( xml_doc, temp, temp_list[i] );
                    }


                    attr = ( temp_list[i] as IAttr );
                    if( attr != null )
                    {
                        foreach( var item1 in attr.Attr )
                        {
                            XmlAttribute xa = xml_doc.CreateAttribute( item1.Key );
                            xa.Value = item1.Value + "";
                            temp.Attributes.Append( xa );
                        }
                    }

                    xn.AppendChild( temp );
                }

            }
            else if( dso is StringExList )
            {
                StringExList temp_list = dso as StringExList;


                for( int i = 0; i < temp_list.Count; i++ )
                {
                    temp = xml_doc.CreateElement( temp_list.SonName );

                    if( temp_list[i] is string || temp_list[i] is decimal || temp_list[i] is StringEx )
                    {
                        if( temp_list[i].ToString() != null )
                        {
                            temp.InnerText = temp_list[i].ToString();
                        }
                    }
                    else
                    {
                        throw new Exception( "转换错误" );
                        KeyValueExToXmlStep( xml_doc, temp, temp_list[i] );
                    }


                    attr = ( temp_list[i] as IAttr );
                    if( attr != null )
                    {
                        foreach( var item1 in attr.Attr )
                        {
                            XmlAttribute xa = xml_doc.CreateAttribute( item1.Key );
                            xa.Value = item1.Value + "";
                            temp.Attributes.Append( xa );
                        }
                    }

                    xn.AppendChild( temp );
                }

            }

        }

        
        #endregion



    }


    

    

	
}
