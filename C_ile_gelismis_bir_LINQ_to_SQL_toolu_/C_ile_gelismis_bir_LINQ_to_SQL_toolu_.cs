using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Data.Linq;

namespace LinqToSqlTool
{
    public class LinqToSqlTool
    {
        private string connectionString;

        public LinqToSqlTool(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public List<Customer> GetCustomersByCity(string city)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var dataContext = new DataContext(connection);

                var query = from customer in dataContext.GetTable<Customer>()
                            where customer.City == city
                            select customer;

                return query.ToList();
            }
        }

        public void InsertCustomer(Customer customer)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var dataContext = new DataContext(connection);
                dataContext.GetTable<Customer>().InsertOnSubmit(customer);
                dataContext.SubmitChanges();
            }
        }

        public void UpdateCustomer(Customer customer)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var dataContext = new DataContext(connection);
                var existingCustomer = dataContext.GetTable<Customer>().SingleOrDefault(c => c.CustomerID == customer.CustomerID);

                if (existingCustomer != null)
                {
                    existingCustomer.CompanyName = customer.CompanyName;
                    existingCustomer.ContactName = customer.ContactName;
                    existingCustomer.City = customer.City;

                    dataContext.SubmitChanges();
                    Console.WriteLine("Customer updated successfully.");
                }
                else
                {
                    Console.WriteLine("Customer not found.");
                }
            }
        }

        public void DeleteCustomer(string customerId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var dataContext = new DataContext(connection);
                var customer = dataContext.GetTable<Customer>().SingleOrDefault(c => c.CustomerID == customerId);

                if (customer != null)
                {
                    dataContext.GetTable<Customer>().DeleteOnSubmit(customer);
                    dataContext.SubmitChanges();
                    Console.WriteLine("Customer deleted successfully.");
                }
                else
                {
                    Console.WriteLine("Customer not found.");
                }
            }
        }

        public void ExecuteCustomQuery(string query)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var dataContext = new DataContext(connection);

                var result = dataContext.ExecuteQuery<Customer>(query);

                foreach (var customer in result)
                {
                    Console.WriteLine($"CustomerID: {customer.CustomerID}, CompanyName: {customer.CompanyName}");
                }
            }
        }

        public void ExecuteCustomStoredProcedure(string storedProcedureName, SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var command = new SqlCommand(storedProcedureName, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                var dataAdapter = new SqlDataAdapter(command);
                var dataTable = new System.Data.DataTable();
                dataAdapter.Fill(dataTable);

                foreach (System.Data.DataRow row in dataTable.Rows)
                {
                    var customer = new Customer
                    {
                        CustomerID = row["CustomerID"].ToString(),
                        CompanyName = row["CompanyName"].ToString(),
                        ContactName = row["ContactName"].ToString(),
                        City = row["City"].ToString()
                    };

                    Console.WriteLine($"CustomerID: {customer.CustomerID}, CompanyName: {customer.CompanyName}");
                }
            }
        }

        public List<Customer> GetCustomersWithOrders()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var dataContext = new DataContext(connection);

                var query = from customer in dataContext.GetTable<Customer>()
                            join order in dataContext.GetTable<Order>() on customer.CustomerID equals order.CustomerID into customerOrders
                            select new Customer
                            {
                                CustomerID = customer.CustomerID,
                                CompanyName = customer.CompanyName,
                                ContactName = customer.ContactName,
                                City = customer.City,
                                Orders = customerOrders.ToList()
                            };

                return query.ToList();
            }
        }
    }

    public class Customer
    {
        public string CustomerID { get; set; }
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public string City { get; set; }
        public List<Order> Orders { get; set; }
    }

    public class Order
    {
        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "your_connection_string";

            var tool = new LinqToSqlTool(connectionString);

            List<Customer> customers = tool.GetCustomersByCity("London");
            foreach (var customer in customers)
            {
                Console.WriteLine($"CustomerID: {customer.CustomerID}, CompanyName: {customer.CompanyName}");
            }

            var newCustomer = new Customer
            {
                CustomerID = "NEWCUST",
                CompanyName = "New Company",
                ContactName = "John Doe",
                City = "New York"
            };

            tool.InsertCustomer(newCustomer);
            Console.WriteLine("Customer inserted successfully.");

            var updatedCustomer = new Customer
            {
                CustomerID = "NEWCUST",
                CompanyName = "Updated Company",
                ContactName = "Jane Smith",
                City = "Los Angeles"
            };

            tool.UpdateCustomer(updatedCustomer);

            string customerIdToDelete = "NEWCUST";
            tool.DeleteCustomer(customerIdToDelete);

            string customQuery = "SELECT * FROM Customers WHERE Country = 'Germany'";
            tool.ExecuteCustomQuery(customQuery);

            string storedProcedureName = "GetCustomersByCountry";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Country", "France")
            };
            tool.ExecuteCustomStoredProcedure(storedProcedureName, parameters);

            List<Customer> customersWithOrders = tool.GetCustomersWithOrders();
            foreach (var customer in customersWithOrders)
            {
                Console.WriteLine($"CustomerID: {customer.CustomerID}, CompanyName: {customer.CompanyName}");
                foreach (var order in customer.Orders)
                {
                    Console.WriteLine($"  OrderID: {order.OrderID}, OrderDate: {order.OrderDate}");
                }
            }

            Console.ReadLine();
        }
    }
}


#Sınıfın içinde müşteri verilerini şehre göre alma, müşteri ekleme, müşteri güncelleme, müşteri silme, özel bir sorgu çalıştırma, özel bir saklı yordam çalıştırma ve müşterileri siparişlerle birlikte alma gibi işlemler gerçekleştirilebilir.