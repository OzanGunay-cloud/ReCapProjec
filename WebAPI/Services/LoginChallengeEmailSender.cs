using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using WebAPI.Options;

namespace WebAPI.Services;

public sealed class LoginChallengeEmailSender : ILoginChallengeEmailSender
{
    private readonly LoginChallengeOptions _options;
    private readonly ILogger<LoginChallengeEmailSender> _logger;

    public LoginChallengeEmailSender(
        IOptions<LoginChallengeOptions> options,
        ILogger<LoginChallengeEmailSender> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<LoginChallengeDispatchResult> SendAsync(
        string toEmail,
        string code,
        CancellationToken cancellationToken = default)
    {
        if (!string.Equals(_options.DeliveryMode, "Smtp", StringComparison.OrdinalIgnoreCase) ||
            string.IsNullOrWhiteSpace(_options.SmtpHost) ||
            string.IsNullOrWhiteSpace(_options.FromAddress))
        {
            _logger.LogWarning("Login challenge code for {Email}: {Code}", toEmail, code);
            return new LoginChallengeDispatchResult(
                false,
                code,
                "IP adresi degisti. SMTP hazir olmadigi icin dogrulama kodu log fallback ile olusturuldu.");
        }

        try
        {
            using var client = new SmtpClient(_options.SmtpHost, _options.SmtpPort)
            {
                EnableSsl = _options.UseSsl
            };

            if (!string.IsNullOrWhiteSpace(_options.SmtpUserName))
            {
                client.Credentials = new NetworkCredential(_options.SmtpUserName, _options.SmtpPassword);
            }

            using var message = new MailMessage
            {
                From = new MailAddress(_options.FromAddress, _options.FromDisplayName),
                Subject = _options.Subject,
                Body = $"ReCap giris dogrulama kodunuz: {code}",
                IsBodyHtml = false
            };

            message.To.Add(toEmail);
            await client.SendMailAsync(message, cancellationToken);

            return new LoginChallengeDispatchResult(
                true,
                null,
                "IP adresi degisti. E-posta dogrulama kodu gonderildi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login challenge email could not be sent to {Email}. Falling back to logged code.", toEmail);
            _logger.LogWarning("Login challenge code for {Email}: {Code}", toEmail, code);
            return new LoginChallengeDispatchResult(
                false,
                code,
                "IP adresi degisti ancak e-posta gonderilemedi. Dogrulama kodu development fallback olarak loglandi.");
        }
    }
}
