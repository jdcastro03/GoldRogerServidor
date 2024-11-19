namespace GoldRogerServer.DTOs.Organizer
{
    public class CreateTournamentRequestDTO
    {
        public string TournamentName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TournamentTypeId { get; set; }

    }
}
