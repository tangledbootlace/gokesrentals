using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace GokesInvoicer
{
    public static class Function1
    {
        /// <summary>
        /// Runs every day at 1:00 AM CT and creates an invoice for each property if one does not already exist.
        /// </summary>
        /// <param name="myTimer"></param>
        /// <param name="log"></param>
        [FunctionName("Function1")]
        public static void Run([TimerTrigger("0 0 6 * * *", RunOnStartup = false)]TimerInfo myTimer, TraceWriter log)
        {
            InvoiceService.Run();
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
