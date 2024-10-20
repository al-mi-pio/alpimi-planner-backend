﻿namespace AlpimiAPI.Auth
{
    public class Auth
    {
        public Guid Id { get; set; }
        public required string Password { get; set; }
        public Guid UserID { get; set; }
        public required AlpimiAPI.User.User User { get; set; }
    }
}
