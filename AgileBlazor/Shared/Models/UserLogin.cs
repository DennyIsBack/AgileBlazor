using System.ComponentModel.DataAnnotations;

namespace AgileBlazor.Shared
{
    public class UserLogin
    {
        public class ChangePasswordModel
        {
            public string User { get; set; }

            public string OldPassword { get; set; }

            public string NewPassword { get; set; }

            public int Days { get; set; }
        }

        public class LoginViewModel
        {
            [Required(ErrorMessage = "O nome deve ser informado")]
            [Display(Name = "Usuário")]
            public string User { get; set; }

            [Required(ErrorMessage = "A senha deve ser informada ")]
            [DataType(DataType.Password)]
            [Display(Name = "Senha")]
            public string Password { get; set; }

            public Guid AuthID { get; set; }
        }

        public class UserToken
        {
            public bool Success { get; set; }
            public string? Token { get; set; }
            public DateTime Expiry { get; set; }
        }
    }
}