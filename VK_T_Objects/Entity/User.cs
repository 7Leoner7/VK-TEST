namespace VK_TEST.VK_T_Objects.Entity
{
    public class User : BaseEntityModel
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public DateTime Created_Date { get; set; }
        public UserGroup? UserGroup { get; set; }
        public UserState? UserState { get; set; }
    }
}
