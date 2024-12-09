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

        public int Team1Goals { get; set; } // Goles del primer equipo
        public int Team2Goals { get; set; } // Goles del segundo equipo

        public DateTime? Date { get; set; } // Fecha del partido

        public bool? IsFinished { get; set; }

        public int? Stage { get; set; } // Etapa del torneo

        public bool Active { get; set; } // Indica si el partido está activo

        public bool? Evaluated { get; set; } // Indica si el partido ya fue evaluado


        // Navegación
        public virtual Tournament Tournament { get; set; } // Relación muchos a uno con Tournament
        public virtual Team Team1 { get; set; } // Relación muchos a uno con Team
        public virtual Team Team2 { get; set; } // Relación muchos a uno con Team
        public virtual ICollection<MatchReferee> MatchReferees { get; set; } // Relación uno a muchos con MatchReferees
    }
}
