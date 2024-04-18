using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classifieds.Data.DTOs.GoogleAuthDtos
{
    public class GoogleLoginRequest
    {
        [Required]
        public string TokenId { get; set; }
    }
}
