namespace Application.Common.Settings;

public record JwtSettings(string Issuer, string Audience, string SecretKey, string SecurityAlgorithms, int ExpiryMinutes);
