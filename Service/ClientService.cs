using System.Collections.Generic;
using System.Linq;
using ExpenseManageBack.CustomModel;
using ExpenseManageBack.Model;
using Newtonsoft.Json.Linq;
using yuyu.Infrastructure;
using yuyu.Service;

namespace ExpenseManageBack.Service
{
    public class ClientService : BaseService
    {
        private IUnitWork _unitWork { get; set; }
        public ClientService(IUnitWork unitWork) : base(unitWork)
        {
            _unitWork = unitWork;
        }

        /// <summary>
        /// 产品首页 查询产品列表
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> GetList()
        {
            var allClientType = _unitWork.Find<ClientType>(null).ToList();

            var result = new Dictionary<string, object>();

            foreach (ClientType type in allClientType)
            {
                var list = _unitWork.Find<Client>(u => u.Type.Equals(type.Name)).ToList();

                result.Add(type.Name, list);
            }

            return result;
        }

    }
}
