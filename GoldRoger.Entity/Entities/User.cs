using GoldRoger.Entity.Entities.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GoldRoger.Entity.Entities
{
    namespace GoldRoger.Entity.Entities
    {
        public class User
        {
            public int UserId { get; set; } // Identificador único del usuario
            public string Username { get; set; } // Nombre de usuario
            public string PasswordHash { get; set; } // Contraseña encriptada
            public string Email { get; set; } // Correo electrónico
            public string FirstName { get; set; } // Nombre
            public string LastName { get; set; } // Apellido
            public int UserType { get; set; } // Tipo de usuario (Jugador, Árbitro, Organizador, Entrenador)

            // Navegación
            public virtual UserType UserTypeNavigation { get; set; } // Tipo de usuario (referencia a UserTypes)
            public virtual Coach Coaches { get; set; } // Relación uno a uno con Coaches
            public virtual Player Players { get; set; } // Relación uno a uno con Players
            public virtual Organizer Organizers { get; set; } // Relación uno a uno con Organizers
            public virtual Referee Referees { get; set; } // Relación uno a uno con Referees


            public virtual List<UserPermission>? UserPermission { get; set; }

            [NotMapped]
            public string? JWToken { get; set; }
        }
    }
}

//// Navegación
//public virtual UserType UserTypeNavigation { get; set; } // Tipo de usuario (referencia a UserTypes)
//public virtual Coach Coaches { get; set; } // Relación uno a uno con Coaches
//public virtual Player Players { get; set; } // Relación uno a uno con Players
//public virtual Organizer Organizers { get; set; } // Relación uno a uno con Organizers
//public virtual Referee Referees { get; set; } // Relación uno a uno con Referees
