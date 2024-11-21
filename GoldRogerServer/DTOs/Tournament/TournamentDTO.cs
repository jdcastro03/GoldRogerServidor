namespace GoldRogerServer.DTOs.Tournament
{
    public class TournamentDTO
    {
        public int TournamentId { get; set; }
        public string TournamentName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TournamentTypeId { get; set; }
        public string? OrganizerUsername { get; set; } // El nombre del organizador
    }
}
