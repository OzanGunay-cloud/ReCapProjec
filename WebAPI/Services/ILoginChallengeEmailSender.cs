namespace WebAPI.Services;

public interface ILoginChallengeEmailSender
{
    Task<LoginChallengeDispatchResult> SendAsync(
        string toEmail,
        string code,
        CancellationToken cancellationToken = default);
}
