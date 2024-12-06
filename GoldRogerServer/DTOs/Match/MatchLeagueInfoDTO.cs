namespace GoldRogerServer.DTOs.Match
{
    public class MatchLeagueInfoDTO
    {

        public int MatchId { get; set; }

        public String Team1Name { get; set; }

        public String Team2Name { get; set; }

        public int? Stage { get; set; }
    }
}
