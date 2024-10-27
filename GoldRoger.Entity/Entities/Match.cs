using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldRoger.Entity.Entities
{
    public class Match
    {
        public int MatchId { get; set; } // Identificador del partido
        public int TournamentId { get; set; } // Identificador del torneo
        public int Team1Id { get; set; } // Identificador del primer equipo
        public int Team2Id { get; set; } // Identificador del segundo equipo
        public DateTime Date { get; set; } // Fecha del partido
        public string Score { get; set; } // Resultado del partido

        // Navegación
        public virtual Tournament Tournament { get; set; } // Relación muchos a uno con Tournament
        public virtual Team Team1 { get; set; } // Relación muchos a uno con Team
        public virtual Team Team2 { get; set; } // Relación muchos a uno con Team
        public virtual ICollection<MatchReferee> MatchReferees { get; set; } // Relación uno a muchos con MatchReferees
    }
}
