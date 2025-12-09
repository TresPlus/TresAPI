using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace API.Extensions
{
  public class EmailSender : IEmailSender
  {
    private readonly IConfiguration _config;

    public EmailSender(IConfiguration config)
    {
      _config = config;
    }

    private string SanitizeHeader(string input)
    {
      if (string.IsNullOrWhiteSpace(input))
        return input;

      // Mail header’larında yasak olan karakterler
      var invalidChars = new[] { "\"", "\r", "\n" };

      foreach (var c in invalidChars)
        input = input.Replace(c, "");

      return input.Trim();
    }


    public async Task SendEmailAsync(string to, string subject, string htmlBody)
    {
      to = SanitizeHeader(to);
      subject = SanitizeHeader(subject);
      var from = SanitizeHeader(_config["SMTP:From"]);

      var smtp = new SmtpClient
      {
        Host = _config["SMTP:Host"],
        Port = int.Parse(_config["SMTP:Port"]),
        EnableSsl = bool.Parse(_config["SMTP:SSL"]),
        Credentials = new NetworkCredential(
              _config["SMTP:Username"],
              _config["SMTP:Password"]
          )
      };

      var msg = new MailMessage
      {
        From = new MailAddress(from),
        Subject = subject,
        Body = htmlBody,
        IsBodyHtml = true
      };

      msg.To.Add(to);

      await smtp.SendMailAsync(msg);
    }

  }
}
