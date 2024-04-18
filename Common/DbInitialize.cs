using Classifieds.Data;
using Classifieds.Data.Entities;

namespace Common
{
    public class DbInitialize
    {   
        public DbInitialize()
        {
        }
        public static void Initialize(DataContext context)
        {
            try
            {
                //if (context.Users.Any()) return;

                //var user = new RegisterDto
                //{   
                //    AccountName = "admin",
                //    Email = "admin",
                //    Name = "admin",
                //    Password = "admin",
                //    PhoneNumber = "0123456789",
                //};
                if (context.Categories.Any()) return;
                var category = new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Category built-in",
                    Image = "https://vapa.vn/wp-content/uploads/2022/12/anh-dep-thien-nhien-the-gioi-001.jpg"
                };
                context.Categories.Add(category);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
            }
        }
    }
}
