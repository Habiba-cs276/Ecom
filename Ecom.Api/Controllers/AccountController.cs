using AutoMapper;
using Ecom.Api.Helper;
using Ecom.Core.DTOs;
using Ecom.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ecom.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AccountController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            string result = await _unitOfWork.Auth.RegisterAsync(registerDTO);
          
            if (result != "User Registered Successfully")
                return BadRequest(new ResponseAPI<string>(400, null, result));

            return Ok(new ResponseAPI<string>(200,null,result));
        }
        [HttpPost("LogIn")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            string result =  await _unitOfWork.Auth.LoginAsync(loginDTO);

            if (result.StartsWith("Please"))
            {
                return BadRequest(new ResponseAPI<string>(400, null, result));
            }
            Response.Cookies.Append("token", result, new CookieOptions
            {
                Secure = true,
                HttpOnly = true,
                Domain = "localhost",
                Expires = DateTime.Now.AddDays(1),
                IsEssential =true,
                SameSite =SameSiteMode.None,
            });
            return Ok(new ResponseAPI<string>(200,  null, result)); 
        }

        [HttpPost("active-account")]
        public async Task<IActionResult> Active(ActivateAccountDTO activateAccountDTO)
        {
            var result = await _unitOfWork.Auth.ActivateAccount(activateAccountDTO);
            if (!result)
            {
                return BadRequest(new ResponseAPI<string>(400));
            }
            return Ok(new ResponseAPI<string>(200));
        }

        [HttpGet("send-email-forget-password")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            var result= await _unitOfWork.Auth.SendEmailForForgetPassword(email);
            if (!result)
            {
                return BadRequest(new ResponseAPI<string>(400));
            }
            return Ok(new ResponseAPI<string>(200));
        }
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            var result = await _unitOfWork.Auth.ResetPassword(resetPasswordDTO);
            if(string.IsNullOrEmpty(result) || !result.StartsWith("Password"))
            {
                return BadRequest(new ResponseAPI<string>(400));
            }
            return Ok(new ResponseAPI<string>(200));
        }

        [HttpPut("Update-Address")]
        [Authorize]
        public async Task<IActionResult> UpdateAddress(ShipAddressDTO shipAddressDTO)
        {
            var UserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if(UserEmail == null)
            {
                return BadRequest(new ResponseAPI<string>(400));
            }
            var result = await _unitOfWork.Auth.UpdateAddress(UserEmail, shipAddressDTO);
            if(!result)
            {
                return BadRequest(new ResponseAPI<string>(400));
            }
            return Ok(new ResponseAPI<string>(200,null,"Address Updated Succefully!"));

        }
        [HttpGet("Get-User-Address")]
        [Authorize]
        public async Task<IActionResult> GetAddress()
        {
            var UserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(UserEmail))
            {
                return Unauthorized(new ResponseAPI<string>(401, null, "User not authorized"));
            }
            var result= await _unitOfWork.Auth.GetAddress(UserEmail);
            if (result is null)
            {
                return NotFound(new ResponseAPI<string>(404, null, "No address found for this user"));
            }
            return Ok(new ResponseAPI<ShipAddressDTO>(200, result));

        }
    }
}
