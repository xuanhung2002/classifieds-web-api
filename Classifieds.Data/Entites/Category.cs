using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classifieds.Data.Entites
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public string? Image { get; set; }
    }
}
