using Classifieds.Data.DTOs;
using Classifieds.Data.DTOs.CategoryDTOs;
using Classifieds.Data.Enums;
using Classifieds.Services.IServices;
using Classifieds.Services.Services;
using Classifieds_API.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Classifieds_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : BaseController
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            return Ok(category);
        }
        [HttpPost]
        [Authorize(Role.Admin, Role.SuperAdmin)]
        public async Task<IActionResult> Create([FromForm] AddCategoryRequest request)
        {
            await _categoryService.AddAsync(request);
            return Ok();
        }
        [HttpPut]
        [Authorize(Role.Admin, Role.SuperAdmin)]
        public async Task<IActionResult> Update([FromForm] UpdateCategoryRequest request)
        {
            await _categoryService.UpdateAsync(request);
            return Ok();
        }
    }
}
