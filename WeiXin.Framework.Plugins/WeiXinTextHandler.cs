using WeiXin.Framework.Core;
using WeiXin.Framework.Core.Entities;
using WeiXin.Framework.Plugins.Business_Entity;
using System.Configuration;
using System.IO;
using System;
using System.Data.SqlClient;
using System.Data;
using System.Xml.Linq;


namespace WeiXin.Framework.Plugins
{
    /// <summary>
    /// 微信文本消息处理控制器
    /// </summary>
    public class WeiXinTextHandler : WeiXinHandlerType
    {        
        #region 字段/属性

        /// <summary>
        /// 消息类型text
        /// </summary>
        public override WeiXinMsgType WeiXinMsgType
        {
            get { return WeiXinMsgType.Text; }
        }
        #endregion

        public override void ProcessWeiXin(WeiXinContext context)
        {            
            WeiXinTextMessageEntity requestModel = context.Request.GetRequestModel<WeiXinTextMessageEntity>();
            string INFO = requestModel.Content;
            string rtn_Info = string.Empty;
            string routine = ConfigurationManager.ConnectionStrings["routine"].ConnectionString;
            if (INFO.IndexOf ("注册")>=0)
            {
                string name = string.Empty;
                if (INFO.IndexOf("+")>0)
                {
                    name = INFO.Substring(INFO.IndexOf("+") + 1).Trim();
                }
                else 
                {
                    name = INFO.Substring(INFO.IndexOf("注册") + 2).Trim();
                }
                if (Deploy_Info.SetEngineer(requestModel.FromUserName, name))                
                {
                    //FileStream file = new FileStream(@"C:\User.txt", FileMode.Append);
                    ////获得字节数组
                    //byte[] data = System.Text.Encoding.Default.GetBytes(name + "|" + requestModel.FromUserName + "\r\n");
                    ////开始写入
                    //file.Write(data, 0, data.Length);
                    ////清空缓冲区、关闭流
                    //file.Flush();
                    //file.Close();
                    rtn_Info = "【" + name + "】注册成功";
                }
                else 
                {
                    rtn_Info = "【" + name + "】注册失败";
                }
            }

            else if (INFO.IndexOf("实施") >= 0)
            {
                string HospID = INFO.Substring(INFO.IndexOf("实施") + 2).Trim();
                string FromUserName = requestModel.FromUserName;
                string User_Name = Deploy_Info.GetEngineer(FromUserName);
                if (User_Name=="-1")
                {
                    rtn_Info = "工程师姓名尚未注册，请注册后登记实施信息！";
                }
                else
                {
                string Hosp_info = Deploy_Info.GetHosp(HospID, "", "");                
                string[] Hospinfo = Hosp_info.Split('|');                
                string Hosp_Name = Hospinfo[1];
                    if (Hosp_Name == "-1")
                    {
                        rtn_Info = "查无此【" + HospID + "】医院信息，请核对后重新登记！";
                    }
                    else 
                    { 
                    string Work_Time = DotNetExtensions.IntStringToDateTime(requestModel.CreateTime).ToString ();
                        if (Deploy_Info.SetWork(FromUserName, User_Name, HospID, Hosp_Name, Work_Time))
                        {                    
                            rtn_Info = "【" + Hosp_Name + "】实施工作登记成功,请在[" + routine + "]分钟内上传位置与照片信息！";
                        }
                    }
                }
            }
            else if (INFO.IndexOf("查询") >= 0)
            {
                string HospID = INFO.Substring(INFO.IndexOf("查询") + 2).Trim();
                string FromUserName = requestModel.FromUserName;
                string Engineer = Deploy_Info.GetEngineer(FromUserName);
                if (Engineer == "-1")
                {
                    rtn_Info = "工程师姓名尚未注册，请注册后重新进行查询！";
                }
                else
                {
                    string Hosp_info = Deploy_Info.GetHosp(HospID, "", "");
                    string[] Hospinfo = Hosp_info.Split('|');
                    string Hosp_Name = Hospinfo[1];
                    if (Hosp_Name == "-1")
                    {
                        rtn_Info = "查无此【" + HospID + "】医院信息，请核对后重新查询！";
                    }
                    else
                    {
                        string Location_Info = Deploy_Info.GetLocation(HospID);
                        string[] Location = Location_Info.Split('|');
                        int ArticleCount = 0;
                        bool IsLocation = false;                        
                        if (Location_Info != "-1")
                        { IsLocation = true; }
                        FileInfo[] Pic = Deploy_Info.GetMultimedia(HospID);
                        if (IsLocation || Pic.Length > 0)
                        {
                            WeiXinNewsMessageEntity responseModelNews = new WeiXinNewsMessageEntity()
                            {
                                ToUserName = requestModel.FromUserName,
                                MsgType = "News"
                            };
                            XElement xElement = responseModelNews.GetXElement();
                            int num = 0;
                            if (Pic.Length <= 7)//实际的图片数量为pic.length,最多图文消息为8个
                            { num = Pic.Length; }
                            else { num = 7; }
                            if (IsLocation)
                            {
                                ArticleCount = num + 1;
                            }
                            else { ArticleCount = num; }

                            xElement.Add(new XElement("ArticleCount", ArticleCount.ToString()));
                            object[] item = new object[num + 1];
                            for (int i = 0; i < num; i++)
                            {
                                item[i] = new XElement("item", new XElement("Title", Hosp_Name), new XElement("Description", ""), new XElement("PicUrl", "http://210.73.74.172/image/" + Pic[i].Name.ToString()), new XElement("Url", "http://210.73.74.172/image/" + Pic[i].Name.ToString()));
                            }
                            if (IsLocation)
                            {
                                item[num] = new XElement("item", new XElement("Title", "地图导航"), new XElement("Description", Hosp_Name), new XElement("PicUrl", "http://210.73.74.172/image/Map.jpg"), new XElement("Url", "http://apis.map.qq.com/uri/v1/geocoder?coord=" + Location[0] + "," + Location[1] + "&referer=myapp"));
                            }
                            
                            xElement.Add(new XElement("Articles", item));
                            context.Response.Write(xElement);
                            return;
                            
                        }                        
                        else 
                        {
                            rtn_Info = "【" + Hosp_Name + "】没有上传照片和位置信息！";
                        }
                    }
                }
            }         
            
            else
            {
                rtn_Info = "请回复‘注册工程师姓名’（如‘注册小明’）进行人员注册；回复‘实施医院编码’（如‘实施99900001’）上报实施工作，并在【" + routine + "】分钟内上报医院位置与照片信息；回复‘查询医院编码’（如‘查询99900001’）查询该医院照片并进行位置导航";
            }
            

            WeiXinTextMessageEntity responseModel = new WeiXinTextMessageEntity
                {
                    ToUserName =requestModel.FromUserName,                    
                    Content = string.Format(rtn_Info),
                    MsgType = requestModel.MsgType
                };
            context.Response.Write(responseModel);
        }       
    }
}
