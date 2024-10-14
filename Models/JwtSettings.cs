namespace snap_backend.Models
{
    public class JwtSettings
    {
        public required string Issuer { get; set; }
        public required string Audience { get; set; }
        public required string SecretKey { get; set; }
        public required int AccessTokenExpiration { get; set; }
        public required int RefreshTokenExpiration { get; set; }
    }
}
