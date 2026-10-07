using AutoMapper;
using Ecom.Core.DTOs;
using Ecom.Core.Entites;
using Ecom.Core.Interfaces;
using Ecom.Core.Services;
using Ecom.Core.Sharing;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Repositries
{
    public class AuthRepositry : IAuthRepositry
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager; 
        private readonly IEmailService _emailService;
        private readonly IGenerateToken _generateToken;
        private readonly IMapper _mapper; 
        public AuthRepositry(UserManager<ApplicationUser> userManager, IEmailService emailService, 
            SignInManager<ApplicationUser> signInManager, IGenerateToken generateToken, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _generateToken = generateToken;
            _mapper = mapper;   

        }
        public async Task<string> RegisterAsync(RegisterDTO registerDTO)
        {
            if(registerDTO == null)
            {
                return null;
            }
            if(await _userManager.FindByEmailAsync(registerDTO.Email) is not null)
            {
                return "This Email is already registerd";
            }
            if(await _userManager.FindByNameAsync(registerDTO.UserName) is not null)
            {
                return "This User Name is already registerd";
            }
            ApplicationUser user = new ApplicationUser()
            {
                Email = registerDTO.Email,
                UserName = registerDTO.UserName,
                DisplayName = registerDTO.DisplayName
            };
            var isAdd = await _userManager.CreateAsync(user,registerDTO.Password);

            if (!isAdd.Succeeded)
            {
                return isAdd.Errors.ToList()[0].Description;
            }
            //  `To Activate Email 
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await  SendEmail(user.Email, token, "active", "Active Email", "Please Activate Your Email");

            return "User Registered Successfully";
        }

        public async Task<bool> ActivateAccount(ActivateAccountDTO activateAccountDTO)
        {
            var findUser = await _userManager.FindByEmailAsync(activateAccountDTO.Email);
            if(findUser == null)
            {
                return false;
            }
            var isConfirmed = await _userManager.ConfirmEmailAsync(findUser, activateAccountDTO.Token);
            if (isConfirmed.Succeeded)
            {
                return true;
            }
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(findUser);

            await SendEmail(findUser.Email, token, "active", "Active Email", "Please Activate Your Email..click on button sent in E-mail to active");

            return false;
        }
        public async Task<string> LoginAsync(LoginDTO loginDTO)
        {
            if(loginDTO == null)
            {
                return null;
            }
            var findUser = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (findUser == null)
            {
                return "Please, Check your E-mail or Password, some thing wrong";
            }

            if (!findUser.EmailConfirmed)
            {
                string token = await _userManager.GenerateEmailConfirmationTokenAsync(findUser);
                
                await SendEmail(findUser.Email, token, "active", "Active Email", "Please Activate Your Email..click on button to active");
             
                return "Please Confirm Your Email first, We have sent activate message to your E-mail";
            }
            var result = await _signInManager.CheckPasswordSignInAsync(findUser, loginDTO.Password,true);
           
            if (result.Succeeded)
            {
                return await _generateToken.GetAndCreateToken(findUser);
            }
            return "Please, Check your E-mail or Password, some thing wrong";
        }

        public async Task SendEmail(string email, string code, string component, string subject,string message)
        {
            var result = new EmailDTO(email,
                subject,
                EmailStringBody.Send(email, code, component, message));

            await _emailService.SendEmail(result);
        }
        
        public async Task<bool> SendEmailForForgetPassword(string email)
        {
            var findUser = await _userManager.FindByEmailAsync(email);
            if(findUser is null)
            {
                return false;
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(findUser);

           await SendEmail(email, token, "active", "reset-password", "Click on this button to Reset Your Password!");
           
            return true;
        }

        public async Task<string> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            var findUser = await _userManager.FindByEmailAsync(resetPasswordDTO.Email);
            
            if( findUser is null)
            {
                return null;
            }
            var result = await _userManager.ResetPasswordAsync(findUser, resetPasswordDTO.token, resetPasswordDTO.Password);

            if (result.Succeeded)
            {
                return "Password change successfully.";
            }
            return result.Errors.ToList()[0].Description;
        }

        public async Task<bool> UpdateAddress(string email, ShipAddressDTO address)
        {
            var FindUser = await _userManager.Users
                .Include(u=>u.address)
                .FirstOrDefaultAsync(u=>u.Email==email);

            if(FindUser is null)
            {
                return false;
            }
            if(FindUser.address == null)
            {
                FindUser.address = _mapper.Map<Address>(address); 
            }
            else
            {
                _mapper.Map(address,FindUser.address);
            }
            var result = await _userManager.UpdateAsync(FindUser);

            return result.Succeeded;

        }

        public async Task<ShipAddressDTO> GetAddress(string email)
        {
            var FindUser = await _userManager.Users
             .Include(u => u.address)
             .FirstOrDefaultAsync(u => u.Email == email);

            if (FindUser is null)
            {
                return null;
            }

            return _mapper.Map<ShipAddressDTO>(FindUser.address);
        }
    }
}
