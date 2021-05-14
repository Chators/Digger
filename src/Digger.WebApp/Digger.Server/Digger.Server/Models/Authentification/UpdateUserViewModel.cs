using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Digger.Server.Models.Authentification
{
    public class UpdateUserViewModel
    {
        public string Pseudo { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; }
    }
}
