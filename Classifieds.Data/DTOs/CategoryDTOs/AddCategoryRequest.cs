using Microsoft.AspNetCore.Http;

namespace Classifieds.Data.DTOs
{
    public class AddCategoryRequest
    {
        public string Name { get; set; }
        public IFormFile Image { get; set; }
    }
}
