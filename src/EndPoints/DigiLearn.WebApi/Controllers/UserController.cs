using Common.Application;
using Common.Application.SecurityUtil;
using DigiLearn.WebApi.Infrastructure;
using DigiLearn.WebApi.Infrastructure.JwtUtils;
using DigiLearn.WebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Drawing;
using Microsoft.AspNetCore.Authorization;
using UserModule.Core.Commands.Notifications.Create;
using UserModule.Core.Commands.Notifications.Delete;
using UserModule.Core.Commands.Roles.Create;
using UserModule.Core.Commands.Roles.Edit;
using UserModule.Core.Commands.Users.ChangeAvatar;
using UserModule.Core.Commands.Users.ChangePassword;
using UserModule.Core.Commands.Users.Edit;
using UserModule.Core.Commands.Users.FullEdit;
using UserModule.Core.Commands.Users.Register;
using UserModule.Core.Queries._DTOs;
using UserModule.Core.Services;
using UserModule.Data.Entities.Roles;
using UserModule.Core.Commands.Notifications.DeleteAll;
using UserModule.Core.Commands.Notifications.Seen;

namespace DigiLearn.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ApiController
    {
        private readonly IConfiguration _configuration;
        private readonly IUserFacade _facade;
        private readonly IRoleFacade _roleFacade;
        private readonly INotificationFacade _notificationFacade;

        public UserController(IUserFacade facade, IConfiguration configuration, IRoleFacade roleFacade, INotificationFacade notificationFacade)
        {
            _facade = facade;
            _configuration = configuration;
            _roleFacade = roleFacade;
            _notificationFacade = notificationFacade;
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

        #region User
        [HttpPatch("EditUserProfile")]
        [Authorize]
        public async Task<ApiResult> EditUserProfile(EditUserCommand command)
        {
            return CommandResult(await _facade.EditUserProfile(command));
        }

        [HttpPatch("EditUser")]
        [Authorize]
        public async Task<ApiResult> EditUser(FullEditUserCommand command)
        {
            return CommandResult(await _facade.EditUser(command));
        }

        [HttpPatch("ChangeAvatar")]
        [Authorize]
        public async Task<ApiResult> ChangeAvatar(ChangeUserAvatarCommand command)
        {
            return CommandResult(await _facade.ChangeAvatar(command));
        }

        [HttpPatch("ChangeUserPassword")]
        [Authorize]
        public async Task<ApiResult> ChangePassword(ChangeUserPasswordCommand command)
        {
            return CommandResult(await _facade.ChangePassword(command));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <example>
        /// نمونه ریکوئست  => api/User/GetUserByPhoneNumber?{phoneNumber}
        /// </example>
        /// <returns></returns>
        [HttpGet("GetUserByPhoneNumber")]
        public async Task<ApiResult<UserDto?>> GetUserByPhoneNumber(string phoneNumber)
        {
            return QueryResult(await _facade.GetUserByPhoneNumber(phoneNumber));
        }

        [HttpGet("GetUserById")]
        public async Task<ApiResult<UserDto?>> GetUserById(Guid userId)
        {
            return QueryResult(await _facade.GetById(userId));
        }

        [HttpGet("GetUserByFilter")]
        public async Task<ApiResult<UserFilterResult>> GetUserByFilter(UserFilterParams filterParams)
        {
            return QueryResult(await _facade.GetByFilter(filterParams));
        }
        #endregion

        #region Role

        [HttpPost("CreateRole")]
        [Authorize]
        public async Task<ApiResult> CreateRole(CreateRoleCommand command)
        {
            return CommandResult(await _roleFacade.Create(command));
        }

        [HttpPatch("EditRole")]
        [Authorize]
        public async Task<ApiResult> EditRole(EditRoleCommand command)
        {
            return CommandResult(await _roleFacade.Edit(command));
        }

        [HttpDelete("DeleteRole")]
        [Authorize]
        public async Task<ApiResult> DeleteRole(Guid roleId)
        {
            return CommandResult(await _roleFacade.Delete(roleId));
        }

        [HttpGet("GetRoleById")]
        [Authorize]
        public async Task<ApiResult<Role?>> GetRoleById(Guid roleId)
        {
            return QueryResult(await _roleFacade.GetRoleById(roleId));
        }

        [HttpGet("GetAllRoles")]
        [Authorize]
        public async Task<ApiResult<List<Role>>> GetAllRoles()
        {
            return QueryResult(await _roleFacade.GetAllRoles());
        }
        #endregion

        #region Notification   
       
        [HttpPost("CreateNotification")]
        public async Task<ApiResult> CreateNotification(CreateNotificationCommand command)
        {
            return CommandResult(await _notificationFacade.Create(command));
        }
        
        [HttpDelete("DeleteNotification")]
        public async Task<ApiResult> DeleteNotification(DeleteNotificationCommand command)
        {
            return CommandResult(await _notificationFacade.Delete(command));
        }
        
        [HttpDelete("DeleteAll")]
        public async Task<ApiResult> DeleteAllNotification(DeleteAllNotificationCommand command)
        {
            return CommandResult(await _notificationFacade.DeleteAll(command));
        }
        
        [HttpPatch("SeenNotification")]
        public async Task<ApiResult> SeenNotification(SeenNotificationCommand command)
        {
            return CommandResult(await _notificationFacade.Seen(command));
        }
        
        [HttpGet("GetNotificationByFilter")]
        public async Task<ApiResult<NotificationFilterResult>> GetNotificationByFilter(NotificationFilterParams filterParams)
        {
            return QueryResult(await _notificationFacade.GetByFilter(filterParams));
        }
        
        #endregion

    }
}
