namespace GoldRogerServer.DTOs.Coach
{
    public class CreateTeamRequestDTO
    {
        public string TeamName { get; set; } // Nombre del equipo
        public int? TournamentId { get; set; } // Identificador del torneo
    }
}
