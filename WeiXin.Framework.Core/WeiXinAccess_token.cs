using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace WeiXin.Framework.Core
{
    /// <summary>  
    ///Access_token 的摘要说明  
    /// </summary>  
    public class WeiXinAccess_token
    {
        public WeiXinAccess_token()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }
        /// <summary>
        /// 通过appID和appsecret获取Access_token
        /// </summary>
        /// <returns></returns>
        private static WeiXinAccess_token GetAccess_token()
        {
            string AppID = ConfigurationManager.ConnectionStrings["AppID"].ConnectionString;
            string AppSecret = ConfigurationManager.ConnectionStrings["AppSecret"].ConnectionString;
            string strUrl = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + AppID + "&secret=" + AppSecret;
            WeiXinAccess_token mode = new WeiXinAccess_token();
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(strUrl);
            req.Method = "GET";
            using (WebResponse wr = req.GetResponse())
            {
                HttpWebResponse myResponse = (HttpWebResponse)req.GetResponse();
                StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
                string content = reader.ReadToEnd();//在这里对Access_token 赋值  
                WeiXinAccess_token token = new WeiXinAccess_token();                
                token = JsonHelper.ParseFromJson<WeiXinAccess_token>(content);
                mode._access_token = token._access_token;
                mode._expires_in =token._expires_in;                
            }
            return mode;
        }
        /// <summary>
        /// 获取Access_token值
        /// </summary>
        /// <returns></returns>
        public static string IsExistAccess_Token()
        {
            string Token = string.Empty;
            DateTime YouXRQ;
            // 读取XML文件中的数据，并显示出来 ，注意文件路径  
            string filepath = System.Web.HttpContext.Current.Server.MapPath("XMLFile.xml");
            StreamReader str = new StreamReader(filepath, System.Text.Encoding.UTF8);
            XmlDocument xml = new XmlDocument();
            xml.Load(str);
            str.Close();
            str.Dispose();
            Token = xml.SelectSingleNode("xml").SelectSingleNode("Access_Token").InnerText;
            YouXRQ = Convert.ToDateTime(xml.SelectSingleNode("xml").SelectSingleNode("Access_YouXRQ").InnerText);
            if (DateTime.Now > YouXRQ)
            {
                DateTime _youxrq = DateTime.Now;
                WeiXinAccess_token mode = GetAccess_token();
                xml.SelectSingleNode("xml").SelectSingleNode("Access_Token").InnerText = mode._access_token;
                _youxrq = _youxrq.AddSeconds(int.Parse(mode.expires_in));
                xml.SelectSingleNode("xml").SelectSingleNode("Access_YouXRQ").InnerText = _youxrq.ToString();
                xml.Save(filepath);
                Token = mode._access_token;
            }
            return Token;
        }
 


        string _access_token;
        string _expires_in;

        /// <summary>
        /// 获取到的凭证 
        /// </summary>
        public string access_token
        {
            get { return _access_token; }
            set { _access_token = value; }
        }


        /// <summary>
        /// 凭证有效时间，单位：秒
        /// </summary>
        public string expires_in
        {
            get { return _expires_in; }
            set { _expires_in = value; }
        }
    }
}
