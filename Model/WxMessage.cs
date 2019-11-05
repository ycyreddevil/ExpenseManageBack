using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseManageBack.Model
{

     
     
     
    
    public class WxTextMessage
    {
        public string touser { get; set; }
        public string toparty { get; set; }
        public string totag { get; set; }
        public string msgtype { get; set; }
        public int agentid { get; set; }
        public WxTextMessageTextFiled text { get; set; }
        public int safe { get; set; }
        public int enable_id_trans { get; set; }

        public class WxTextMessageTextFiled
        {
            public string content { get; set; }

            public WxTextMessageTextFiled(string txt)
            {
                content = txt;
            }
        }
        /// <summary>
        /// 文本消息
        ///     {
        ///           "touser" : "UserID1|UserID2|UserID3",
        ///           "toparty" : "PartyID1|PartyID2",
        ///           "totag" : "TagID1 | TagID2",
        ///           "msgtype" : "text",
        ///           "agentid" : 1,
        ///           "text" : {
        ///               "content" : "你的快递已到，请携带工卡前往邮件中心领取。\n出发前可查看<a href=\"http:///work.weixin.qq.com\">邮件中心视频实况</a>，聪明避开排队。"
        ///           },
        ///           "safe":0,
        ///           "enable_id_trans": 0
        ///        }    
        ///      参数 是否必须        说明
        /// touser      否 成员ID列表（消息接收者，多个接收者用‘|’分隔，最多支持1000个）。特殊情况：指定为 @all，则向该企业应用的全部成员发送
        ///toparty     否 部门ID列表，多个接收者用‘|’分隔，最多支持100个。当touser为 @all时忽略本参数
        ///        totag 否           标签ID列表，多个接收者用‘|’分隔，最多支持100个。当touser为 @all时忽略本参数
        ///        msgtype 是           消息类型，此时固定为：text
        ///    agentid     是 企业应用的id，整型。企业内部开发，可在应用的设置页面查看；第三方服务商，可通过接口 获取企业授权信息 获取该参数值
        ///content     是 消息内容，最长不超过2048个字节，超过将截断（支持id转译）
        ///        safe 否           表示是否是保密消息，0表示否，1表示是，默认0
        /// enable_id_trans 否 表示是否开启id转译，0表示否，1表示是，默认0
        /// 
        /// 
        /// </summary>
        public WxTextMessage(int _agentid,string msg,string _touser= "@all")
        {
            touser = _touser;
            toparty = "";
            totag = "";
            msgtype = "text";
            agentid = _agentid;
            text = new WxTextMessageTextFiled(msg);
            safe = 0;
            enable_id_trans = 0;
        }
    }

    

    public class WxTextCardMessage
    {
        public string touser { get; set; }
        public string toparty { get; set; }
        public string totag { get; set; }
        public string msgtype { get; set; }
        public int agentid { get; set; }
        public WxTextCardMessageTextCardField textcard { get; set; }
        public int enable_id_trans { get; set; }

        public class WxTextCardMessageTextCardField
        {
            public string title { get; set; }
            public string description { get; set; }
            public string url { get; set; }
            public string btntxt { get; set; }

            public WxTextCardMessageTextCardField (string _title, string _description, string _url, string _btntxt)
            {
                title = _title;
                description = _description;
                url = _url;
                btntxt = _btntxt;
            }
        }

        /// <summary>
        /// 文本卡片消息
        /// {
        ///   "touser" : "UserID1|UserID2|UserID3",
        ///   "toparty" : "PartyID1 | PartyID2",
        ///   "totag" : "TagID1 | TagID2",
        ///   "msgtype" : "textcard",
        ///   "agentid" : 1,
        ///   "textcard" : {
        ///            "title" : "领奖通知",
        ///            "description" : "<div class=\"gray\">2016年9月26日</div> <div class=\"normal\">恭喜你抽中iPhone 7一台，领奖码：xxxx</div><div class=\"highlight\">请于2016年10月10日前联系行政同事领取</div>",
        ///            "url" : "URL",
        ///            "btntxt":"更多"
        ///   },
        ///   "enable_id_trans": 0
        ///}
        ///参数       是否必须    说明
        ///touser       否       成员ID列表（消息接收者，多个接收者用‘|’分隔，最多支持1000个）。特殊情况：指定为 @all，则向关注该企业应用的全部成员发送
        ///toparty      否       部门ID列表，多个接收者用‘|’分隔，最多支持100个。当touser为 @all时忽略本参数
        ///totag        否       标签ID列表，多个接收者用‘|’分隔，最多支持100个。当touser为 @all时忽略本参数
        ///msgtype      是       消息类型，此时固定为：textcard
        ///agentid      是       企业应用的id，整型。企业内部开发，可在应用的设置页面查看；第三方服务商，可通过接口 获取企业授权信息 获取该参数值
        ///title        是       标题，不超过128个字节，超过会自动截断（支持id转译）
        ///description  是       描述，不超过512个字节，超过会自动截断（支持id转译）
        ///url          是       点击后跳转的链接。
        ///btntxt       否       按钮文字。 默认为“详情”， 不超过4个文字，超过自动截断。
        ///enable_id_trans 否    表示是否开启id转译，0表示否，1表示是，默认0
        /// 
        /// </summary>
        public WxTextCardMessage(int _agentid, string _title,string _description,string _url,string _btntxt,string _touser= "@all")
        {
            touser = _touser;
            toparty = "";
            totag = "";
            msgtype = "textcard";
            agentid = _agentid;
            textcard = new WxTextCardMessageTextCardField(_title, _description, _url, _btntxt);
            enable_id_trans = 0;
        }
    }
}
