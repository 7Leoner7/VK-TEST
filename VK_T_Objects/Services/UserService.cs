using Microsoft.AspNetCore.Mvc;
using VK_TEST.DataBase;
using VK_TEST.VK_T_Objects.Entity;
using VK_TEST.VK_T_Objects.Models;

namespace VK_TEST.VK_T_Objects.Services
{
    public interface IUserService
    {
        public Task<User?> Authenticate(string login, string password);
        
        public UserGroup? GetUserGroup(string login);

        public User? GetUser(int id);

        public User? GetUserByLogin(string login);

        public List<User>? GetAllUsers();

        public IResult CreateUser(User user);

        public IResult CreateAdmin(User user);

        public IResult ActivateUser(string login);

        public IResult BlockUser(string login);

        public IResult UpdateUser(UserModel model);

        public List<UserGroup> GetAllGroups();

        public List<UserState> GetAllStates();
    }

    public class UserService : IUserService
    {
        private IDbProvider<User> Users { get; set; }
        private IDbProvider<UserGroup> UserGroups { get; set; }
        private IDbProvider<UserState> UserStates { get; set; }
        private IDbProvider<LimiterCount> LimiterCount { get; set; }

        public UserService(
            IDbProvider<User> Users,
            IDbProvider<UserState> UserStates,
            IDbProvider<UserGroup> UserGroups,
            IDbProvider<LimiterCount> limiterCount)
        {
            this.Users = Users;
            this.UserGroups = UserGroups;
            this.UserStates = UserStates;
            LimiterCount = limiterCount;
        }

        async public Task<User?> Authenticate(string login, string password)=>
            await Users.GetAsyncFirstFromPredict(u => (u.Login == login)&&(u.Password == password));

        public UserGroup? GetUserGroup(string login) =>
            Users.GetFirstFromPredict(u => u.Login == login)?.UserGroup;

        
        public User? GetUser(int id) =>
         Users.Get(id);
        
        public User? GetUserByLogin(string login) =>
             Users.GetFirstFromPredict(x => x.Login == login);
        
        public List<User>? GetAllUsers() =>
             Users.GetAll();

        #region [ForActions]
        public IResult CreateUser(User user)
        {
            try
            {
                return Results.Ok(Users.Create(user));
            }
            catch (Exception ex)
            {
                return Results.Problem(detail: ex.Message, statusCode: 409);
            }
        }

        public IResult CreateAdmin(User user)
        {
            try
            {
                var ac = LimiterCount.Get((int)ListLimiterCounter.Admin_Counter);
                var al = LimiterCount.Get((int)ListLimiterCounter.Admin_Limit);
                if (ac.Count >= al.Count)
                    throw new Exception("Admin is already exist");
                ac.Count++;
                LimiterCount.Update(ac);
                return Results.Ok(Users.Create(user));
            }
            catch (Exception ex)
            {
                return Results.Problem(detail: ex.Message, statusCode: 409);
            }
        }

        public IResult ActivateUser(string login)
        {
            try
            {
                var user = Users.GetFirstFromPredict(x => x.Login == login);
                if (user == null) throw new Exception("Пользователя с данным логином не существует");
                user.UserState = UserState.AllStates[0];
                return Results.Ok(Users.Update(user));
            }
            catch (Exception ex)
            {
                return Results.Problem(detail: ex.Message);
            }
        }

        public IResult BlockUser(string login)
        {
            try
            {
                var user = Users.GetFirstFromPredict(x => x.Login == login);
                if (user == null) throw new Exception("Пользователя с данным логином не существует");
                user.UserState = UserState.AllStates[1];
                return Results.Ok(Users.Update(user));
            }
            catch (Exception ex)
            {
                return Results.Problem(detail: ex.Message);
            }
        }
        
        public IResult UpdateUser(UserModel model)
        {
            try
            {
                User? user = Users.GetFirstFromPredict(x => x.Login == model.Login);
                if (user == null) throw new Exception("Пользователя с данным логином нету");

                user.Created_Date = model.Created_Date;
                user.UserState = UserStates.Get(model.UserStateID) ?? user.UserState;
                user.UserGroup = UserGroups.Get(model.UserGroupID) ?? user.UserGroup;
                user.Password = model.Password;

                return Results.Ok(Users.Update(user));
            }
            catch (Exception ex)
            {
                return Results.Problem(detail: ex.Message);
            }
        }

        public List<UserGroup> GetAllGroups() =>
            UserGroups.GetAll();

        public List<UserState> GetAllStates() =>
           UserStates.GetAll();
        #endregion
    }
}
