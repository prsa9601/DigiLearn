using Common.Application;
using Common.Application.SecurityUtil;
using DigiLearn.WebApi.Infrastructure;
using DigiLearn.WebApi.Infrastructure.JwtUtils;
using DigiLearn.WebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UserModule.Core.Commands.Users.Register;
using UserModule.Core.Services;

namespace DigiLearn.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ApiController
    {
        private readonly IConfiguration _configuration;
        private readonly IUserFacade _facade;

        public UserController(IUserFacade facade, IConfiguration configuration)
        {
            _facade = facade;
            _configuration = configuration;
        }

        #region Auth

        [HttpPost("RegisterUser")]
        public async Task<ApiResult<Guid>> Register([FromBody] RegisterUserCommand command)
        {
            return CommandResult<Guid>(await _facade.RegisterUser(command));
        }

        [HttpPost("LoginUser")]
        public async Task<ApiResult<LoginResultDto?>> LoginUser(
            [FromBody] UserLoginCommandViewModel command)
        {
            var user = await _facade.GetUserByPhoneNumber(command.PhoneNumber);
            if (user == null)
            {
                return CommandResult(
                    OperationResult<LoginResultDto>.Error("کاربری با مشخصات وارد شده یافت نشد"));
            }
            if (Sha256Hasher.IsCompare(user.Password, command.Password) == false)
            {
                return CommandResult(
                    OperationResult<LoginResultDto>.Error("کاربری با مشحصات وارد شده یافت نشد"));
            }

            var token = JwtTokenBuilder.BuildToken(user, command.RememberMe, _configuration);
            //JwtTokenBuilder
            return CommandResult<LoginResultDto>(
                OperationResult<LoginResultDto>.Success
                (new LoginResultDto { Token = token }));
        }

        #endregion
    }
}
