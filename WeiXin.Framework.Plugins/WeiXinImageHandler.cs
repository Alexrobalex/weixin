using System.Xml.Linq;
using WeiXin.Framework.Core;
using WeiXin.Framework.Core.Entities;
using System.IO;
using WeiXin.Framework.Plugins.Business_Entity;


namespace WeiXin.Framework.Plugins
{
    /// <summary>
    /// 微信图片消息处理控制器
    /// </summary>
    public class WeiXinImageHandler:WeiXinHandlerType
    {
        #region 字段/属性

        /// <summary>
        /// 消息类型image
        /// </summary>
        public override WeiXinMsgType WeiXinMsgType
        {
            get { return WeiXinMsgType.Image; }
        }

        #endregion

        public override void ProcessWeiXin(WeiXinContext context)
        {
            WeiXinImageMessageEntity requestModel = context.Request.GetRequestModel<WeiXinImageMessageEntity>();
            string AccessToken = WeiXinAccess_token .IsExistAccess_Token();

            string rtnInfo = string.Empty;
            string Hosp_info = Deploy_Info.GetHosp("", requestModel.FromUserName, DotNetExtensions.IntStringToDateTime(requestModel.CreateTime).ToString());
            string[] Hospinfo = Hosp_info.Split('|');
            string HospID = Hospinfo[0];
            string Hosp_Name = Hospinfo[1];
            string FileData = HospID + "|" + Hosp_Name;
            string Engineer = Deploy_Info.GetEngineer(requestModel.FromUserName);
            if (HospID != "-1")
            {
                if (Deploy_Info.SetMultimedia(AccessToken, requestModel.MediaId, HospID))
                {
                    rtnInfo = "收到【" + Hosp_Name + "】照片信息。";
                }
            }
            else
            {
                rtnInfo = "未找到对应医院信息，请先登记实施医院信息，再上传照片信息！";
            }

            //WeiXinImageMessageEntity responseModel = new WeiXinImageMessageEntity
            //{
            //    ToUserName = requestModel.FromUserName,
            //    MsgType = WeiXinMsgType.Image.ToString().ToLower()
            //};

            //XElement xElement = responseModel.GetXElement();
            //xElement.Add(new XElement("Image", new XElement("MediaId", requestModel.MediaId)));

            //context.Response.Write(xElement);
            

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
