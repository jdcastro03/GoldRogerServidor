﻿namespace GoldRogerServer.DTOs.Organizer
{
    public class TournamentInfoDTO
    {

        public int TournamentId { get; set; }
        public string TournamentName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TournamentTypeId { get; set; }
    }
}
