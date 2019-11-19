using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using GokesInvoicer.Objects;

namespace GokesInvoicer
{
    public static class SQL
    {
        private static SqlConnection GetConnection()
        {
            string connectionString = "REDACTED";
            SqlConnection connection = new SqlConnection(connectionString);
            return connection;
        }

        public static List<Tenant> GetTenantsWithProperty()
        {
            string selectStatement = "SELECT TenantId, PropertyID FROM Tenant WHERE PropertyID IS NOT NULL";
            SqlConnection connection = GetConnection();
            SqlCommand selectCommand =  new SqlCommand(selectStatement, connection);

            try
            {
                connection.Open();
                var tenants = new List<Tenant>();
                SqlDataReader reader = selectCommand.ExecuteReader();
               
                while (reader.Read())
                {
                    var tenant = new Tenant
                    {
                        TenantID = (int)reader["TenantId"],
                        PropertyID = (int)reader["PropertyID"],
                    };
                    tenants.Add(tenant);
                }
                return tenants;
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

        public static List<Invoice> GetInvoicesBetweenDates(DateTime firstDate, DateTime secondDate)
        {

            string selectStatement = "SELECT InvoiceID, PropertyID, BillDate FROM Invoice WHERE BillDate >= @firstDate AND BillDate < @secondDate";
            SqlConnection connection = GetConnection();
            SqlCommand selectCommand = new SqlCommand(selectStatement, connection);
            selectCommand.Parameters.AddWithValue("@firstDate", firstDate);
            selectCommand.Parameters.AddWithValue("@secondDate", secondDate);

            try
            {
                connection.Open();
                var invoices = new List<Invoice>();
                SqlDataReader reader = selectCommand.ExecuteReader();

                while (reader.Read())
                {
                    var invoice = new Invoice
                    {
                        InvoiceID = (int)reader["invoiceID"],
                        PropertyID = (int)reader["PropertyID"],
                        BillDate = (DateTime)reader["BillDate"],
                    };
                    invoices.Add(invoice);
                }

                if(invoices.Count > 0)
                {
                    return invoices;
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

        public static Property GetProperty(int propertyID)
        {
            string selectStatement = "SELECT * FROM Property WHERE PropertyID = @PropertyID";
            SqlConnection connection = GetConnection();
            SqlCommand selectCommand = new SqlCommand(selectStatement, connection);
            selectCommand.Parameters.AddWithValue("@PropertyID", propertyID);

            try
            {
                connection.Open();
                SqlDataReader reader = selectCommand.ExecuteReader();

                if (reader.Read())
                {
                    var property = new Property
                    {
                        PropertyID = (int)reader["PropertyID"],
                        Address1 = reader["Address 1"].ToString(),
                        Address2 = reader["Address 2"].ToString(),
                        City = reader["City"].ToString(),
                        State = reader["State"].ToString(),
                        ZipCode = (int)reader["ZipCode"],
                        PropertyValue = (decimal)reader["PropertyValue"],
                        RentCharge = (decimal)reader["RentCharge"]
                    };
                    return property;
                }
                return null;
                
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

        public static bool CreateInvoice(Invoice inv)
        {
            string insertStatement = "INSERT INTO Invoice (InvoiceNumber, AmountDue, BillDate, DueDate, PropertyID, PaidInFull) Values(@InvoiceNumber, @AmountDue, @BillDate, @DueDate, @PropertyID, @PaidInFull)";
            SqlConnection connection = GetConnection();
            SqlCommand insertCommand = new SqlCommand(insertStatement, connection);

            insertCommand.Parameters.AddWithValue("@InvoiceNumber", inv.InvoiceNumber);
            insertCommand.Parameters.AddWithValue("@AmountDue", inv.AmountDue);
            insertCommand.Parameters.AddWithValue("@BillDate", inv.BillDate);
            insertCommand.Parameters.AddWithValue("@DueDate", inv.DueDate);
            insertCommand.Parameters.AddWithValue("@PropertyID", inv.PropertyID);
            insertCommand.Parameters.AddWithValue("@PaidInFull", inv.PaidInFull);

            try
            {


                connection.Open();
                int count = insertCommand.ExecuteNonQuery();
                if (count > 0)
                    return true;
                else
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
