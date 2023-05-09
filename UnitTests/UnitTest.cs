using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using VK_TEST.Controllers;
using VK_TEST.DataBase;
using VK_TEST.VK_T_Objects.Entity;
using VK_TEST.VK_T_Objects.Models;
using VK_TEST.VK_T_Objects.Services;
using Xunit;

namespace VK_TEST.UnitTests
{
    public class UnitTest_VK_TEST_Controller
    {

        public static List<User> Users = new List<User>() 
        { 
            new User{ Id = 1, Password= "password", Login = "Lev", Created_Date = DateTime.UtcNow, UserGroup = UserGroup.AllGroups[0], UserState = UserState.AllStates[0] },
            new User{ Id = 2, Password= "password", Login = "Lev0", Created_Date = DateTime.UtcNow, UserGroup = UserGroup.AllGroups[0], UserState = UserState.AllStates[0] },
            new User{ Id = 3, Password= "password", Login = "Lev1", Created_Date = DateTime.UtcNow, UserGroup = UserGroup.AllGroups[0], UserState = UserState.AllStates[0] },
        };

        [Fact]
        async public void Get()
        {
            var UserProvider = new Mock<IDbProvider<User>>();
            var UserGoupProvider = new Mock<IDbProvider<UserGroup>>();
            var UserStateProvider = new Mock<IDbProvider<UserState>>();
            var LimiterCounterProvider = new Mock<IDbProvider<LimiterCount>>();
            var logger = new Mock<ILogger<VK_TEST_Controller>>();
           
            UserProvider.Setup(x => x.Get(Users[0].Id)).Returns(Users[0]);
            var UserService = new UserService(UserProvider.Object, UserStateProvider.Object, UserGoupProvider.Object, LimiterCounterProvider.Object);
            VK_TEST_Controller controller = new VK_TEST_Controller(logger.Object, UserService);
            var curr_get = await controller.GetUserAsync(Users[0].Id);
            var corr_get = Results.Ok(Users[0]);

            var Jobj1 = JObject.FromObject(curr_get);
            var Jobj2 = JObject.FromObject(corr_get);
            Assert.Equal(Jobj1, Jobj2);
        }

        [Fact]
        async public void GetAll()
        {
            var UserProvider = new Mock<IDbProvider<User>>();
            var UserGoupProvider = new Mock<IDbProvider<UserGroup>>();
            var UserStateProvider = new Mock<IDbProvider<UserState>>();
            var LimiterCounterProvider = new Mock<IDbProvider<LimiterCount>>();
            var logger = new Mock<ILogger<VK_TEST_Controller>>();
          
            UserProvider.Setup(x => x.GetAll()).Returns(Users);
            UserProvider.Setup(x => x.Get(Users[0].Id)).Returns(Users[0]);

            var UserService = new UserService(UserProvider.Object, UserStateProvider.Object, UserGoupProvider.Object, LimiterCounterProvider.Object);
            VK_TEST_Controller controller = new VK_TEST_Controller(logger.Object, UserService);
            var curr_get = await controller.GetAllUsersAsync();
            var corr_get = Results.Ok(Users);

            var Jobj1 = JObject.FromObject(curr_get);
            var Jobj2 = JObject.FromObject(corr_get);
            Assert.Equal(Jobj1, Jobj2);
        }

        [Fact]
        async public void CreateAdminSuccess()
        {
            var UserProvider = new Mock<IDbProvider<User>>();
            var UserGoupProvider = new Mock<IDbProvider<UserGroup>>();
            var UserStateProvider = new Mock<IDbProvider<UserState>>();
            var LimiterCounterProvider = new Mock<IDbProvider<LimiterCount>>();
            var logger = new Mock<ILogger<VK_TEST_Controller>>();

            var auth_model = new AuthenticateModel() { Password = "1234", Login = "NEW_USER" };
            var new_user = new User() { Password = "1234", Login = "NEW_USER", Created_Date = DateTime.MinValue, Id = 0, UserGroup = UserGroup.AllGroups[1], UserState = UserState.AllStates[0] };

            UserProvider.Setup(x => x.Create(new_user)).Returns(new_user);
            UserProvider.Setup(x => x.GetFirstFromPredict(null)).Returns((User)null);
            LimiterCounterProvider.Setup(x => x.Get((int)ListLimiterCounter.Admin_Counter)).Returns(LimiterCount.AllLimiters[2]);
            LimiterCounterProvider.Setup(x => x.Get((int)ListLimiterCounter.Admin_Limit)).Returns(LimiterCount.AllLimiters[1]);
            LimiterCounterProvider.Setup(x => x.Update(LimiterCount.AllLimiters[2])).Returns(LimiterCount.AllLimiters[2]);

            UserProvider.Setup(x => x.Get(Users[0].Id)).Returns(Users[0]);
            var UserService = new UserService(UserProvider.Object, UserStateProvider.Object, UserGoupProvider.Object, LimiterCounterProvider.Object);
            VK_TEST_Controller controller = new VK_TEST_Controller(logger.Object, UserService);

            var curr_get = UserService.CreateAdmin(new_user);
            var corr_get = Results.Ok(new_user);

            var Jobj1 = JObject.FromObject(curr_get);
            var Jobj2 = JObject.FromObject(corr_get);

            Assert.Equal(Jobj1, Jobj2);
        }

        [Fact]
        public void CreateAdminException()
        {
            var UserProvider = new Mock<IDbProvider<User>>();
            var UserGoupProvider = new Mock<IDbProvider<UserGroup>>();
            var UserStateProvider = new Mock<IDbProvider<UserState>>();
            var LimiterCounterProvider = new Mock<IDbProvider<LimiterCount>>();
            var logger = new Mock<ILogger<VK_TEST_Controller>>();

            var new_user = new User() { Password = "1234", Login = "NEW_USER", Created_Date = DateTime.UtcNow, Id = 4, UserGroup = UserGroup.AllGroups[1], UserState = UserState.AllStates[0] };

            UserProvider.Setup(x => x.Create(new_user)).Returns(new_user);
            UserProvider.Setup(x => x.GetFirstFromPredict(null)).Returns((User)null);
            LimiterCounterProvider.Setup(x => x.Get((int)ListLimiterCounter.Admin_Counter)).Returns(LimiterCount.AllLimiters[2]);
            LimiterCounterProvider.Setup(x => x.Get((int)ListLimiterCounter.Admin_Limit)).Returns(LimiterCount.AllLimiters[1]);
            LimiterCounterProvider.Setup(x => x.Update(LimiterCount.AllLimiters[2])).Returns(LimiterCount.AllLimiters[2]);

            UserProvider.Setup(x => x.Get(Users[0].Id)).Returns(Users[0]);
            var UserService = new UserService(UserProvider.Object, UserStateProvider.Object, UserGoupProvider.Object, LimiterCounterProvider.Object);
            VK_TEST_Controller controller = new VK_TEST_Controller(logger.Object, UserService);

            var curr_get = UserService.CreateAdmin(new_user);
            var corr_get = Results.Problem(detail: "В системе уже есть админ", statusCode: 409);

            var Jobj1 = JObject.FromObject(curr_get);
            var Jobj2 = JObject.FromObject(corr_get);
            Assert.Equal(Jobj1, Jobj2);
        }
    }
}
