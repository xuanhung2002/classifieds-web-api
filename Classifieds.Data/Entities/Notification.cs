namespace Classifieds.Data.Entities
{
    public class Notification : BaseEntity
    {
        public string Content { get; set; }
        public Guid UserId { get; set; }
    }
}
