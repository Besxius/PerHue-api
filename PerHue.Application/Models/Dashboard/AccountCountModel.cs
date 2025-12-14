namespace PerHue.Application.Models.Dashboard
{
    /// <summary>
    /// Account count statistics model
    /// </summary>
    public class AccountCountModel
    {
        public int TotalAccounts { get; set; }
        public int ActiveAccounts { get; set; }
        public int InactiveAccounts { get; set; }
        public int BannedAccounts { get; set; }
        public int ExpertAccounts { get; set; }
        public int RegularAccounts { get; set; }
        public int NewAccountsThisMonth { get; set; }
        public int NewAccountsThisWeek { get; set; }
        public int NewAccountsToday { get; set; }
        public Dictionary<string, int> AccountsByRole { get; set; } = new();
        public Dictionary<string, int> AccountsByDay { get; set; } = new();
        public Dictionary<string, int> AccountsByMonth { get; set; } = new();
    }
}