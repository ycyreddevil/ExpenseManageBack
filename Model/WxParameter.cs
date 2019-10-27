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
        private List<WxApp> Apps { get; set; }
        public WxApp App { get; set; }

        public WxParameter(string appName)
        {            
            CorpId = AppSettingHelper.GetSection("WxParameter:CorpId");
            UserInfoSaveCookieDays= Convert.ToInt32(AppSettingHelper.GetSection("WxParameter:UserInfoSaveCookieDays"));
            Apps = Json.ToObject<List<WxApp>>(AppSettingHelper.GetSection("WxParameter:Apps"));
            App = null;
            foreach(WxApp app in Apps)
            {
                if(app.Name.Equals(appName))
                {
                    App = app;
                    break;
                }
            }
        }


    }

    public class WxApp
    {
        public string Name{ get; set; }
        public string AgentId { get; set; }
        public string Secret { get; set; }

    }
}
