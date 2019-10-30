using ExpenseManageBack.Infrastructure;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using yuyu.Infrastructure;

namespace ExpenseManageBack.Model
{
    public class WxParameter
    {
        public string CorpId { get; set; }
        public int UserInfoSaveCookieDays { get; set; }
        public WxApp App { get; set; }

        public WxParameter(string appName)
        {            
            CorpId = AppSettingHelper.GetSection("WxParameter:CorpId");
            UserInfoSaveCookieDays= Convert.ToInt32(AppSettingHelper.GetSection("WxParameter:UserInfoSaveCookieDays"));
            App = new WxApp
            {
                AgentId = AppSettingHelper.GetSection(string.Format("WxParameter:{0}:AgentId", appName)),
                Secret = AppSettingHelper.GetSection(string.Format("WxParameter:{0}:Secret", appName))
            };
        }
    }

    public class WxApp
    {
        public string AgentId { get; set; }
        public string Secret { get; set; }
    }
}
