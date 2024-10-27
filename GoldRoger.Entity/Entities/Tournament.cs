using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldRoger.Entity.Entities
{
    public class Tournament
    {
        public int TournamentId { get; set; } // Identificador del torneo
        public int OrganizerId { get; set; } // Identificador del organizador
        public string TournamentName { get; set; } // Nombre del torneo
        public DateTime StartDate { get; set; } // Fecha de inicio
        public DateTime EndDate { get; set; } // Fecha de finalización
        public int TournamentTypeId { get; set; } // Identificador del tipo de torneo

        // Navegación
        public virtual Organizer Organizer { get; set; } // Relación muchos a uno con Organizer
        public virtual TournamentType TournamentType { get; set; } // Relación muchos a uno con TournamentType
        public virtual ICollection<Match> Matches { get; set; } // Relación uno a muchos con Matches
    }
}
