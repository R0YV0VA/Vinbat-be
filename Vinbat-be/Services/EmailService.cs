using System.Net;
using System.Net.Mail;

namespace Vinbat_be.Services;

public class EmailService
{
    private readonly string? Adress;
    private readonly string? Password;

    public EmailService(string? adress, string? password)
    {
        Adress = adress;
        Password = password;
    }

    public async Task Send (string recepient, string subject, string body)
    {
        using (var message = new MailMessage()) 
        {
            message.To.Add(new MailAddress(recepient, $"To {recepient}"));
            message.From = new MailAddress(Adress, "Vinbat");
            message.Subject = subject;
            message.Body = body;

            using (var client = new SmtpClient("smtp.gmail.com"))
            {
                client.Port = 587;
                client.Credentials = new NetworkCredential(Adress, Password);
                client.EnableSsl = true;
                await client.SendMailAsync(message);
            }
        }
    }
}
