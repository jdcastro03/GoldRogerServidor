using GoldRoger.Entity.Entities.GoldRoger.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldRoger.Entity.Entities
{
    public class Player
    {
        public int PlayerId { get; set; } // Identificador del jugador (debe coincidir con el UserId en Usuarios)
        public int TeamId { get; set; } // Identificador del equipo (debe existir en Equipos)
        public string Position { get; set; } // Posición en el equipo

        // Navegación
        public virtual User User { get; set; } // Relación uno a uno con User
        public virtual Team Team { get; set; } // Relación muchos a uno con Team
    }
}
