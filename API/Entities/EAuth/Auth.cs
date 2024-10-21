using AlpimiAPI.Entities.EUser;

namespace AlpimiAPI.Entities.EAuth
{
    public class Auth
    {
        public Guid Id { get; set; }
        public required string Password { get; set; }
        public Guid UserID { get; set; }
        public required string Salt { get; set; }
        public required User User { get; set; }
    }
}
