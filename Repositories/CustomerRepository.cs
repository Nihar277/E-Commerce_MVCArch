using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVC.Models;
using Npgsql;

namespace MVC.Repositories
{
    public class CustomerRepository
    {
        public readonly NpgsqlConnection conn;
        public CustomerRepository(NpgsqlConnection connection)
        {
            conn=connection;
        }
        public async Task<CustomerModel> Login(vm_Login login)
        {
            CustomerModel customer1=new CustomerModel();
            string query="SELECT * from t_shopping_customer where c_email=@email AND c_password=@password;";

            try
            {
                await conn.CloseAsync();
                var cmd=new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("email", login.Email);
                cmd.Parameters.AddWithValue("password", login.Password);
                await conn.OpenAsync();
                var reader=cmd.ExecuteReader();
                if (reader.Read())
                {
                    customer1.Customerid=(int)reader["c_id"];
                    customer1.Email=(string)reader["c_email"];
                    customer1.Password=(string)reader["c_password"];
                    customer1.Name=(string)reader["c_name"];
                }    
                return customer1;
            }
            catch (Exception ex)
            {
                await conn.CloseAsync();
                Console.WriteLine("Login Error: " + ex.Message);
                return customer1;
            }
            finally
            {
                await conn.CloseAsync();
            }
        }

        public async Task<CustomerModel> GetCustomerById(int id)
        {
            CustomerModel? customer=null;
            try
            {
                await conn.CloseAsync();
                string query=@"SELECT * from t_shopping_customer where c_id=@id";
                var cmd=new NpgsqlCommand(query, conn);
                await conn.OpenAsync();
                cmd.Parameters.AddWithValue("id", id);
                
                var reader=cmd.ExecuteReader();
                if (reader.Read())
                {
                    customer=new CustomerModel()
                    {
                        Customerid=(int)reader["c_id"],
                        Email=(string)reader["c_email"],
                        Password=(string)reader["c_password"],
                        Name=(string)reader["c_name"],
                    };
                }
                return customer;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Id Error: " + ex.Message);
                await conn.CloseAsync();
                return customer;
            }
            finally
            {
                await conn.CloseAsync();
            }
        }
    }
}