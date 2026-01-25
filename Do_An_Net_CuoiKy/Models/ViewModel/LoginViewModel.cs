using System.ComponentModel.DataAnnotations;

namespace Do_An_Net_CuoiKy.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Nhập email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Nhập mật khẩu")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
