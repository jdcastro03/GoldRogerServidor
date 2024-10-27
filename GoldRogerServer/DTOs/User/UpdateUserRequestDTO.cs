namespace GoldRogerServer.DTOs.User
{
    public class UpdateUserRequestDTO
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; } // Opcional para la actualización
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int UserType { get; set; }
    }
}
