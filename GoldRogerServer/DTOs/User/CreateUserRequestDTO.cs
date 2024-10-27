namespace GoldRogerServer.DTOs.User
{
    public class CreateUserRequestDTO
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int UserType { get; set; }

        // Campos adicionales específicos por tipo de usuario
        public string Position { get; set; } // Para jugadores
        public int? TeamId { get; set; }     // Opcional para jugadores
        public string LicenseNumber { get; set; } // Para entrenadores y árbitros
        public string OrganizationName { get; set; } // Para organizadores
    }
}
