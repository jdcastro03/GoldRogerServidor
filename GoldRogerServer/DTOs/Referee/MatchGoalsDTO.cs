namespace GoldRogerServer.DTOs.Referee
{
    public class MatchGoalsDTO
    {
        public int Team1Goals { get; set; }

        public int Team2Goals { get; set; }

        public bool? IsFinished { get; set; }
    }
}
