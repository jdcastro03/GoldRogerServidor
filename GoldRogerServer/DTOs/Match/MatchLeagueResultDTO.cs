namespace GoldRogerServer.DTOs.Match
{
    public class MatchLeagueResultDTO
    {
        public int MatchId { get; set; }
    
            public String Team1Name { get; set; }
    
            public int Team1Goals { get; set; }
            public String Team2Name { get; set; }
    
            public int Team2Goals { get; set; }
    
            public int? Stage { get; set; }
    }
}
