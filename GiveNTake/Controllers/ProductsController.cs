using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GiveNTake.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        [HttpGet("search/{category}/{subcategory=all}")]
        public string[] GetProducts(string category, string subcategory) {
            return new [] {
                   $"Category: {category} SubCategory: {subcategory}"
                };
        }
    }
}