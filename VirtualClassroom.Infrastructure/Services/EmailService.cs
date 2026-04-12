
//using MailKit.Net.Smtp;
//using Microsoft.Extensions.Configuration;
//using MimeKit;

//public class EmailService
//{
//    private readonly IConfiguration _config;

//    public EmailService(IConfiguration config)
//    {
//        _config = config;
//    }

//    public async Task SendEmailAsync(string toEmail, string subject, string body)
//    {
//        var message = new MimeMessage();

//        message.From.Add(new MailboxAddress("Virtual Classroom",
//            _config["EmailSettings:Email"]));

//        message.To.Add(MailboxAddress.Parse(toEmail));
//        message.Subject = subject;

//        message.Body = new TextPart("plain")
//        {
//            Text = body
//        };

//        using (var client = new SmtpClient())
//        {
//            await client.ConnectAsync(
//                _config["EmailSettings:Host"],
//                int.Parse(_config["EmailSettings:Port"]),
//                MailKit.Security.SecureSocketOptions.StartTls);

//            await client.AuthenticateAsync(
//                _config["EmailSettings:Email"],
//                _config["EmailSettings:Password"]);

//            await client.SendAsync(message);
//            await client.DisconnectAsync(true);
//        }
//    }
//}



using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
    {
        var apiKey = _config["SendGrid:ApiKey"];
        var client = new SendGridClient(apiKey);

        var from = new EmailAddress("jaillymaniya07@gmail.com", "Virtual Classroom");

        var msg = MailHelper.CreateSingleEmail(
            from,
            new EmailAddress(toEmail),
            subject,
            "",
            htmlContent
        );

        var response = await client.SendEmailAsync(msg);

        // 🔥 DEBUG (VERY IMPORTANT)
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Body.ReadAsStringAsync();
            Console.WriteLine("SendGrid Error: " + error);
        }
    }
}