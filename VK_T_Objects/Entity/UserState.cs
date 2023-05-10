namespace VK_TEST.VK_T_Objects.Entity
{
    public class UserState : BaseEntityModel
    {
        public string Code { get; set; }
        public string? Description { get; set; }

        /// <summary>
        /// List for init DB
        /// </summary>
        public static List<UserState> AllStates = new List<UserState>()
        {
            new UserState() { Code = "Active" , Description = "Активный аккаунт" },
            new UserState() { Code = "Blocked" , Description = "Заблокированный аккаунт" },
        };
    }
}
