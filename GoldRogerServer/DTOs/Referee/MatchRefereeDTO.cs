namespace GoldRogerServer.DTOs.Referee
{
    public class MatchRefereeDTO
    {

        public int MatchId { get; set; }

        public String Team1Name { get; set; }

        public string Team2Name { get; set; }

        public DateTime? Date { get; set; }
    }
}
