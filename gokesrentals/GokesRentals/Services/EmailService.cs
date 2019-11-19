using GokesRentals.Objects;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GokesRentals.Services
{
    public class EmailService
    {
        public static void SendEmail(MaintenanceRequest maintenance)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator = new HttpBasicAuthenticator(
                "api", "REDACTED");
            RestRequest request = new RestRequest();
            request.AddParameter("domain", "mail.tcgsniper.com", ParameterType.UrlSegment);
            request.Resource = "mail.tcgsniper.com/messages";
            request.AddParameter("from", "Gokes Rentals <noreply@mail.tcgsniper.com>");
            request.AddParameter("to", "jonathan.hosein@live.com");
            request.AddParameter("subject", "You have a new maintenance request - " + maintenance.Summary);
            request.AddParameter("text",maintenance.FirstName + " " + maintenance.LastName + "\r\n" + maintenance.Description);
            request.Method = Method.POST;

            try
            {
                IRestResponse r = client.Execute(request);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public static void SendEmail(string subject, string body, string toEmail)
        {

            var client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator = new HttpBasicAuthenticator(
                "api", "REDACTED");
            RestRequest request = new RestRequest();
            request.AddParameter("domain", "mail.tcgsniper.com", ParameterType.UrlSegment);
            request.Resource = "mail.tcgsniper.com/messages";
            request.AddParameter("from", "Gokes Rentals <noreply@mail.tcgsniper.com>");
            request.AddParameter("to", toEmail);
            request.AddParameter("subject", subject);
            request.AddParameter("text", body);
            request.Method = Method.POST;

            try
            {
                IRestResponse r = client.Execute(request);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
