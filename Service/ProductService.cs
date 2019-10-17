using System.Collections.Generic;
using System.Linq;
using ExpenseManageBack.CustomModel;
using ExpenseManageBack.Model;
using Newtonsoft.Json.Linq;
using yuyu.Infrastructure;
using yuyu.Service;

namespace ExpenseManageBack.Service
{
    /// <summary>
    /// 产品service
    /// </summary>
    public class ProductService : BaseService
    {
        private IUnitWork _unitWork { get; set; }
        
        public ProductService(IUnitWork unitWork) : base(unitWork)
        {
            _unitWork = unitWork;
        }

        /// <summary>
        /// 产品首页 查询产品列表
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> getProductList()
        {
            var allProductType = _unitWork.Find<Product>(null).Select(u => u.Type).Distinct().ToList();

            var result = new Dictionary<string, object>();
            
            foreach (var type in allProductType)
            {
                var productList = _unitWork.Find<Product>(u => u.Type.Equals(type)).ToList();
                
                result.Add(type, productList);
            }
            
            return result;
        }
    }
}