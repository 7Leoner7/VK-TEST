namespace VK_TEST.VK_T_Objects.Entity
{
    public class UserGroup : BaseEntityModel
    {
        public string Code { get; set; }
        public string? Description { get; set; }

        /// <summary>
        /// List for init DB
        /// </summary>
        public static List<UserGroup> AllGroups = new List<UserGroup>()
        {
            new UserGroup() { Code = "User" , Description = "Обычный пользователь"},
            new UserGroup() { Code = "Admin" , Description = "Супер-пользователь" },
        };
    }
}
