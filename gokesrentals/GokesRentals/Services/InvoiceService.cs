using GokesRentals.Objects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace GokesRentals.Services
{
    public static class InvoiceService
    {
        public static Invoice GetInvoiceByInvoiceNumber(Guid invoiceNumber)
        {
            SqlConnection connection = ConnectionString.GetConnection();

            string selectStatement
                = "SELECT *  "
                + "FROM [Goke's Rentals].[dbo].[Invoice] "
                + "WHERE InvoiceNumber = @InvoiceNumber";


            SqlCommand selectCommand = new SqlCommand(selectStatement, connection);
            selectCommand.Parameters.AddWithValue("@InvoiceNumber", invoiceNumber);

            try
            {
                connection.Open();
                SqlDataReader reader = selectCommand.ExecuteReader(CommandBehavior.SingleRow);
                var invoice = new Invoice();

                if (reader.Read())
                {
                    invoice.InvoiceID = (int)reader["InvoiceID"];
                    invoice.InvoiceNumber = (Guid)reader["InvoiceNumber"];
                    invoice.AmountDue = (decimal)reader["AmountDue"];
                    invoice.BillDate = (DateTime)reader["BillDate"];
                    invoice.DueDate = (DateTime)reader["DueDate"];
                    invoice.PropertyID = (int)reader["PropertyID"];
                    invoice.PaidInFull = (bool)reader["PaidInFull"];

                    return invoice;
                }
                else
                {
                    return null;
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        public static Invoice GetInvoiceByID(int invoiceID)
        {
            SqlConnection connection = ConnectionString.GetConnection();

            string selectStatement
                = "SELECT *  "
                + "FROM [Goke's Rentals].[dbo].[Invoice] "
                + "WHERE InvoiceID = @InvoiceID";


            SqlCommand selectCommand = new SqlCommand(selectStatement, connection);
            selectCommand.Parameters.AddWithValue("@InvoiceID", invoiceID);

            try
            {
                connection.Open();
                SqlDataReader reader = selectCommand.ExecuteReader(CommandBehavior.SingleRow);
                var invoice = new Invoice();

                if (reader.Read())
                {
                    invoice.InvoiceID = (int)reader["InvoiceID"];
                    invoice.InvoiceNumber = (Guid)reader["InvoiceNumber"];
                    invoice.AmountDue = (decimal)reader["AmountDue"];
                    invoice.BillDate = (DateTime)reader["BillDate"];
                    invoice.DueDate = (DateTime)reader["DueDate"];
                    invoice.PropertyID = (int)reader["PropertyID"];
                    invoice.PaidInFull = (bool)reader["PaidInFull"];

                    return invoice;
                }
                else
                {
                    return null;
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        public static List<Payment> GetAllPayemntsForInvoice(int invoiceID)
        {
            SqlConnection connection = ConnectionString.GetConnection();

            string selectStatement
                = "SELECT *  "
                + "FROM [Goke's Rentals].[dbo].[Payment] "
                + "WHERE InvoiceID = @InvoiceID";


            SqlCommand selectCommand = new SqlCommand(selectStatement, connection);
            selectCommand.Parameters.AddWithValue("@InvoiceID", invoiceID);

            try
            {
                connection.Open();
                SqlDataReader reader = selectCommand.ExecuteReader();
                var payments = new List<Payment>();

                while (reader.Read())
                {
                    var payment = new Payment();
                    payment.PaymentID = (int)reader["PaymentID"];
                    payment.InvoiceID = (int)reader["InvoiceID"];
                    payment.PaymentAmount = (decimal)reader["PaymentAmount"];
                    payment.Date = (DateTime)reader["Date"];
                    payment.TenantId = (int)reader["TenantId"];
                    payment.Receipt = reader["Receipt"].ToString();

                    payments.Add(payment);
                }
                return payments;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        public static void PayInvoice(int invoiceID,int tenantID, decimal amount, decimal fee)
        {
            try
            {
                var tenant = Methods.Methods.GetTenant(tenantID);
                var invoice = GetInvoiceByID(invoiceID);
                string description = "Goke's Rental Invoice " + invoiceID.ToString();
                string receipt = StripeService.ChargeTenant(tenant, amount, description, fee);

                if (receipt == null)
                    throw new Exception("An attempted charged was declined.");

                bool success = LogPayment(invoiceID, tenantID, amount, receipt);

                if (!success)
                    throw new Exception("Transaction successful, but could not log payment to database.");

                CalculatePaidInFull(invoice);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public static bool LogPayment(int invoiceID, int tenantID, decimal amount, string receipt)
        {
            SqlConnection connection = ConnectionString.GetConnection();

            string insertStatement = "INSERT INTO Payment (InvoiceID, PaymentAmount, Receipt, Date, TenantId) "
                + "Values (@InvoiceID, @PaymentAmount, @Receipt, @Date, @TenantId)";

            SqlCommand insertCommand = new SqlCommand(insertStatement, connection);
            insertCommand.Parameters.AddWithValue("@InvoiceID", invoiceID);
            insertCommand.Parameters.AddWithValue("@PaymentAmount", amount);
            insertCommand.Parameters.AddWithValue("@Receipt", receipt);
            insertCommand.Parameters.AddWithValue("@Date", DateTime.UtcNow);
            insertCommand.Parameters.AddWithValue("TenantId", tenantID);

            try
            {
                connection.Open();
                int rows = insertCommand.ExecuteNonQuery();

                if (rows > 0)
                    return true;

                return false;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        public static void CalculatePaidInFull(Invoice invoice)
        {
            List<Payment> allPayments = new List<Payment>();
            decimal totalPayments = 0;
            allPayments = GetAllPayemntsForInvoice(invoice.InvoiceID);

            if (allPayments.Count == 0)
                return;

            foreach (var payment in allPayments)
            {
                totalPayments += payment.PaymentAmount;
            }

            if (totalPayments >= invoice.AmountDue)
                MarkPaidInFull(invoice.InvoiceID);
        }

        public static bool MarkPaidInFull(int invoiceID)
        {
            SqlConnection connection = ConnectionString.GetConnection();

            string selectStatement
                = "UPDATE [Goke's Rentals].[dbo].[Invoice] "
                + "SET PaidInFull = @Condition "
                + "WHERE InvoiceID = @InvoiceID";
            SqlCommand selectCommand = new SqlCommand(selectStatement, connection);
            selectCommand.Parameters.AddWithValue("@InvoiceID", invoiceID);
            selectCommand.Parameters.AddWithValue("@Condition", true);

            try
            {
                connection.Open();
                var rows = selectCommand.ExecuteNonQuery();

                if (rows > 0)
                    return true;

                return false;
               
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

    }
}
