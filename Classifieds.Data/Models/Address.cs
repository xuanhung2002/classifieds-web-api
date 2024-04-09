using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classifieds.Data.Models
{
    public class Address
    {
        public string Provine { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string? SpecificAddress { get; set; }
    }
}
