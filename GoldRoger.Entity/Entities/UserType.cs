using GoldRoger.Entity.Entities.GoldRoger.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldRoger.Entity.Entities
{
    public class UserType
    {
        public int UserTypeId { get; set; } // Identificador único del tipo de usuario
        public string UserTypeName { get; set; } // Nombre del tipo de usuario

        // Navegación
        public virtual ICollection<User> Users { get; set; } // Relación uno a muchos con Users
    }
}
