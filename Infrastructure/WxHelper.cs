using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ExpenseManageBack.Infrastructure
{
    public class WxHelper
    {
        private string CorpId = "";
        private string AppSecret = "";
        private string AgentId = "";
        private HttpContext Context;

        public WxHelper(string appSecret,string agentId,HttpContext context)
        {
            AppSecret = appSecret;
            AgentId = agentId;
            Context = context;
        }

        public string GetToken()
        {
            string token = "";
            CookieHelper cookie = new CookieHelper(Context);

            return token;
        }
    }
}
