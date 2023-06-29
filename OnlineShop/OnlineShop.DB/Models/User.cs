using Microsoft.AspNetCore.Identity;
using OnlineShop.DB.Contexts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace OnlineShop.DB.Models
{
    public class User : IdentityUser
    {
        [AllowNull]
        public string? RealName { get; set; }
        [AllowNull]
        public string? SerName { get; set; }

        [AllowNull]
        public string? NikName { get; set; }
        [AllowNull]
        public string? Avatar { get; set; } = null;
        public List<ComparingProducts> ComparingProducts { get; set; } = new List<ComparingProducts>();

    }
}
