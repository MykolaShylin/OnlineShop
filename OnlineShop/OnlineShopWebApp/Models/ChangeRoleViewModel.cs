using OnlineShop.DB.Models;
using System.Collections.Generic;

namespace OnlineShopWebApp.Models
{
    public class ChangeRoleViewModel
    {
        public string UserId { get; set; }
        public string Login { get; set; }
        public List<Role> AllRoles { get; set; }
        public IList<string> UserRoles { get; set; }
        public ChangeRoleViewModel()
        {
            AllRoles = new List<Role>();
            UserRoles = new List<string>();
        }
    }
}
