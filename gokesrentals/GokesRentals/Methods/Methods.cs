using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using GokesRentals.Services;
using GokesRentals.Objects;


namespace GokesRentals.Methods
{
    public class Methods
    {
        public static Tenant GetTenant(int TenantID)
        {
            SqlConnection connection = ConnectionString.GetConnection();
            string selectStatement
                = "SELECT PropertyID, FirstName, LastName, EmailAddress, PhoneNumber,  "
                + "StripeID, StripeIsVerified, Employer, JobTitle, Role "
                + "FROM Tenant "
                + "WHERE TenantId = @TenantId";
            SqlCommand selectCommand =
                new SqlCommand(selectStatement, connection);
            selectCommand.Parameters.AddWithValue("@TenantId", TenantID);

            try
            {
                connection.Open();
                SqlDataReader tenantReader =
                    selectCommand.ExecuteReader(CommandBehavior.SingleRow);
                if (tenantReader.Read())
                {
                    Tenant CurrentTenant = new Tenant();
                    CurrentTenant.TenantID = TenantID;
                    CurrentTenant.PropertyID = (int)tenantReader["PropertyID"];
                    CurrentTenant.FirstName = tenantReader["FirstName"].ToString();
                    CurrentTenant.LastName = tenantReader["LastName"].ToString();
                    CurrentTenant.EmailAddress = tenantReader["EmailAddress"].ToString();
                    CurrentTenant.PhoneNumber = tenantReader["PhoneNumber"].ToString();
                    CurrentTenant.StripeID = tenantReader["StripeID"].ToString();
                    CurrentTenant.StripeIsVerified = tenantReader["StripeIsVerified"] == DBNull.Value ? false : (bool)tenantReader["StripeIsVerified"];
                    CurrentTenant.Role = tenantReader["Role"].ToString();
                    return CurrentTenant;
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

        public static Administrator GetAdministrator(string AdminID)
        {
            SqlConnection connection = ConnectionString.GetConnection();
            string selectStatement
                = "SELECT ID, FirstName, LastName, EmailAddress, PhoneNumber, Role "
                + "FROM Administrator "
                + "WHERE ID = @AdminID";
            SqlCommand selectCommand =
                new SqlCommand(selectStatement, connection);
            selectCommand.Parameters.AddWithValue("@AdminID", AdminID);

            try
            {
                connection.Open();
                SqlDataReader tenantReader =
                    selectCommand.ExecuteReader(CommandBehavior.SingleRow);
                if (tenantReader.Read())
                {
                    Administrator currentAdministrator = new Administrator();
                    currentAdministrator.ID = (int)tenantReader["ID"];
                    currentAdministrator.FirstName = tenantReader["FirstName"].ToString();
                    currentAdministrator.LastName = tenantReader["LastName"].ToString();
                    currentAdministrator.EmailAddress = tenantReader["EmailAddress"].ToString();
                    currentAdministrator.PhoneNumber = tenantReader["PhoneNumber"].ToString();
                    return currentAdministrator; 
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

        public static MaintenanceRequest CreateMaintenanceRequest(int TenantID, string Summary, string Description)
        {
            SqlConnection connection = ConnectionString.GetConnection();
            string selectStatement
                = "SELECT PropertyID, FirstName, LastName, EmailAddress, PhoneNumber  "
                + "FROM Tenant "
                + "WHERE TenantID = @TenantID";
            SqlCommand selectCommand =
                new SqlCommand(selectStatement, connection);
            selectCommand.Parameters.AddWithValue("@TenantID", TenantID);

            try
            {
                connection.Open();
                SqlDataReader tenantReader =
                    selectCommand.ExecuteReader(CommandBehavior.SingleRow);
                if (tenantReader.Read())
                {
                    MaintenanceRequest MaintenanceRequest = new MaintenanceRequest();


                        MaintenanceRequest.TenantID = (int)TenantID; 
                        MaintenanceRequest.PropertyID = (int)tenantReader["PropertyID"]; //HARDCODED VALUE
                        MaintenanceRequest.FirstName = tenantReader["FirstName"].ToString();
                        MaintenanceRequest.LastName = tenantReader["LastName"].ToString();
                        MaintenanceRequest.Summary = Summary; 
                        MaintenanceRequest.Description = Description; 
                        MaintenanceRequest.OpenDate = DateTime.UtcNow.ToString();
                    MaintenanceRequest.ActiveRequest = true;
                   
                    return MaintenanceRequest;
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

        public static Tenant CreateTenantClass(string firstName, string lastName, int propertyID, string emailAddress, string phoneNumber, string contract)
        {
            try
            {
                if (firstName != null && lastName != null && propertyID != null && emailAddress != null && phoneNumber != null)
                {
                    Tenant tenant = new Tenant();

                    tenant.FirstName = firstName;
                    tenant.LastName = lastName;
                    tenant.PropertyID = propertyID;
                    tenant.EmailAddress = emailAddress;
                    tenant.PhoneNumber = phoneNumber;
                    tenant.ContractLink = contract;

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
        }

        public static Property CreatePropertyClass(string address1, string address2, string city, string state, int? zipcode, decimal? propertyValue, decimal? rentCharge )
        {
            try
            {
                if (address1 != null && address2 != null && city != null && state != null && zipcode !=null && propertyValue != null && rentCharge !=null)
                {
                    Property property = new Property();

                    property.Address1 = address1;
                    property.Address2 = address2;
                    property.City = city;
                    property.State = state;
                    property.Zipcode = (int)zipcode;
                    property.PropertyValue = (decimal)propertyValue;
                    property.RentCharge = (decimal)rentCharge;

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
        }
        public static PersonalInformation UserInputPersonalInformation(int TenantID, string FirstName, string LastName, string Phone, string Email)
        {
            PersonalInformation PersonalInformation = new PersonalInformation
            {
                TenantID = TenantID, //hardcoded value
                FirstName = FirstName,
                LastName = LastName,
                Phone = Phone,
                Email = Email
            };
            return PersonalInformation;
        }

        public static EmploymentDetails UserInputEmploymentDetails(int TenantID, string Employer, string JobTitle)
        {
            EmploymentDetails EmploymentDetails = new EmploymentDetails
            {
                TenantID = TenantID, //Hardcoded Value
                Employer = Employer,
                JobTitle = JobTitle
            };
            return EmploymentDetails;
        }

        //Takes all requests for a given user and places them in a list to be displayed on portal
        public static List<MaintenanceRequest> RetrieveMaitenanceRequests(int PropertyID)
        {
            SqlConnection connection = ConnectionString.GetConnection();
            string selectStatement
                = "SELECT * "
                + "FROM [Goke's Rentals].[dbo].[MaintenanceTicket] "
                + "WHERE PropertyID = @PropertyID "
                + "ORDER BY TicketID DESC";
            SqlCommand selectCommand =
                new SqlCommand(selectStatement, connection);
            selectCommand.Parameters.AddWithValue("@PropertyID", PropertyID);

            try
            {
                connection.Open();

                SqlDataReader reader = selectCommand.ExecuteReader();
                List<MaintenanceRequest> MR = new List<MaintenanceRequest>();
                while (reader.HasRows && reader.Read())
                {
                    MaintenanceRequest maintenanceRequests = new MaintenanceRequest();
                    maintenanceRequests.TenantID = (int)reader["TenantID"];
                    maintenanceRequests.PropertyID = (int)reader["PropertyID"];
                    maintenanceRequests.TicketID = (int)reader["TicketID"];
                    maintenanceRequests.Summary = (string)reader["Summary"];
                    maintenanceRequests.Description = (string)reader["Description"];
                    maintenanceRequests.OpenDate = reader["OpenDate"].ToString();
                    maintenanceRequests.EndDate = reader["CloseDate"].ToString();
                    maintenanceRequests.ActiveRequest = (bool)reader["TicketActive"];
                    MR.Add(maintenanceRequests);
                }
                return MR;

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
        //Takes all requests for EVERY user and places them in a list to be displayed on portal
        public static List<MaintenanceRequest> RetrieveAllMaitenanceRequests()
        {
            SqlConnection connection = ConnectionString.GetConnection();
            string selectStatement
                = "SELECT * "
                + "FROM [Goke's Rentals].[dbo].[MaintenanceTicket] "
                + "ORDER BY TicketID DESC";
            SqlCommand selectCommand =
                new SqlCommand(selectStatement, connection);

            try
            {
                connection.Open();

                SqlDataReader reader = selectCommand.ExecuteReader();
                List<MaintenanceRequest> MR = new List<MaintenanceRequest>();
                while (reader.HasRows && reader.Read())
                {
                    MaintenanceRequest maintenanceRequests = new MaintenanceRequest();
                    maintenanceRequests.TenantID = (int)reader["TenantID"];
                    maintenanceRequests.PropertyID = (int)reader["PropertyID"];
                    maintenanceRequests.TicketID = (int)reader["TicketID"];
                    maintenanceRequests.Summary = (string)reader["Summary"];
                    maintenanceRequests.Description = (string)reader["Description"];
                    maintenanceRequests.OpenDate = reader["OpenDate"].ToString();
                    maintenanceRequests.ActiveRequest = (bool)reader["TicketActive"];
                    maintenanceRequests.EndDate = reader["CloseDate"].ToString();
                    MR.Add(maintenanceRequests);
                }
                return MR;

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

        //Retrieves all property information and loads it into a list to be displayed on the admin portal
        public static List<Property> RetrieveAllProperties()
        {
            SqlConnection connection = ConnectionString.GetConnection();
            string selectStatement
                = "SELECT * "
                + "FROM [Goke's Rentals].[dbo].[Property]";
            SqlCommand selectCommand =
                new SqlCommand(selectStatement, connection);

            try
            {
                connection.Open();

                SqlDataReader reader = selectCommand.ExecuteReader();
                List<Property> PR = new List<Property>();
                while (reader.HasRows && reader.Read())
                {
                    Property AllProperties = new Property();
                    AllProperties.PropertyID = (int)reader["PropertyID"];
                    AllProperties.Address1 = (string)reader["Address 1"];
                    AllProperties.Address2 = (string)reader["Address 2"];
                    AllProperties.City = (string)reader["City"];
                    AllProperties.State = (string)reader["State"];
                    AllProperties.Zipcode = (int)reader["ZipCode"];
                    AllProperties.PropertyValue = (decimal)reader["PropertyValue"];
                    AllProperties.RentCharge = (decimal)reader["RentCharge"];
                    PR.Add(AllProperties);
                }
                return PR;

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

        //Retrieves all tenant information and loads it into a list to be displayed on the admin portal
        public static List<Tenant> RetrieveAllTenants()
        {
            SqlConnection connection = ConnectionString.GetConnection();
            string selectStatement
                = "SELECT * "
                + "FROM [Goke's Rentals].[dbo].[Tenant]";
            SqlCommand selectCommand =
                new SqlCommand(selectStatement, connection);

            try
            {
                connection.Open();

                SqlDataReader reader = selectCommand.ExecuteReader();
                List<Tenant> PR = new List<Tenant>();
                while (reader.HasRows && reader.Read())
                {
                    Tenant AllTenants = new Tenant();
                    AllTenants.TenantID = (int)reader["TenantID"];
                    AllTenants.PropertyID = (int)reader["PropertyID"];
                    AllTenants.FirstName = (string)reader["FirstName"];
                    AllTenants.LastName = (string)reader["LastName"];
                    AllTenants.EmailAddress = (string)reader["EmailAddress"];
                    AllTenants.PhoneNumber = (string)reader["PhoneNumber"];
                    AllTenants.ContractLink = reader["ContractLink"].ToString();
                    PR.Add(AllTenants);
                }
                return PR;

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

        public static List<Invoice> GetInvoices(int propertyID)
        {
            SqlConnection connection = ConnectionString.GetConnection();
            string selectStatement
                = "SELECT * "
                + "FROM [Goke's Rentals].[dbo].[Invoice] WHERE PropertyID = @PropertyID";
            SqlCommand selectCommand = new SqlCommand(selectStatement, connection);
            selectCommand.Parameters.AddWithValue("@PropertyID", propertyID);

            try
            {
                connection.Open();

                SqlDataReader reader = selectCommand.ExecuteReader();
                List<Invoice> invoices = new List<Invoice>();
                while (reader.HasRows && reader.Read())
                {
                    Invoice inv = new Invoice();
                    inv.InvoiceID = (int)reader["InvoiceID"];
                    inv.InvoiceNumber = (Guid)reader["InvoiceNumber"];
                    inv.AmountDue = (decimal)reader["AmountDue"];
                    inv.BillDate = (DateTime)reader["BillDate"];
                    inv.DueDate = (DateTime)reader["DueDate"];
                    inv.PropertyID = (int)reader["PropertyID"];
                    inv.PaidInFull = (bool)reader["PaidInFull"];
                    invoices.Add(inv);
                }
                    return invoices;
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
