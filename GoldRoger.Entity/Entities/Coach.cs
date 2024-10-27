using GoldRoger.Entity.Entities.GoldRoger.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldRoger.Entity.Entities
{
    public class Coach
    {
        public int CoachId { get; set; } // Identificador del entrenador (clave foránea que hace referencia a Users.UserId)
        public string LicenseNumber { get; set; } // Número de licencia

        // Navegación
        public virtual User User { get; set; } // Relación uno a uno con User
        public virtual Team Team { get; set; } // Relación uno a uno con Team
    }
}