using System;
using System.Collections.Generic;
using ExpenseManageBack.CustomModel;
using ExpenseManageBack.Infrastructure;
using ExpenseManageBack.Model;
using ExpenseManageBack.Service;
using Microsoft.AspNetCore.Mvc;
using yuyu.Infrastructure;

namespace ExpenseManageBack.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ProductController : ControllerBase
    {
        private ProductService _service;

        public ProductController(ProductService productService, WxHelper wxHelper)
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
            
            return resp;
        }
        
        /// <summary>
        /// 产品首页 通过名字查询产品
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpPost]
        public Response<List<Product>> getProductByName(string name)
        {
            var resp = new Response<List<Product>>();

            try
            {
                resp.Result = _service.getProductByName(name);
            }
            catch (Exception e)
            {
                resp.code = 500;
                resp.message = e.Message;
            }
            
            return resp;
        }
        
        /// <summary>
        /// 通过编号查询产品
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpPost]
        public Response<Product> getProductByCode(string code)
        {
            var resp = new Response<Product>();

            try
            {
                resp.Result = _service.getProductByCode(code);
            }
            catch (Exception e)
            {
                resp.code = 500;
                resp.message = e.Message;
            }
            
            return resp;
        }
        
        /// <summary>
        /// 删除产品
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpPost]
        public Response deleteProduct(string code)
        {
            var resp = new Response();

            try
            {
                _service.deleteProduct(code);
            }
            catch (Exception e)
            {
                resp.code = 500;
                resp.message = e.Message;
            }
            
            return resp;
        }
        
        /// <summary>
        /// 新增或更新 产品
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpPost]
        public Response addOrUpdateProduct(Product product)
        {
            var resp = new Response();

            try
            {
                _service.addOrUpdateProduct(product);
            }
            catch (Exception e)
            {
                resp.code = 500;
                resp.message = e.Message;
            }
            
            return resp;
        }
    }
}