using DocuVault.Data;
using System;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Data.OleDb;

namespace DocuVault.Services
{
    public class EmailService
    {
        private readonly string _apiKey;

        public EmailService()
        {
            // Retrieve the SendGrid API key from the environment variables
            _apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");

            // Check if the API key is null or empty
            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new InvalidOperationException("SENDGRID_API_KEY environment variable is not set.");
            }
        }

        public async Task SendEmailAsync(string toEmail, string subject, string plainTextContent, string htmlContent, string verificationPin)
        {
            // Create a SendGrid client using the API key
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress("karlnathanr2@gmail.com", "DocuVault");
            var to = new EmailAddress(toEmail);

            // Append the PIN to the email content
            plainTextContent += $" Your Verification PIN is: {verificationPin}";
            htmlContent += $"Your Verification PIN is: <strong>{verificationPin}</strong>";

            // Create the email message
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            // Send the email asynchronously
            var response = await client.SendEmailAsync(msg);

            // Log email sending status in the database
            var db = new AccessDB();
            try
            {
                string status = response.StatusCode == System.Net.HttpStatusCode.OK ||
                                response.StatusCode == System.Net.HttpStatusCode.Accepted ? "Sent" : "Failed";

                string query = "INSERT INTO EmailLogs (RecipientEmail, Subject, SentDate, Status) VALUES (?, ?, ?, ?)";
                var parameters = new[]
                {
                    new OleDbParameter("?", toEmail),
                    new OleDbParameter("?", subject),
                    new OleDbParameter("?", DateTime.Now),
                    new OleDbParameter("?", status)
                };

                await db.ExecuteNonQueryAsync(query, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging email to database: {ex.Message}");
            }

            // Check and log the response status
            if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                Console.WriteLine("Email sent successfully.");
            }
            else
            {
                Console.WriteLine($"Error sending email: {response.StatusCode}");
            }
        }
    }
}
