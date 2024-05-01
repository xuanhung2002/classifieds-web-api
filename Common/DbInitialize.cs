using Classifieds.Data;
using Classifieds.Data.DTOs.UserDtos;
using Classifieds.Data.Entities;
using Classifieds.Data.Models;
using System.Security.Cryptography;
using System.Text;
using static System.Net.WebRequestMethods;

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
                if (context.Users.Any()) return;

                using var hashFunc = new HMACSHA256();
                var passwordBytes = Encoding.UTF8.GetBytes("admin@12345");

                var newUser = new User
                {
                    AccountName = "admin",
                    Name = "admin",
                    PhoneNumber = "0384651408",
                    Email = "classifieds.system@outlook.com",
                    Role = Role.Admin,
                    PasswordHash = hashFunc.ComputeHash(passwordBytes),
                    PasswordSalt = hashFunc.Key
                };

                context.Users.Add(newUser);
                context.SaveChanges();

                if (context.Categories.Any()) return;
                var categories = new List<Category>
                {
                    new Category
                    {
                        Id = Guid.NewGuid(),
                        Name = "Motobike",
                        Image = "https://congthuong-cdn.mastercms.vn/stores/news_dataimages/thuylinh/062021/11/16/in_article/1430_Vision_tiYp_tYc_la_mYu_xe_ban_chYy_nhYt_trong_phan_khuc_xe_tay_ga_cYa_Honda_ViYt_Nam.jpg?rt=20210611161431"
                    },
                    new Category
                    {
                        Id = Guid.NewGuid(),
                        Name = "Car",
                        Image = "https://vcdn-vnexpress.vnecdn.net/2022/08/26/Lamborghini-Urus-1-8513-1661497000.jpg"
                    },
                    new Category
                    {
                        Id = Guid.NewGuid(),
                        Name = "Furniture",
                        Image = "https://time.com/shopping/static/22665237c09337cd06e958d5e74f521f/4febf/Best-Online-Furniture-Stores.webp"
                    },
                    new Category
                    {
                        Id = Guid.NewGuid(),
                        Name = "Clothing",
                        Image = "https://cdn.sanity.io/images/v6oximkk/production/15b4d96dc748fa1df20394f76c8c20b272d7919d-1500x1000.jpg?w=1500&h=1000&auto=format"
                    },
                    new Category
                    {
                        Id = Guid.NewGuid(),
                        Name = "Book",
                        Image = "https://static.scientificamerican.com/sciam/cache/file/1DDFE633-2B85-468D-B28D05ADAE7D1AD8_source.jpg?w=1200"
                    },
                    new Category
                    {
                        Id = Guid.NewGuid(),
                        Name = "Electronic",
                        Image = "https://cdn.nguyenkimmall.com/images/detailed/615/SAMSUNG_TULANH-MAYGIAT.jpg"
                    },
                    new Category
                    {
                        Id = Guid.NewGuid(),
                        Name = "Phone and Computer",
                        Image = "https://securityintelligence.com/wp-content/webp-express/webp-images/doc-root/wp-content/uploads/2019/04/external_mobile-security-versus-desktop-and-laptop-security-is-there-even-a-difference-anymore.jpg.webp"
                    },
                    new Category
                    {
                        Id = Guid.NewGuid(),
                        Name = "Television",
                        Image = "https://www.sony.com.vn/image/ff0a71866476e0ad65b8d848f2d7b40c?fmt=pjpeg&wid=1014&hei=396&bgcolor=F1F5F9&bgc=F1F5F9"
                    },
                    // Thêm các danh mục khác tại đây...
                };

                foreach (var category in categories)
                {
                    context.Categories.Add(category);
                }

                context.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
