using WeiXin.Framework.Core;
using WeiXin.Framework.Core.Entities;
using WeiXin.Framework.Plugins.Business_Entity;
using System.IO;
using System;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Globalization;

namespace WeiXin.Framework.Plugins
{
    /// <summary>
    /// 微信位置消息处理控制器
    /// </summary>
    public class WeixinLocationHandler : WeiXinHandlerType
    {        
        #region 字段/属性

        /// <summary>
        /// 消息类型text
        /// </summary>
        public override WeiXinMsgType WeiXinMsgType
        {
            get { return WeiXinMsgType.Location; }
        }
        #endregion

        public override void ProcessWeiXin(WeiXinContext context)
        {
            WeiXinLocationMessageEntity requestModel = context.Request.GetRequestModel<WeiXinLocationMessageEntity>();            
            string rtnInfo = string.Empty;
            //string FileData = formatTime(long.Parse(requestModel.CreateTime)) + "|" + requestModel.FromUserName + "|" + rtnInfo;
            string Hosp_info = Deploy_Info.GetHosp("",requestModel.FromUserName, DotNetExtensions.IntStringToDateTime(requestModel.CreateTime).ToString ());
            string[] Hospinfo = Hosp_info.Split('|');
            string HospID = Hospinfo[0];
            string Hosp_Name = Hospinfo[1];
            string FileData = HospID + "|" + Hosp_Name;
            if (HospID != "-1")
            {
                if (Deploy_Info.SetLocation("Hosp", Hospinfo[0], requestModel.Location_X.ToString(), requestModel.Location_Y.ToString(), requestModel.Label))
            {
                //FileStream file = new FileStream(@"C:\Location.txt", FileMode.Append);
                ////获得字节数组
                //byte[] data = System.Text.Encoding.Default.GetBytes(FileData + "\r\n");
                ////开始写入
                //file.Write(data, 0, data.Length);
                ////清空缓冲区、关闭流
                //file.Flush();
                //file.Close();
                rtnInfo = "收到【" + Hosp_Name + "】位置信息:" + requestModel.Label + "|" + requestModel.Location_X.ToString() + "|" + requestModel.Location_Y.ToString();                
            }
            }
            else
            {
             rtnInfo = "未找到对应医院信息，请先登记实施医院信息，再上传位置信息！";
            }
            WeiXinTextMessageEntity responseModel = new WeiXinTextMessageEntity
            {
                ToUserName = requestModel.FromUserName,
                Content = string.Format(rtnInfo),
                MsgType = "Text"
            };
            context.Response.Write(responseModel);
        }
    }
}
