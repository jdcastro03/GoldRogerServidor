using GoldRoger.Entity.Entities.GoldRoger.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldRoger.Entity.Entities
{
    public class Organizer
    {
        public int OrganizerId { get; set; } // Identificador del organizador (clave foránea que hace referencia a Users.UserId)
        public string OrganizationName { get; set; } // Nombre de la organización

        // Navegación
        public virtual User User { get; set; } // Relación uno a uno con User
        public virtual ICollection<Tournament> Tournaments { get; set; } // Relación uno a muchos con Tournaments
    }
}
