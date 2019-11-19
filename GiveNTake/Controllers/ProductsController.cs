using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GiveNTake.Model;
using GiveNTake.Model.DTO;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace GiveNTake.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : Controller
    {
        private readonly GiveNTakeContext _context;
        private static readonly IMapper _productsMapper;

        public ProductsController(GiveNTakeContext context) => _context = context;

        static ProductsController()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Product, ProductDTO>()
                    .ForMember(dto => dto.City, opt => opt.MapFrom(product => product.City))
                    .ForMember(dto => dto.Category, opt => opt.MapFrom(product => product.Category.ParentCategory.Name))
                    .ForMember(dto => dto.Subcategory, opt => opt.MapFrom(product => product.Category.Name));

                cfg.CreateMap<User, OwnerDTO>()
                    .ForMember(dto => dto.Id, opt => opt.MapFrom(product => product.UserId));

                cfg.CreateMap<City, CityDTO>()
                    .ForMember(dto => dto.Id, opt => opt.MapFrom(city => city.CityId))
                    .ForMember(dto => dto.Name, opt=> opt.MapFrom(city => city.Name));

                cfg.CreateMap<ProductMedia, MediaDTO>()
                    .ForMember(dto => dto.Url, opt => opt.MapFrom(media => media.Url));

                cfg.CreateMap<Category, CategoryDTO>();
                cfg.CreateMap<Category, SubCategoryDTO>();

            });
            _productsMapper = config.CreateMapper();

        }

        [HttpGet("searchcategory/{category}/{subcategory=all}/")]
        public string[] SearchByProducts(string category, string subcategory)
        {
            return new[]{$"Category: {category}, Subcategory: {subcategory}"};
        }

        [HttpGet("cities")]
        public async Task<ActionResult<CityDTO[]>> GetCities()
        {
            var cities = await _context.Cities
                .ToArrayAsync();
            return _productsMapper.Map<CityDTO[]>(cities);
        }
    }
}