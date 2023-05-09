namespace VK_TEST.VK_T_Objects.Entity
{
    public enum ListLimiterCounter
    {
         InitializedDB = 1,
         Admin_Limit = 2,
         Admin_Counter = 3
    }

    public class LimiterCount : BaseEntityModel
    {
        public string Header { get; set; }
        public int Count { get; set; }

        /// <summary>
        /// Список для инициализации БД
        /// </summary>
        public static List<LimiterCount> AllLimiters = new() {
            new LimiterCount() { Header = "InitializedDB", Count = 1 },
            new LimiterCount() { Header = "Admin_Limit", Count = 1 },
            new LimiterCount() { Header = "Admin_Counter", Count = 0 }
        };
    }
}
