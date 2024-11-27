namespace GoldRogerServer.DTOs.Player
{
    public class PlayerStatsDTO
    {

        public int PlayerId { get; set; }

        public String FirstName { get; set; }

        public string LastName { get; set; }
        public int Goals { get; set; }
        public int YellowCards { get; set; }
        public int RedCards { get; set; }

    }
}
