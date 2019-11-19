using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GokesInvoicer
{
    public static class Discord
    {
        public static async void AlertDiscord(string message)
        {
            string uri = "/api/webhooks/555989611390697487/JUsYKEHBgXsUZoT73rxFrOC6xq4Z9Ike8-prvAOM9zmyvyqLj2jjZvkr-ecPXnWu9Ru4";

            var client = new RestClient();
            client.BaseUrl = new Uri("https://discordapp.com");

            var request = new RestRequest(Method.POST)
            {
                Resource = uri,
                RequestFormat = DataFormat.Json
            };
            request.AddParameter("content", "GokesInvoicer: " + message);

            client.Execute(request);

        }
    }
}
