using System;
using System.Collections.Generic;
using ExpenseManageBack.CustomModel;
using ExpenseManageBack.Service;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseManageBack.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ProductController : ControllerBase
    {
        private ProductService _service;

        public ProductController(ProductService productService)
        {
            _service = productService;
        }

        /// <summary>
        /// 产品首页 查询产品列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public Response<Dictionary<string, object>> getProductList()
        {
            var resp = new Response<Dictionary<string, object>>();

            try
            {
                resp.Result = _service.getProductList();
            }
            catch (Exception e)
            {
                resp.code = 500;
                resp.message = e.Message;
            }
            
            return null;
        }
    }
}