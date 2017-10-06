using System;
using System.Xml.Linq;
using WeiXin.Framework.Core;

namespace WeiXin.Framework.Plugins
{
    /// <summary>
    /// 微信用户关注事件控制器
    /// </summary>
    public class WeiXinSubscribeHandler : WeiXinHandlerType
    {
        #region 字段/属性

        /// <summary>
        /// 消息类型Event
        /// </summary>
        public override WeiXinMsgType WeiXinMsgType
        {
            get { return WeiXinMsgType.Event; }
        }

        /// <summary>
        /// 用户关注事件
        /// </summary>
        public override WeiXinEventType WeiXinEventType
        {
            get { return WeiXinEventType.SubScribe; }
        }

        #endregion

        public override void ProcessWeiXin(WeiXinContext context)
        {
            XElement result = new XElement("xml", new XElement("ToUserName", context.Request.FromUserName),
                new XElement("FromUserName", context.Request.ToUserName),
                new XElement("CreateTime", DateTime.Now.GetInt()),
                new XElement("MsgType", WeiXinMsgType.Text.ToString().ToLower()),
                new XElement("Content", "欢迎关注【新农合医院端实施与现场验收项目】的微信订阅号.回复‘注册+工程师姓名’进行人员注册，回复‘实施+医院编码’上报实施工作，同步上报医院位置信息"));
            context.Response.Write(result);
        }
    }
}
