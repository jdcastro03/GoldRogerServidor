using GoldRoger.Entity.Entities.GoldRoger.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldRoger.Entity.Entities
{
    public class MatchReferee
    {
        public int MatchId { get; set; } // Identificador del partido
        public int RefereeId { get; set; } // Identificador del árbitro

        // Navegación
        public virtual Match Match { get; set; } // Relación muchos a uno con Match
        public virtual Referee Referee { get; set; } // Relación muchos a uno con Referee
    }
}
