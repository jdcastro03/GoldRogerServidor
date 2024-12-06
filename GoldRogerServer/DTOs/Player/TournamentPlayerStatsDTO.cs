namespace GoldRogerServer.DTOs.Player
{
    public class TournamentPlayerStatsDTO
    {
        public int PlayerId { get; set; }

        public String FirstName { get; set; }

        public string LastName { get; set; }

        public String TeamName { get; set; }
        public int Goals { get; set; }
        public int YellowCards { get; set; }
        public int RedCards { get; set; }
    }
}
