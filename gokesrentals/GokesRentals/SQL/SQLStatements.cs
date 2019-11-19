using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using GokesRentals.Services;
using GokesRentals.Objects;

namespace GokesRentals.SQL
{
    public class SQLStatements
    {
        private static Tenant AddEmptyString(Tenant tenant)
        {
            if (String.IsNullOrEmpty(tenant.FirstName))
                tenant.FirstName = "";

            if (String.IsNullOrEmpty(tenant.LastName))
                tenant.LastName = "";

            if (String.IsNullOrEmpty(tenant.PhoneNumber))
                tenant.PhoneNumber = "";
            if (String.IsNullOrEmpty(tenant.ContractLink))
                tenant.ContractLink = "";
            return tenant;
        }
        public static bool AddMaintenanceRequest(MaintenanceRequest MaintenanceRequest)
        {
            SqlConnection connection = ConnectionString.GetConnection();
            string insertstatement =
                "Insert into [Goke's Rentals].[dbo].[MaintenanceTicket] " + "(TenantID, PropertyID, FirstName, LastName, Summary, Description, OpenDate, TicketActive) " +
                "VALUES (@TenantID, @PropertyID, @FirstName, @LastName, @Summary, @Description, @OpenDate, @TicketActive)";
            SqlCommand insertcommand =
                new SqlCommand(insertstatement, connection);
            insertcommand.Parameters.AddWithValue("@TenantID", MaintenanceRequest.TenantID);
            insertcommand.Parameters.AddWithValue("@PropertyID", MaintenanceRequest.PropertyID);
            insertcommand.Parameters.AddWithValue("@FirstName", MaintenanceRequest.FirstName);
            insertcommand.Parameters.AddWithValue("@LastName", MaintenanceRequest.LastName);
            insertcommand.Parameters.AddWithValue("@Summary", MaintenanceRequest.Summary);
            insertcommand.Parameters.AddWithValue("@Description", MaintenanceRequest.Description);
            insertcommand.Parameters.AddWithValue("@OpenDate", MaintenanceRequest.OpenDate);
            insertcommand.Parameters.AddWithValue("@TicketActive", true);
            try
            {
                connection.Open();
                int count = insertcommand.ExecuteNonQuery();
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

        public static bool AddTenant(Tenant tenant)
        {
            SqlConnection connection = ConnectionString.GetConnection();
            string insertstatement =
                "Insert into [Goke's Rentals].[dbo].[Tenant] " + "(PropertyID, FirstName, LastName, EmailAddress, PhoneNumber, Password, Role, ContractLink) " +
                "VALUES (@PropertyID, @FirstName, @LastName, @EmailAddress, @PhoneNumber, @Password, @Role, @ContractLink)";
            SqlCommand insertcommand = new SqlCommand(insertstatement, connection);

            //Sometimes not all information is supplied at signup. Handle accordingly
            tenant = AddEmptyString(tenant);

            insertcommand.Parameters.AddWithValue("@PropertyID", tenant.PropertyID);
            insertcommand.Parameters.AddWithValue("@FirstName", tenant.FirstName);
            insertcommand.Parameters.AddWithValue("@LastName", tenant.LastName);
            insertcommand.Parameters.AddWithValue("@EmailAddress", tenant.EmailAddress);
            insertcommand.Parameters.AddWithValue("@PhoneNumber", tenant.PhoneNumber);
            insertcommand.Parameters.AddWithValue("@Password", tenant.PasswordHash);
            insertcommand.Parameters.AddWithValue("@ContractLink", tenant.ContractLink);
            insertcommand.Parameters.AddWithValue("@Role", "member");
            try
            {
                connection.Open();
                int count = insertcommand.ExecuteNonQuery();
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

        public static bool AddProperty(Property property)
        {
            SqlConnection connection = ConnectionString.GetConnection();
            string insertstatement =
                "Insert into [Goke's Rentals].[dbo].[Property] " + "([Address 1], [Address 2], City, State, ZipCode, PropertyValue, RentCharge) " +
                "VALUES (@Address1, @Address2, @City, @State, @ZipCode, @PropertyValue, @RentCharge)";
            SqlCommand insertcommand =
                new SqlCommand(insertstatement, connection);
            insertcommand.Parameters.AddWithValue("@Address1", property.Address1);
            insertcommand.Parameters.AddWithValue("@Address2", property.Address2);
            insertcommand.Parameters.AddWithValue("@City", property.City);
            insertcommand.Parameters.AddWithValue("@State", property.State);
            insertcommand.Parameters.AddWithValue("@ZipCode", property.Zipcode);
            insertcommand.Parameters.AddWithValue("@PropertyValue", property.PropertyValue);
            insertcommand.Parameters.AddWithValue("@RentCharge", property.RentCharge);
            try
            {
                connection.Open();
                int count = insertcommand.ExecuteNonQuery();
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

        public static bool UpdateBillingInformation(int tenantId, string stripeID, bool isVerified)
        {
            SqlConnection connection = ConnectionString.GetConnection();
            string updatestatement =
                "UPDATE [Goke's Rentals].[dbo].[Tenant] " +
                "SET StripeID = @StripeID, StripeIsVerified = @Verified " +
                "WHERE TenantId = @TenantId";
            SqlCommand updatecommand =
                new SqlCommand(updatestatement, connection);

            updatecommand.Parameters.AddWithValue("@StripeID", stripeID);
            updatecommand.Parameters.AddWithValue("@TenantId", tenantId);
            updatecommand.Parameters.AddWithValue("@Verified", isVerified);
            try
            {
                connection.Open();
                int count = updatecommand.ExecuteNonQuery();
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

        public static bool UpdatePersonalInformation(PersonalInformation PersonalInformation)
        {
            SqlConnection connection = ConnectionString.GetConnection();
            string updatestatement =
                "UPDATE [Goke's Rentals].[dbo].[Tenant] " +
                "SET FirstName = @FirstName, LastName = @LastName, PhoneNumber = @Phone, EmailAddress = @Email " +
                "WHERE TenantID = @TenantID";
            SqlCommand updatecommand =
                new SqlCommand(updatestatement, connection);
            updatecommand.Parameters.AddWithValue("@TenantID", PersonalInformation.TenantID);
            updatecommand.Parameters.AddWithValue("@FirstName", PersonalInformation.FirstName);
            updatecommand.Parameters.AddWithValue("@LastName", PersonalInformation.LastName);
            updatecommand.Parameters.AddWithValue("@Phone", PersonalInformation.Phone);
            updatecommand.Parameters.AddWithValue("@Email", PersonalInformation.Email);
            try
            {
                connection.Open();
                int count = updatecommand.ExecuteNonQuery();
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

        public static bool UpdateEmploymentDetails(EmploymentDetails EmploymentDetails)
        {
            SqlConnection connection = ConnectionString.GetConnection();
            string updatestatement =
                "UPDATE [Goke's Rentals].[dbo].[Tenant] " +
                "SET Employer = @Employer, JobTitle = @JobTitle " +
                "WHERE TenantID = @TenantID";
            SqlCommand updatecommand =
                new SqlCommand(updatestatement, connection);
            updatecommand.Parameters.AddWithValue("@TenantID", EmploymentDetails.TenantID);
            updatecommand.Parameters.AddWithValue("@Employer", EmploymentDetails.Employer);
            updatecommand.Parameters.AddWithValue("@JobTitle", EmploymentDetails.JobTitle);
            try
            {
                connection.Open();
                int count = updatecommand.ExecuteNonQuery();
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

        public static bool UpdateStripeStatus(int tenantId, bool isVerified)
        {
            SqlConnection connection = ConnectionString.GetConnection();
            string updatestatement =
                "UPDATE [Goke's Rentals].[dbo].[Tenant] " +
                "SET StripeIsVerified = @Verified " +
                "WHERE TenantID = @TenantID";
            SqlCommand updatecommand =
                new SqlCommand(updatestatement, connection);
            updatecommand.Parameters.AddWithValue("@TenantID", tenantId);
            updatecommand.Parameters.AddWithValue("@Verified", isVerified);

            try
            {
                connection.Open();
                int count = updatecommand.ExecuteNonQuery();
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
        //Changes the request status to closed and adds a close date to the ticket
        public static bool CloseMaitenanceRequest(int ticketID, decimal cost)
        {
            SqlConnection connection = ConnectionString.GetConnection();
            string updatestatement =
                "UPDATE [Goke's Rentals].[dbo].[MaintenanceTicket] " +
                "SET TicketActive = @TicketActive, CloseDate = @CloseDate, Cost = @Cost " +
                "WHERE TicketID = @TicketID";
            SqlCommand updatecommand =
                new SqlCommand(updatestatement, connection);
            updatecommand.Parameters.AddWithValue("@TicketID", ticketID);
            updatecommand.Parameters.AddWithValue("@Cost", cost);
            updatecommand.Parameters.AddWithValue("@TicketActive", false);
            updatecommand.Parameters.AddWithValue("@CloseDate", DateTime.UtcNow.ToString());
            try
            {
                connection.Open();
                int count = updatecommand.ExecuteNonQuery();
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

        public static Property GetWidgetInfo(int PropertyID)
        {
            SqlConnection connection = ConnectionString.GetConnection();
            string selectstatement =
                "SELECT [Address 1], [Address 2], City, State, ZipCode, RentCharge FROM [Goke's Rentals].[dbo].[Property] WHERE PropertyID = @PropertyID";
            SqlCommand selectcommand =
                new SqlCommand(selectstatement, connection);
            selectcommand.Parameters.AddWithValue("@PropertyID", PropertyID);
            try
            {
                connection.Open();
                SqlDataReader WidgetInfoReader = selectcommand.ExecuteReader(CommandBehavior.SingleRow);
                if (WidgetInfoReader.Read())
                {
                    Property CurrentWidgetInfo = new Property();
                    CurrentWidgetInfo.PropertyID = PropertyID;
                    CurrentWidgetInfo.Address1 = WidgetInfoReader["Address 1"].ToString();
                    CurrentWidgetInfo.Address2 = WidgetInfoReader["Address 2"].ToString();
                    CurrentWidgetInfo.City = WidgetInfoReader["City"].ToString();
                    CurrentWidgetInfo.State = WidgetInfoReader["State"].ToString();
                    CurrentWidgetInfo.Zipcode = (int)WidgetInfoReader["ZipCode"];
                    CurrentWidgetInfo.RentCharge = (decimal)WidgetInfoReader["RentCharge"];
                    return CurrentWidgetInfo;
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

        public static Tenant GetPersonalInformation(int tenantID)
        {
            SqlConnection connection = ConnectionString.GetConnection();

            string selectStatement
                = "SELECT FirstName, LastName, EmailAddress, PhoneNumber, ContractLink "
                + "FROM Tenant "
                + "WHERE TenantID = @TenantID";
            SqlCommand selectCommand =
                new SqlCommand(selectStatement, connection);
            selectCommand.Parameters.AddWithValue("@TenantID", tenantID);

            try
            {
                connection.Open();
                SqlDataReader tenantReader =
                    selectCommand.ExecuteReader(CommandBehavior.SingleRow);
                if (tenantReader.Read())
                {
                    Tenant info = new Tenant();
                    info.FirstName = tenantReader["FirstName"].ToString();
                    info.LastName = tenantReader["LastName"].ToString();
                    info.EmailAddress = tenantReader["EmailAddress"].ToString();
                    info.PhoneNumber = tenantReader["PhoneNumber"].ToString();
                    info.ContractLink = tenantReader["ContractLink"].ToString();
                    return info;
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


        public static EmploymentDetails GetEmploymentDetails(int tenantID)
        {
            SqlConnection connection = ConnectionString.GetConnection();

            string selectStatement
                = "SELECT Employer, JobTitle  "
                + "FROM [Goke's Rentals].[dbo].[Tenant] "
                + "WHERE TenantID = @TenantID";


            SqlCommand selectCommand =
                new SqlCommand(selectStatement, connection);
            selectCommand.Parameters.AddWithValue("@TenantID", tenantID);

            try
            {
                connection.Open();
                SqlDataReader reader = selectCommand.ExecuteReader(CommandBehavior.SingleRow);
                EmploymentDetails details = new EmploymentDetails();

                if (reader.Read())
                {
                    details.Employer = reader["Employer"].ToString();
                    details.JobTitle = reader["JobTitle"].ToString();

                    return details;
                }
                else
                {
                    return details;
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

        public static Property GetPropertyInfo(int propertyID)
        {
            SqlConnection connection = ConnectionString.GetConnection();

            string selectStatement
                = "SELECT [Address 1], [Address 2], City, State, ZipCode, PropertyValue, RentCharge  "
                + "FROM [Goke's Rentals].[dbo].[Property] "
                + "WHERE PropertyID = @PropertyID";


            SqlCommand selectCommand = new SqlCommand(selectStatement, connection);
            selectCommand.Parameters.AddWithValue("@PropertyID", propertyID);

            try
            {
                connection.Open();
                SqlDataReader reader = selectCommand.ExecuteReader(CommandBehavior.SingleRow);
                var property = new Property();

                if (reader.Read())
                {
                    property.Address1 = reader["Address 1"].ToString();
                    property.Address2 = reader["Address 2"].ToString();
                    property.City = reader["City"].ToString();
                    property.State = reader["State"].ToString();
                    property.Zipcode = (int)reader["ZipCode"];
                    property.PropertyValue = (decimal)reader["PropertyValue"];
                    property.RentCharge = (decimal)reader["RentCharge"];
                    return property;
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

        public static bool ValidateTenantEmail(string email)
        {
            SqlConnection connection = ConnectionString.GetConnection();

            string selectStatement
                = "SELECT EmailAddress  "
                + "FROM [Goke's Rentals].[dbo].[Tenant] "
                + "WHERE EmailAddress = @EmailAddress";


            SqlCommand selectCommand = new SqlCommand(selectStatement, connection);
            selectCommand.Parameters.AddWithValue("@EmailAddress", email);
            try
            {
                connection.Open();
                SqlDataReader reader = selectCommand.ExecuteReader(CommandBehavior.SingleRow);

                if (reader.Read())
                {
                    return true;
                }
                else
                {
                    return false;
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

        public static Tenant GetTenantByEmail(string email)
        {
            SqlConnection connection = ConnectionString.GetConnection();

            string selectStatement
                = "SELECT TenantId, PropertyID, FirstName, LastName, Password, PhoneNumber, StripeID, StripeIsVerified, ContractLink, Employer, JobTitle, Role "
                + "FROM [Goke's Rentals].[dbo].[Tenant] "
                + "WHERE EmailAddress = @EmailAddress";


            SqlCommand selectCommand = new SqlCommand(selectStatement, connection);
            selectCommand.Parameters.AddWithValue("@EmailAddress", email);

            try
            {
                connection.Open();
                SqlDataReader reader = selectCommand.ExecuteReader(CommandBehavior.SingleRow);

                if (reader.Read())
                {
                    Tenant tenant = new Tenant();
                    tenant.TenantID = (int)reader["TenantId"];
                    tenant.PropertyID = (int)reader["PropertyID"];
                    tenant.FirstName = reader["FirstName"].ToString();
                    tenant.LastName = reader["LastName"].ToString();
                    tenant.PasswordHash = reader["Password"].ToString();
                    tenant.StripeID = reader["StripeID"].ToString();
                    tenant.StripeIsVerified = reader["StripeIsVerified"] == DBNull.Value ? false : true;
                    tenant.ContractLink = reader["ContractLink"].ToString();
                    tenant.Employer = reader["Employer"].ToString();
                    tenant.JobTitle = reader["JobTitle"].ToString();
                    tenant.Role = reader["Role"].ToString();

                    return tenant;
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

        public static Administrator GetAdminByEmail(string email)
        {
            SqlConnection connection = ConnectionString.GetConnection();

            string selectStatement
                = "SELECT  ID, FirstName, LastName, PasswordHash, PhoneNumber, Role "
                + "FROM [Goke's Rentals].[dbo].[Administrator] "
                + "WHERE EmailAddress = @EmailAddress";


            SqlCommand selectCommand = new SqlCommand(selectStatement, connection);
            selectCommand.Parameters.AddWithValue("@EmailAddress", email);

            try
            {
                connection.Open();
                SqlDataReader reader = selectCommand.ExecuteReader(CommandBehavior.SingleRow);

                if (reader.Read())
                {
                    Administrator admin = new Administrator();
                    admin.ID = (int)reader["ID"];
                    admin.FirstName = reader["FirstName"].ToString();
                    admin.LastName = reader["LastName"].ToString();
                    admin.EmailAddress = email;
                    admin.PasswordHash = reader["PasswordHash"].ToString();
                    admin.PhoneNumber = reader["PhoneNumber"].ToString();
                    admin.Role = reader["Role"].ToString();
                    return admin;
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

        public static bool SaveTenantByAdministrator(Tenant tenant)
        {
            SqlConnection connection = ConnectionString.GetConnection();

            string updatestatement =
                "UPDATE [Goke's Rentals].[dbo].[Tenant] " +
                "SET FirstName = @FirstName, LastName = @LastName, PropertyID = @PropertyID, EmailAddress = @EmailAddress, PhoneNumber = @PhoneNumber, ContractLink = @ContractLink " +
                "WHERE TenantId = @TenantId";
            SqlCommand updatecommand = new SqlCommand(updatestatement, connection);
            tenant = AddEmptyString(tenant);
            updatecommand.Parameters.AddWithValue("@FirstName", tenant.FirstName);
            updatecommand.Parameters.AddWithValue("@LastName", tenant.LastName);
            updatecommand.Parameters.AddWithValue("@PropertyID", tenant.PropertyID);
            updatecommand.Parameters.AddWithValue("@EmailAddress", tenant.EmailAddress);
            updatecommand.Parameters.AddWithValue("@PhoneNumber", tenant.PhoneNumber);
            updatecommand.Parameters.AddWithValue("@ContractLink", tenant.ContractLink);
            updatecommand.Parameters.AddWithValue("@TenantId", tenant.TenantID);

            try
            {
                connection.Open();
                int count = updatecommand.ExecuteNonQuery();
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

        public static bool SavePropertyByAdministrator(Property property)
        {
            SqlConnection connection = ConnectionString.GetConnection();

            string updatestatement =
                "UPDATE [Goke's Rentals].[dbo].[Property] " +
                "SET [Address 1] = @Address1, [Address 2] = @Address2, City = @City, State = @State, ZipCode = @ZipCode, PropertyValue = @PropertyValue, RentCharge = @RentCharge " +
                "WHERE PropertyID = @PropertyID";
            SqlCommand updatecommand = new SqlCommand(updatestatement, connection);
            updatecommand.Parameters.AddWithValue("@Address1", property.Address1);
            updatecommand.Parameters.AddWithValue("@Address2", property.Address2);
            updatecommand.Parameters.AddWithValue("@City", property.City);
            updatecommand.Parameters.AddWithValue("@State", property.State);
            updatecommand.Parameters.AddWithValue("@ZipCode", property.Zipcode);
            updatecommand.Parameters.AddWithValue("@PropertyValue", property.PropertyValue);
            updatecommand.Parameters.AddWithValue("@RentCharge", property.RentCharge);
            updatecommand.Parameters.AddWithValue("@PropertyID", property.PropertyID);

            try
            {
                connection.Open();
                int count = updatecommand.ExecuteNonQuery();
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

        public static decimal GetPropertyExpense(int propertyID)
        {
            SqlConnection connection = ConnectionString.GetConnection();

            string updatestatement =
                "SELECT COST from [Goke's Rentals].[dbo].[MaintenanceTicket] WHERE PropertyID = @PropertyID AND COST IS NOT NULL ";
            SqlCommand updatecommand = new SqlCommand(updatestatement, connection);
            updatecommand.Parameters.AddWithValue("@PropertyID", propertyID);

            try
            {
                connection.Open();
                var reader = updatecommand.ExecuteReader();
                if (reader.HasRows)
                {
                    decimal total = 0;

                    while (reader.Read())
                    {
                        total += (decimal)reader["Cost"];
                    }
                    return total;
                }
                else
                {
                    return 0;
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

        public static decimal GetPropertyRevenue(int propertyID)
        {
            SqlConnection connection = ConnectionString.GetConnection();

            string selectstatement =
                "SELECT PaymentID, Payment.InvoiceID, PaymentAmount, PropertyID From Payment Left JOIN Invoice ON Payment.InvoiceID = Invoice.InvoiceID WHERE PropertyID = @PropertyID ";
            SqlCommand selectcommand = new SqlCommand(selectstatement, connection);
            selectcommand.Parameters.AddWithValue("@PropertyID", propertyID);

            try
            {
                connection.Open();
                var reader = selectcommand.ExecuteReader();

                if (reader.HasRows)
                {
                    decimal total = 0;

                    while (reader.Read())
                    {
                        total += (decimal)reader["PaymentAmount"];
                    }
                    return total;
                }
                else
                {
                    return 0;
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

        public static int CountMtnceRequests(int propertyID)
        {
            SqlConnection connection = ConnectionString.GetConnection();

            string countstatement =
                "SELECT COUNT(TicketID) " +
                "FROM [Goke's Rentals].[dbo].[MaintenanceTicket] " +
                "WHERE PropertyID = @PropertyID AND TicketActive = 1";


            SqlCommand countcommand = new SqlCommand(countstatement, connection);
            countcommand.Parameters.AddWithValue("@PropertyID", propertyID);
            connection.Open();

            Int32 count = (Int32)countcommand.ExecuteScalar();

            try
            {
                if (count > 0)
                {
                    return count;
                }
                else
                {
                    count = 0;
                    return count;
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

        public static bool DoesPropertyHaveTenants(int propertyID)
        {
            SqlConnection connection = ConnectionString.GetConnection();

            string selectstatement =
                "SELECT EmailAddress FROM Tenant WHERE PropertyID = @PropertyID ";
            SqlCommand selectcommand = new SqlCommand(selectstatement, connection);
            selectcommand.Parameters.AddWithValue("@PropertyID", propertyID);

            try
            {
                connection.Open();
                var reader = selectcommand.ExecuteReader();

                if (reader.HasRows)
                {
                    return true;
                }
                else
                {
                    return false;
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

        public static bool DeleteProperty(int propertyID)
        {
            SqlConnection connection = ConnectionString.GetConnection();

            string selectstatement =
                "DELETE FROM Property WHERE PropertyID = @PropertyID";
            SqlCommand selectcommand = new SqlCommand(selectstatement, connection);
            selectcommand.Parameters.AddWithValue("@PropertyID", propertyID);

            try
            {
                connection.Open();
                var rows = selectcommand.ExecuteNonQuery();

                if (rows > 0)
                {
                    return true;
                }
                else
                {
                    return false;
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

        public static bool ReassignMaintenanceTickets(int propertyID)
        {
            SqlConnection connection = ConnectionString.GetConnection();

            string selectstatement =
                "Update MaintenanceTicket SET PropertyID = 10 WHERE PropertyID = @PropertyID";
            SqlCommand selectcommand = new SqlCommand(selectstatement, connection);
            selectcommand.Parameters.AddWithValue("@PropertyID", propertyID);
            try
            {
                connection.Open();
                var rows = selectcommand.ExecuteNonQuery();

                if (rows >= 0)
                {
                    return true;
                }
                else
                {
                    return false;
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

        public static bool ReassignInvoices(int propertyID)
        {
            SqlConnection connection = ConnectionString.GetConnection();

            string selectstatement =
                "Update Invoice SET PropertyID = 10 WHERE PropertyID = @PropertyID";
            SqlCommand selectcommand = new SqlCommand(selectstatement, connection);
            selectcommand.Parameters.AddWithValue("@PropertyID", propertyID);
            try
            {
                connection.Open();
                var rows = selectcommand.ExecuteNonQuery();

                if (rows >= 0)
                {
                    return true;
                }
                else
                {
                    return false;
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

        public static bool DeleteTenant(int tenantID)
        {
            SqlConnection connection = ConnectionString.GetConnection();

            string selectstatement =
                "ALTER TABLE dbo.MaintenanceTicket nocheck constraint all; " + 
                "DELETE FROM Tenant Where TenantId = @TenantId; " +
                "ALTER TABLE dbo.MaintenanceTicket check constraint all";
            SqlCommand selectcommand = new SqlCommand(selectstatement, connection);
            selectcommand.Parameters.AddWithValue("@TenantId", tenantID);
            try
            {
                connection.Open();
                var rows = selectcommand.ExecuteNonQuery();

                if (rows > 0)
                {
                    return true;
                }
                else
                {
                    return false;
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

        public static Payment GetLastPayment(int tenantID)
        {
            SqlConnection connection = ConnectionString.GetConnection();
            string selectstatement =
                "SELECT * From Payment WHERE TenantId = @TenantId";
            SqlCommand selectcommand =
                new SqlCommand(selectstatement, connection);
            selectcommand.Parameters.AddWithValue("@TenantId", tenantID);

            try
            {
                connection.Open();
                var reader = selectcommand.ExecuteReader();

                if (reader.Read())
                {
                    Payment payment = new Payment()
                    {
                        PaymentID = (int)reader["PaymentID"],
                        InvoiceID = (int)reader["InvoiceID"],
                        PaymentAmount = (decimal)reader["PaymentAmount"],
                        Receipt = reader["Receipt"].ToString(),
                        Date = (DateTime)reader["Date"],
                        TenantId = tenantID
                    };

                    return payment;
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
    }
}

