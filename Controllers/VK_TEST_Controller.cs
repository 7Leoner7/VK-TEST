using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using VK_TEST.VK_T_Objects.Entity;
using VK_TEST.VK_T_Objects.Models;
using VK_TEST.VK_T_Objects.Services;

namespace VK_TEST.Controllers
{
    [Authorize(AuthenticationSchemes = "Basic")]
    [ApiController]
    [Tags("VK TEST API")]
    [Route("[controller]")]
    public class VK_TEST_Controller : ControllerBase
    {
        private readonly ILogger<VK_TEST_Controller> _logger;
        static private Queue<User> QueneOfRegisterUser { get; set; } = new();
        private IUserService UserService { get; set; }

        public VK_TEST_Controller(
            ILogger<VK_TEST_Controller> logger,
            IUserService userService)
        {
            _logger = logger;
            UserService = userService;
        }

        #region [NonAction]
        private void ThrowAdmin()
        {
            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
            var username = credentials[0];

            var group = this.UserService.GetUserGroup(username) ?? UserGroup.AllGroups[0];
            if (group.Code != UserGroup.AllGroups[1].Code) throw new Exception("You don't have permission");
        }

        /// <summary>
        /// Задержка. Нужна для демонстрации работы CreateUser
        /// </summary>
        /// <returns></returns>
        private async Task DelayDequene()
        {
            await Task.Run(async () =>
            {
                await Task.Delay(5000);
                QueneOfRegisterUser.Dequeue();
            });
        }

        private bool IsUserContainsInQofRU(User user) =>
            QueneOfRegisterUser.FirstOrDefault(u => u.Login == user.Login) != null;

        private bool IsUserContainsInDB(User user) =>
            UserService.GetUserByLogin(user.Login) != null;
        #endregion

        #region [HttpActions]
        #region [User]
        [Tags("User")]
        [HttpGet("/Api/User/Get")]
        async public Task<IResult> GetUserAsync(int id) =>
             await Task.Run(() => Results.Ok(UserService.GetUser(id)));

        [Tags("User")]
        [HttpGet("/Api/User/GetByLogin")]
        async public Task<IResult> GetUserByLoginAsync(string login) =>
             await Task.Run(() => Results.Ok(UserService.GetUserByLogin(login)));

        [Tags("User")]
        [HttpGet("/Api/User/GetAll")]
        async public Task<IResult> GetAllUsersAsync() =>
            await Task.Run(() => Results.Ok(UserService.GetAllUsers()));

        [Tags("User")]
        [AllowAnonymous]
        [HttpPost("/Api/User/RegisterUser")]
        async public Task<IResult> CreateUserAsync([FromBody] AuthenticateModel auth_model) =>
            await Task.Run(async () =>
            {
                try
                {
                    User user = new User() { Created_Date = DateTime.UtcNow, Login = auth_model.Login, Password = auth_model.Password, 
                        UserGroup = UserService.GetAllGroups().First(x => x.Code == "User"), 
                        UserState = UserService.GetAllStates().First(x => x.Code == "Active")
                    };

                    if (IsUserContainsInQofRU(user)) throw new Exception("User with this login is exist");
                    if (IsUserContainsInDB(user)) throw new Exception("User with this login is exist");

                    QueneOfRegisterUser.Enqueue(user);
                    await DelayDequene();

                    return UserService.CreateUser(user);
                }
                catch (Exception ex)
                {
                    return Results.Problem(detail: ex.Message, statusCode: 409);
                }
            });

        [Tags("User")]
        [HttpPost("/Api/User/RegisterAdmin")]
        async public Task<IResult> CreateAdminAsync([FromBody] AuthenticateModel auth_model) =>
            await Task.Run(async () =>
            {
                try
                {
                    //ThrowAdmin();
                    User user = new User() { Created_Date = DateTime.UtcNow, Login = auth_model.Login, Password = auth_model.Password,
                        UserGroup = UserService.GetAllGroups().First(x => x.Code == "Admin"),
                        UserState = UserService.GetAllStates().First(x => x.Code == "Active")
                    };

                    if (IsUserContainsInQofRU(user)) throw new Exception("User with this login is exist");
                    if (IsUserContainsInDB(user)) throw new Exception("User with this login is exist");

                    QueneOfRegisterUser.Enqueue(user);
                    await DelayDequene();

                    return UserService.CreateAdmin(user);
                }
                catch (Exception ex)
                {
                    return Results.Problem(detail: ex.Message);
                }
            });

        [Tags("User")]
        [HttpPost("/Api/User/Update")]
        async public Task<IResult> UpdateUserAsync([FromBody] UserModel user) =>
            await Task.Run(() =>
            {
                try
                {
                    ThrowAdmin();
                    return UserService.UpdateUser(user);
                }
                catch (Exception ex)
                {
                    return Results.Problem(detail: ex.Message);
                }
            });

        [Tags("User")]
        [HttpGet("/Api/User/Activate")]
        async public Task<IResult> ActivateUserAsync(string login) =>
            await Task.Run(() => UserService.ActivateUser(login));

        [Tags("User")]
        [HttpDelete("/Api/User/BlockUser")]
        async public Task<IResult> BlockUserAsync(string login) =>
            await Task.Run(() => UserService.BlockUser(login));
        #endregion

        [Tags("Group")]
        [HttpGet("/Api/Group/GetAll")]
        async public Task<IResult> GetAllGroupsAsync() =>
            await Task.Run(() => Results.Ok(UserService.GetAllGroups()));

        [Tags("State")]
        [HttpGet("/Api/State/GetAll")]
        async public Task<IResult> GetAllStatesAsync() =>
            await Task.Run(() => Results.Ok(UserService.GetAllStates()));
        #endregion
    }
}