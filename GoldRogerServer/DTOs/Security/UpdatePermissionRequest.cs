namespace GoldRogerServer.DTOs.Security
{
    public class UpdatePermissionRequest
    {
        public string Key { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public int Id { get; set; }
    }
}