using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GoldRoger.Entity.Entities
{
    public class Team
    {
        public int TeamId { get; set; } // Identificador del equipo
        public string TeamName { get; set; } // Nombre del equipo
        public int CoachId { get; set; } // Identificador del entrenador

        //add tournamenti can be null
        public int? TournamentId { get; set; } // Identificador del torneo

        // Navegación
        public virtual Coach Coach { get; set; } // Relación uno a uno con Coach
        public virtual ICollection<Player> Players { get; set; } // Relación uno a muchos con Players
    }
}
