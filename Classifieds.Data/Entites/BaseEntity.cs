namespace Classifieds.Data.Entites
{
    public class IdEntity
    {
        public Guid Id { get; set; }
    }
    public class BaseEntity : IdEntity
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
