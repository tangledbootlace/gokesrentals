using Folly;
using Folly.Auth;
using GokesRentals.Objects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace GokesRentals.Services
{
    public static class AccountRecoveryService
    {
        public static void RequestResetToken(string email, bool isAdmin = false)
        {

            if (isAdmin)
            {
                var admin = SQL.SQLStatements.GetAdminByEmail(email);

                if (admin == null)
                    throw new RecoveryException("No email found.");

                var recovery = new Recovery()
                {
                    userID = admin.ID,
                    Token = StringHelper.RandomString(100),
                    ExpirationDate = DateTime.UtcNow.AddHours(24)
                };
                if (SaveToken(recovery, true))
                {
                    admin.EmailAddress = email;
                    SendRecoveryEmail(admin, recovery.Token);
                }
            }
            else
            {
                var user = SQL.SQLStatements.GetTenantByEmail(email);

                if (user == null)
                    throw new RecoveryException("No email found.");

                var recovery = new Recovery()
                {
                    userID = user.TenantID,
                    Token = StringHelper.RandomString(100),
                    ExpirationDate = DateTime.UtcNow.AddHours(24)
                };

                if (SaveToken(recovery))
                {
                    user.EmailAddress = email;
                    SendRecoveryEmail(user, recovery.Token);
                }
                else
                {
                    throw new RecoveryException("Could not generate recovery token");
                }
            }


        }

        public static bool ValidateResetToken(string token, bool isAdmin = false)
        {
            if (token == null)
                return false;

            var recoveryToken = new Recovery();
            
            recoveryToken = GetRecoveryToken(token, isAdmin);
            

            if (recoveryToken == null || recoveryToken.IsExpired())
                return false;

            return true;
        }

        public static bool RedeemResetToken(string password, string token, bool isAdmin = false)
        {
            var success = false;
            var tenant = new Tenant();
            var admin = new Administrator();
            var recoveryToken = GetRecoveryToken(token, isAdmin);
            var passwordHash = PasswordHash.CreateHash(password);
            password = null;

            if (recoveryToken == null)
                throw new RecoveryException("Invalid reset token");

            if (isAdmin)
            {
                admin = Methods.Methods.GetAdministrator(recoveryToken.userID.ToString());
                success = UpdatePasswordNoComparison(admin.ID, passwordHash, isAdmin);
            }
            else
            {
                tenant = Methods.Methods.GetTenant(recoveryToken.userID);

                success = UpdatePasswordNoComparison(tenant.TenantID, passwordHash, isAdmin);
            }



            if (success)
            {
                DeleteToken(recoveryToken);
                return true;
            }
            return false;


        }

        private static bool SaveToken(Recovery recovery, bool isAdmin = false)
        {
            SqlConnection connection = ConnectionString.GetConnection();
            string insertstatement =
                "Insert into [Goke's Rentals].[dbo].[RecoveryToken] " + "(tenantID, token, expirationDate, isAdmin) " +
                "VALUES (@TenantID, @token, @expirationDate, @isAdmin)";
            SqlCommand insertcommand =
                new SqlCommand(insertstatement, connection);
            insertcommand.Parameters.AddWithValue("@TenantID", recovery.userID);
            insertcommand.Parameters.AddWithValue("@token", recovery.Token);
            insertcommand.Parameters.AddWithValue("@expirationDate", recovery.ExpirationDate);
            insertcommand.Parameters.AddWithValue("@isAdmin", isAdmin);
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

        private static bool DeleteToken(Recovery recovery)
        {
            SqlConnection connection = ConnectionString.GetConnection();
            string insertstatement =
                "DELETE FROM [Goke's Rentals].[dbo].[RecoveryToken] " + "WHERE TenantID = @TenantId";
            SqlCommand insertcommand =
                new SqlCommand(insertstatement, connection);
            insertcommand.Parameters.AddWithValue("@TenantID", recovery.userID);
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

        private static Recovery GetRecoveryToken(string token, bool isAdmin)
        {
            SqlConnection connection = ConnectionString.GetConnection();
            string selectstatement;
            if (isAdmin)
            {
                selectstatement =
                "SELECT id, tenantID, token, expirationDate  FROM [Goke's Rentals].[dbo].[RecoveryToken] WHERE token = @token AND isAdmin = @isAdmin";
            }
            else
            {
                selectstatement = "SELECT id, tenantID, token, expirationDate  FROM [Goke's Rentals].[dbo].[RecoveryToken] WHERE token = @token AND isAdmin = @isAdmin";
            }
            
            SqlCommand selectcommand =  new SqlCommand(selectstatement, connection);
            selectcommand.Parameters.AddWithValue("@token", token);
            selectcommand.Parameters.AddWithValue("@isAdmin", isAdmin);

            try
            {
                connection.Open();
                SqlDataReader reader = selectcommand.ExecuteReader(CommandBehavior.SingleRow);
                if (reader.Read())
                {
                    Recovery recovery = new Recovery();
                    recovery.userID = (int)reader["tenantID"];
                    recovery.Token = reader["token"].ToString();
                    recovery.ExpirationDate = (DateTime)reader["expirationDate"];
                    return recovery;
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

        private static void SendRecoveryEmail(Tenant t, string token)
        {
            string subject = "Reset your Goke's Rentals password";
            string body = "Hey there! Someone requested a password reset for your Goke's Rental account. You may reset your password here: https://gokesrentals.azurewebsites.net/reset-password?token=" + token;
            EmailService.SendEmail(subject, body, t.EmailAddress);
        }
        private static void SendRecoveryEmail(Administrator a, string token)
        {
            string subject = "Reset your Goke's Rentals password";
            string body = "Hey there! Someone requested a password reset for your Goke's Rental account. You may reset your password here: https://gokesrentals.azurewebsites.net/admin/reset-password?token=" + token;
            EmailService.SendEmail(subject, body, a.EmailAddress);
        }

        public static bool UpdatePasswordNoComparison(int ID, string newPassword, bool isAdmin = false)
        {
            SqlConnection connection = ConnectionString.GetConnection();
            SqlCommand insertcommand = new SqlCommand();
            string insertStatement;

            if (isAdmin)
            {
                insertStatement = "UPDATE [Goke's Rentals].[dbo].[Administrator] SET PasswordHash = @Password WHERE ID = @ID";
                insertcommand.CommandText = insertStatement;
                insertcommand.Connection = connection;

                insertcommand.Parameters.AddWithValue("@Password", newPassword);
                insertcommand.Parameters.AddWithValue("@ID", ID);
            }
            else
            {
                insertStatement = "Update [Goke's Rentals].[dbo].[Tenant] SET Password = @Password WHERE TenantId = @TenantId";
                insertcommand.CommandText = insertStatement;
                insertcommand.Connection = connection;

                insertcommand.Parameters.AddWithValue("@Password", newPassword);
                insertcommand.Parameters.AddWithValue("@TenantId", ID);
            }


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



    }
}
