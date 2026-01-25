using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Do_An_Net_CuoiKy.Models
{
    public class AppUserModel : IdentityUser
    {
        [StringLength(255)]
        public string? Address { get; set; }
        public string? FullName { get; set; }
        // Navigation properties
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
