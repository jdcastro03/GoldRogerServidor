namespace GoldRogerServer.DTOs.Organizer
{
    public class LeagueStandingDTO
    {

        public String TeamName { get; set; }

        public int Points { get; set; }

        public int MatchesPlayed { get; set; }

        public int Wins { get; set; }

        public int Draws { get; set; }

        public int Losses { get; set; }

        public int GoalsFor { get; set; }

        public int GoalsAgainst { get; set; }
    }
}
