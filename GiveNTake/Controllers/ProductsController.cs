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
using Microsoft.AspNetCore.Authorization;

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
                    .ForMember(dto => dto.Id, opt => opt.MapFrom(product => product.Id));

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

        [AllowAnonymous]
        [HttpGet("cities")]
        public async Task<ActionResult<CityDTO[]>> GetCities()
        {
            var cities = await _context.Cities
                .ToArrayAsync();
            return _productsMapper.Map<CityDTO[]>(cities);
        }

        [AllowAnonymous]
        [HttpGet("{id}", Name = nameof(GetProduct))]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Owner)
                .Include(p => p.City)
                .Include(p => p.Media)
                .Include(p => p.Category)
                .ThenInclude(c => c.ParentCategory)
                .SingleOrDefaultAsync(p => p.ProductId == id);

            return _productsMapper.Map<ProductDTO>(product);
        }

        [HttpPost("")]
        public async Task<ActionResult<ProductDTO>> AddNewProduct([FromBody] NewProductDTO newProduct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Category category = _context.Categories
                .Include(c => c.ParentCategory)
                .SingleOrDefault(c => c.Name == newProduct.Subcategory || c.ParentCategory.Name == newProduct.Subcategory);

            if (category == null)
            {
                return new BadRequestObjectResult("The provided category and sub category doesnt exist");
            }

            City city = _context.Cities.SingleOrDefault(c => c.Name == newProduct.City);

            if (city == null)
            {
                return new BadRequestObjectResult("The provided city doesnt exist");
            }

            var user = await _context.Users.FindAsync(User.Identity.Name);

            var product = new Product()
            {
                Owner = user,
                Category = category,
                Title = newProduct.Title,
                Description = newProduct.Description,
                City = city,
                PublishDate = DateTime.UtcNow
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                    nameof(GetProduct),
                    new { id = product.ProductId },
                    _productsMapper.Map<ProductDTO>(product));
        }
    }
}