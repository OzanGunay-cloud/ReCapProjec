namespace WebAPI.Options;

public sealed class LoginChallengeOptions
{
    public const string SectionName = "LoginChallenge";

    public bool Enabled { get; set; } = true;

    public string DeliveryMode { get; set; } = "LogOnly";

    public int ChallengeTtlMinutes { get; set; } = 5;

    public int MaxAttempts { get; set; } = 5;

    public string Subject { get; set; } = "ReCap giris dogrulama kodunuz";

    public string? FromAddress { get; set; }

    public string? FromDisplayName { get; set; }

    public string? SmtpHost { get; set; }

    public int SmtpPort { get; set; } = 587;

    public string? SmtpUserName { get; set; }

    public string? SmtpPassword { get; set; }

    public bool UseSsl { get; set; } = true;
}
