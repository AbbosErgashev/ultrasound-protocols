using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using UltrasoundProtocol.Application.Settings;

namespace UltrasoundProtocol.Application.Services.Email;

public interface IEmailService
{
    Task SendAsync(string toEmail, string subject, string htmlBody);
    Task SendResultReadyAsync(string toEmail, string patientName, string bodyPart, DateTime examDate);
}

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendAsync(string toEmail, string subject, string htmlBody)
    {
        using var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
        {
            Credentials = new NetworkCredential(_settings.SenderEmail, _settings.SenderPassword),
            EnableSsl = _settings.EnableSsl
        };

        var message = new MailMessage(_settings.SenderEmail, toEmail, subject, htmlBody)
        {
            IsBodyHtml = true
        };

        await client.SendMailAsync(message);
    }

    public async Task SendResultReadyAsync(string toEmail, string patientName, string bodyPart, DateTime examDate)
    {
        var subject = "MedUZI — Tekshiruv natijangiz tayyor!";
        var body = $"""
            <div style="font-family:Arial,sans-serif;max-width:600px;margin:0 auto;">
                <div style="background:#1b6ec2;color:white;padding:20px;text-align:center;">
                    <h2>🏥 MedUZI Diagnostika Markazi</h2>
                </div>
                <div style="padding:20px;background:#f8f9fa;">
                    <p>Hurmatli <strong>{patientName}</strong>,</p>
                    <p>Sizning <strong>{bodyPart}</strong> bo'yicha UZI tekshiruvi natijasi tayyor.</p>
                    <p>📅 Tekshiruv sanasi: <strong>{examDate:dd.MM.yyyy}</strong></p>
                    <p>Natijangizni ko'rish uchun tizimga kiring:</p>
                    <p style="text-align:center;">
                        <a href="https://localhost/login" 
                           style="background:#1b6ec2;color:white;padding:12px 24px;text-decoration:none;border-radius:6px;display:inline-block;">
                            Natijani ko'rish
                        </a>
                    </p>
                    <hr style="border:1px solid #dee2e6;">
                    <p style="color:#6c757d;font-size:12px;">
                        Bu avtomatik xabar. Savollar uchun: +998 71 200-00-01
                    </p>
                </div>
            </div>
            """;

        await SendAsync(toEmail, subject, body);
    }
}
