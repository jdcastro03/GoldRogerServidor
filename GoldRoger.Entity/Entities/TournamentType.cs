using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldRoger.Entity.Entities
{
    public class TournamentType
    {
        public int TournamentTypeId { get; set; } // Identificador del tipo de torneo
        public string TournamentTypeName { get; set; } // Nombre del tipo de torneo

        // Navegación
        public virtual ICollection<Tournament> Tournaments { get; set; } // Relación uno a muchos con Tournaments
    }
}
