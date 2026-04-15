


//using Microsoft.Extensions.Configuration;
//using SendGrid;
//using SendGrid.Helpers.Mail;

//public class EmailService
//{
//    private readonly IConfiguration _config;

//    public EmailService(IConfiguration config)
//    {
//        _config = config;
//    }

//    public async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
//    {
//        var apiKey = _config["SendGrid:ApiKey"];
//        var client = new SendGridClient(apiKey);

//        var from = new EmailAddress("jaillymaniya07@gmail.com", "Virtual Classroom");

//        var msg = MailHelper.CreateSingleEmail(
//            from,
//            new EmailAddress(toEmail),
//            subject,
//            "",
//            htmlContent
//        );

//        var response = await client.SendEmailAsync(msg);

//        // 🔥 DEBUG (VERY IMPORTANT)
//        if (!response.IsSuccessStatusCode)
//        {
//            var error = await response.Body.ReadAsStringAsync();
//            Console.WriteLine("SendGrid Error: " + error);
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
