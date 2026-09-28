using Ecom.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Core.Interfaces
{
    public interface IAuthRepositry
    {
        Task<string> RegisterAsync(RegisterDTO dto);
        Task<string> LoginAsync(LoginDTO dto);
        Task<bool> ActivateAccount(ActivateAccountDTO activateAccountDTO);
        Task SendEmail(string email, string code, string component, string subject, string message);
        Task<bool> SendEmailForForgetPassword(string email);
        Task<string> ResetPassword(ResetPasswordDTO resetPasswordDTO);


    }
}
