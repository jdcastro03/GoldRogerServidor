using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldRoger.Entity.Entities
{
    public class LeagueStanding
    {
        public int TournamentId { get; set; } // Referencia al torneo
        public int TeamId { get; set; } // Referencia al equipo
        public int Points { get; set; } // Puntos acumulados por el equipo en este torneo
        public int MatchesPlayed { get; set; } // Número de partidos jugados
        public int Wins { get; set; } // Número de victorias
        public int Draws { get; set; } // Número de empates
        public int Losses { get; set; } // Número de derrotas
        public int GoalsFor { get; set; } // Goles a favor
        public int GoalsAgainst { get; set; } // Goles en contra
        public int GoalDifference { get; set; } // Diferencia de goles(calculada)
    }
}

