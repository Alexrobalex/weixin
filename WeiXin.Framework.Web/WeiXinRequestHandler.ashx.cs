using System;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using WeiXin.Framework.Core;
using System.IO;
using System.Text;

namespace WeiXin.Framework.Web
{
    /// <summary>
    /// 处理微信请求信息入口控制器
    /// </summary>
    public class WeiXinRequestHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            // 请求方式大写
            string httpMethod = context.Request.HttpMethod.ToUpper();
            // 响应消息结果字符串对象
            object responseXml = string.Empty;

            //FileStream file = new FileStream(@"D:\MyWeb\log.txt", FileMode.Create);
            //file.Close();

            try
            {

                switch (httpMethod)
                {
                    // GET方式请求
                    case "GET":
                        string signature = context.Request.QueryString["signature"], // 微信加密签名
                               timestamp = context.Request.QueryString["timestamp"], // 时间戳
                               nonce = context.Request.QueryString["nonce"], // 随机数
                               echostr = context.Request.QueryString["echostr"];// 随机字符串
                        // 微信请求参数非空验证
                        if (!String.IsNullOrEmpty(signature) && !String.IsNullOrEmpty(timestamp) && !String.IsNullOrEmpty(nonce) && !String.IsNullOrEmpty(echostr))
                        {
                            // 检查加密签名是否正确
                            if (UtilityHelper.CheckSignature(signature, timestamp, nonce))
                            {
                                responseXml = echostr;
                            }
                        }
                        FileStream file = new FileStream(@"C:\log.txt", FileMode.Create);
                            //获得字节数组
                            byte[] data = System.Text.Encoding.Default.GetBytes(responseXml.ToString());
                            //开始写入
                            file.Write(data, 0, data.Length);
                            //清空缓冲区、关闭流
                            file.Flush();
                            file.Close();
                        break;
                    // POST方式请求
                    case "POST":
                        if (!string.IsNullOrEmpty(context.Request.Form["weixinMsg"]))
                        {
                            // 处理test.html页面测试的请求，并返回信息
                            XElement requestXml = XElement.Parse(context.Request.Form["weixinMsg"]);
                            WeiXinApplication weiXinApplication = new WeiXinApplication(requestXml);
                            responseXml = weiXinApplication.GetResponseXml();                            
                        }
                        else
                        {
                            // 解析微信请求
                            XElement requestXml = XElement.Load(context.Request.InputStream);
                            // 处理微信消息请求，并返回信息
                            WeiXinApplication weiXinApplication = new WeiXinApplication(requestXml);
                            responseXml = weiXinApplication.GetResponseXml();
                        }
                            FileStream file1 = new FileStream(@"C:\log.txt", FileMode.Create);
                            //获得字节数组
                            byte[] data1 = System.Text.Encoding.Default.GetBytes(responseXml.ToString());
                            //开始写入
                            file1.Write(data1, 0, data1.Length);
                            //清空缓冲区、关闭流
                            file1.Flush();
                            file1.Close();
                        break;
                }
            }
            catch (XmlException ex)
            {
                responseXml = string.Format("xml解析异常:{0}", ex.Message);
            }
            catch (WeiXinHandlerNotFoundException ex)
            {
                responseXml = ex.Message;
            }
            catch (Exception ex)
            {
                responseXml = ex.Message;
            }

            context.Response.Clear();
            context.Response.Charset = "UTF-8";
            context.Response.Write(responseXml);
            context.Response.End();
            
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}