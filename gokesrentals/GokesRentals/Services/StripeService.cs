using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GokesRentals.Objects;
using Stripe;

namespace GokesRentals.Services
{
    public class StripeService
    {
        public static bool CreateCustomer(Tenant tenant, string stripeToken, bool isVerified)
        {

            var options = new CustomerCreateOptions
            {
                Email = tenant.EmailAddress,
                SourceToken = stripeToken,
            };

            var service = new CustomerService();
            Customer customer = service.Create(options); //TODO: Store customerID. Token is not needed.

            var success = SaveStripeCustomerDB(tenant.TenantID, customer.Id, isVerified);
            return success;
        }

        public static bool UpdateCustomer(int tenantId, string stripeID, string stripeToken, bool isVerified)
        {
            var options = new CustomerUpdateOptions
            {
                SourceToken = stripeToken, 
            };

            var service = new CustomerService();
            Customer customer = service.Update(stripeID, options);

            SQL.SQLStatements.UpdateStripeStatus(tenantId, isVerified);

            return true;
        }

        public static Customer GetCustomer(string stripeCustomer)
        {
            var service = new CustomerService();
            return service.Get(stripeCustomer);
        }

        public static BankAccount GetBankAccount(Tenant tenant)
        {
            var customer = GetCustomer(tenant.StripeID);
            var service = new BankAccountService();

            BankAccount bankAccount = service.Get(tenant.StripeID, customer.DefaultSourceId);
            bankAccount.Id = "";
            bankAccount.CustomerId = "";
            return bankAccount;
        }

        public static bool VerifyBankAccount(Tenant tenant, List<string> inboundDeposits)
        {
            var customer = GetCustomer(tenant.StripeID);
            List<long> deposits = ValidateDeposits(inboundDeposits);



            //This may have to be provided by customer.
            var options = new BankAccountVerifyOptions
            {
                Amounts = new List<long>()
            };

            foreach (var deposit in deposits)
            {
                options.Amounts.Add(deposit);
            }

            var service = new BankAccountService();

            try
            {
                BankAccount bankAccount = service.Verify(
                    tenant.StripeID,
                    customer.DefaultSourceId,
                    options
                    );

                if (bankAccount.Status.ToLower() == "verified")
                {
                   return SQL.SQLStatements.UpdateStripeStatus(tenant.TenantID, true);
                }
                else
                {
                    return false;
                }
            }
            catch (StripeException ex)
            {
                if (ex.Message == "This bank account has already been verified.")
                    return SQL.SQLStatements.UpdateStripeStatus(tenant.TenantID, true);

                    throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

        }

        public static string ChargeTenant(Tenant tenant, decimal amount, string description, decimal fee = 0)
        {
            //Combine amount w/ fee
            amount += fee;

            var options = new ChargeCreateOptions
            {
                Amount = ConvertToCents(amount),
                Currency = "usd",
                Description = description,
                StatementDescriptor = description,
                CustomerId = tenant.StripeID,
                ReceiptEmail = tenant.EmailAddress,
            };
            try
            {
                var service = new ChargeService();
                Charge charge = service.Create(options);
                var status = charge.Status;
                

                if (status.ToLower() == "succeeded" || status.ToLower() =="pending")
                    return charge.ReceiptUrl;

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static decimal CalculateTransactionFee(decimal amount, bool isACH = true)
        {
            if (isACH == false)
                throw new NotImplementedException();

            const decimal feePercentage = 0.008m;
            const decimal feeFixed = 0;
            const decimal feeCap = 5.00m;
            decimal newCharge = 0.00m;
            decimal difference = 0;

            var numerator = amount + feeFixed;
            var denominator = 1 - feePercentage;
            newCharge = numerator / denominator;

            difference = newCharge - amount;

            if (difference > feeCap)
                return feeCap;


            return difference;
        }


        private static bool SaveStripeCustomerDB(int tenantId, string stripeID, bool isVerified)
        {
           var success =  SQL.SQLStatements.UpdateBillingInformation(tenantId, stripeID, isVerified);

            return success;
        }

        private static List<long> ValidateDeposits(List<string>deposits)
        {
            List<decimal> decimalDeposits = new List<decimal>();
            List<long> depositCents = new List<long>();

            foreach (var deposit in deposits)
            {
                if (decimal.TryParse(deposit, out decimal result))
                {
                    decimalDeposits.Add(result); 
                }
                else
                {
                    throw new Exception("Invalid deposits.");
                }
            }

            foreach (var deposit in decimalDeposits)
            {
                depositCents.Add(ConvertToCents(deposit));
            }
            return depositCents;
        }

        private static long ConvertToCents(decimal amount)
        {
            var cents = amount * 100;

            return (long)cents;
        }
    }
}
