namespace WebAPI.Models;

public sealed class VerifyLoginChallengeRequest
{
    public string ChallengeId { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;
}
