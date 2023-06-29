namespace Dan.Models
{
    public struct LeaderboardSearchQuery
    {
        public int Skip { get; set; }
        public int Take { get; set; }
        public string Username { get; set; }
        public TimePeriodType TimePeriod { get; set; }

        public static LeaderboardSearchQuery Default => new LeaderboardSearchQuery
        {
            Skip = 0,
            Take = 0,
            Username = "",
            TimePeriod = (int) TimePeriodType.AllTime
        };
        
        public static LeaderboardSearchQuery Paginated(int skip, int take) => 
            ByUsernameAndTimePaginated("", TimePeriodType.AllTime, skip, take);
        
        public static LeaderboardSearchQuery ByUsername(string username) => 
            ByUsernamePaginated(username, 5, 5);
        
        public static LeaderboardSearchQuery ByUsernamePaginated(string username, int prev, int next) => 
            ByUsernameAndTimePaginated(username, TimePeriodType.AllTime, prev, next);
        
        public static LeaderboardSearchQuery ByTimePeriod(TimePeriodType timePeriod) => 
            ByTimePeriodPaginated(timePeriod, 0, 0);
        
        public static LeaderboardSearchQuery ByTimePeriodPaginated(TimePeriodType timePeriod, int skip, int take) => 
            ByUsernameAndTimePaginated("", timePeriod, skip, take);
        
        public static LeaderboardSearchQuery ByUsernameAndTime(string username, TimePeriodType timePeriod) => 
            ByUsernameAndTimePaginated(username, timePeriod, 0, 0);
        
        public static LeaderboardSearchQuery ByUsernameAndTimePaginated(string username, TimePeriodType timePeriod, int skip, int take) => new LeaderboardSearchQuery
        {
            Skip = skip,
            Take = take,
            Username = username,
            TimePeriod = timePeriod
        };
        
        public Entry[] Filter(Entry[] entries, bool ascendingOrder)
        {
            var searchQuery = this;
            ulong timePeriod = searchQuery.TimePeriod switch
            {
                TimePeriodType.Today => 1,
                TimePeriodType.ThisWeek => 7,
                TimePeriodType.ThisMonth => 30,
                TimePeriodType.ThisYear => 365,
                _ => 0
            };
            
            if (timePeriod != 0)
            {
                var timePeriodInSeconds = timePeriod * 24 * 60 * 60;
                var now = (ulong) System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var timePeriodAgo = now - timePeriodInSeconds;
                entries = System.Array.FindAll(entries, entry => entry.Date >= timePeriodAgo);
            }
            
            if (!string.IsNullOrEmpty(Username))
            {
                if (ascendingOrder)
                    System.Array.Reverse(entries);
                
                for (int i = 0; i < entries.Length; i++) 
                    entries[i].Rank = i + 1;
                
                var username = Username;
                entries = System.Array.FindAll(entries, entry => entry.Username.ToLower().Contains(username.ToLower()));
            }
            
            if (Skip == 0 && Take == 0)
                return entries;

            var skip = Skip;
            var take = Take;

            if (Take == 0)
                take = entries.Length;
            
            if (skip < 0)
                skip = 0;
            
            if (skip > entries.Length)
                skip = entries.Length;
            
            if (take < 0)
                take = 0;
            
            if (take > entries.Length)
                take = entries.Length;
            
            var length = take - skip;
            if (length <= 0)
                return System.Array.Empty<Entry>();
            
            var newEntries = new Entry[length];
            
            if (ascendingOrder)
                System.Array.Reverse(entries);
            
            System.Array.Copy(entries, skip, newEntries, 0, length);

            for (int i = 0; i < newEntries.Length; i++) 
                newEntries[i].Rank = skip + i + 1;

            return newEntries;
        }
    }
}