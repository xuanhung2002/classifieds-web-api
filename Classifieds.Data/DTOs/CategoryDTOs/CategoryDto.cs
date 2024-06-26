namespace Classifieds.Data.DTOs
{
    public class CategoryDto : BaseDto
    {   
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Image { get; set; }
    }
}
