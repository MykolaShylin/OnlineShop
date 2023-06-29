using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.DB.Models
{
    public class Role : IdentityRole
    {
        public string Description { get; set; }
    }
}
