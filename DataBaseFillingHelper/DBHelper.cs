using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseFillingHelper
{
    public class DBHelper
    {
        private static NpgsqlConnection? connection = null;
        //Single-Ton
        private static DBHelper instance = null;
        public static DBHelper getInstance(string server = "localhost", string port = "5432", string database = "onlineshopdb", string userID = "postgres", string password = "19374562")
        {
            if (instance == null)
            {
                instance = new DBHelper(server, port, database, userID, password);
            }
            return instance;
        }

        private DBHelper(string server, string port, string database, string userID, string password)
        {
            var connStr = $"Host={server};Port={port};Database={database};Username={userID};Password={password}";
            connection = new NpgsqlConnection(connStr);
            connection?.Open();
        }
        public string TestConnect()
        {
            if (connection != null && connection.State == System.Data.ConnectionState.Open)
            {
                return "Подключение к только что созданной БД успешно";
            }
            else if (connection == null)
            {
                return "Connection is null";
            }
            else return "Connection Error";
        }
        public void CreateDataBase()
        {
            Console.WriteLine("Начало создания Таблиц...");
            var CreateTableClient = "CREATE TABLE Client (\r\n  ID SERIAL PRIMARY KEY,\r\n  FirstName VARCHAR(255) NOT NULL,\r\n  LastName VARCHAR(255) NOT NULL,\r\n  email VARCHAR(255) NOT NULL UNIQUE,\r\n  password VARCHAR(255) NOT NULL,\r\n  phonenumber VARCHAR(255)\r\n);\r\n";
            var CreateTableAnOrder = "CREATE TABLE AnOrder (\r\n  ID SERIAL PRIMARY KEY,\r\n  Client_ID INTEGER NOT NULL REFERENCES Client(ID),\r\n  OrderDate DATE NOT NULL,\r\n  DeliveredDate DATE\r\n);\r\n";
            var CreateTableCategory = "CREATE TABLE Category (\r\n  ID SERIAL PRIMARY KEY,\r\n  Name VARCHAR(255) NOT NULL UNIQUE\r\n);\r\n";
            var CreateTableProduct = "CREATE TABLE Product (\r\n  ID SERIAL PRIMARY KEY,\r\n  Name VARCHAR(255) NOT NULL,\r\n  Category_ID INTEGER NOT NULL REFERENCES Category(ID),\r\n  price NUMERIC NOT NULL CHECK (price >= 0)\r\n);\r\n";
            var CreateTableInformation = "CREATE TABLE Information (\r\n  ID SERIAL PRIMARY KEY,\r\n  Name VARCHAR(255) NOT NULL,\r\n  title VARCHAR(255) NOT NULL,\r\n  Product_ID INTEGER NOT NULL REFERENCES Product(ID)\r\n);\r\n";
            var CreateTableProductInOrder = "CREATE TABLE Product_In_Order (\r\n  ID SERIAL PRIMARY KEY,\r\n  Order_ID INTEGER NOT NULL REFERENCES AnOrder(ID),\r\n  Product_ID INTEGER NOT NULL REFERENCES Product(ID),\r\n  amount INTEGER NOT NULL CHECK (amount >= 0)\r\n);\r\n";
            var CreateTableReview = "CREATE TABLE Review (\r\n  ID SERIAL PRIMARY KEY,\r\n  Client_ID INTEGER NOT NULL REFERENCES Client(ID),\r\n  Product_ID INTEGER NOT NULL REFERENCES Product(ID),\r\n  Rating INTEGER NOT NULL CHECK (Rating >= 1 AND Rating <= 5),\r\n  Review_text TEXT,\r\n  ReviewDate DATE\r\n);\r\n";

            // Запустить скрипт создания таблиц и других объектов базы данных.
            var createClients = connection.CreateCommand();
            createClients.CommandText = CreateTableClient;
            createClients.ExecuteNonQuery();
            Console.WriteLine("Client создана");

            var createOrders = connection.CreateCommand();
            createOrders.CommandText = CreateTableAnOrder;
            createOrders.ExecuteNonQuery();
            Console.WriteLine("Order создана");


            var createCategory = connection.CreateCommand();
            createCategory.CommandText = CreateTableCategory;
            createCategory.ExecuteNonQuery();
            Console.WriteLine("Category создана");


            var createProduct = connection.CreateCommand();
            createProduct.CommandText = CreateTableProduct;
            createProduct.ExecuteNonQuery();
            Console.WriteLine("Product создана");

            var createInformation = connection.CreateCommand();
            createInformation.CommandText = CreateTableInformation;
            createInformation.ExecuteNonQuery();
            Console.WriteLine("Information создана");

            var createProductInOrder = connection.CreateCommand();
            createProductInOrder.CommandText = CreateTableProductInOrder;
            createProductInOrder.ExecuteNonQuery();
            Console.WriteLine("Product_In_Order создана");

            var createReview = connection.CreateCommand();
            createReview.CommandText = CreateTableReview;
            createReview.ExecuteNonQuery();
            Console.WriteLine("Review создана");
        }
        public void FillDataBase(EntitiesGenerator EG)
        {
            foreach (var category in EG.Categories)
            {
                InsertNewCategory(category);
            }
            foreach (var client in EG.Clients)
            {
                InsertNewClient(client);
            }
            foreach (var product in EG.Products)
            {
                InsertNewProduct(product);
            }
            foreach (var order in EG.Orders)
            {
                InsertNewOrder(order);
            }
            foreach (var review in EG.Reviews)
            {
                InsertNewReview(review);
            }
        }

        public void InsertNewClient(Client newClient)
        {
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO client (FirstName, LastName, Email, Password, PhoneNumber) " +
                                  "Values (@firstname, @lastname, @email, @password, @phonenumber);";

                cmd.Parameters.Add(new NpgsqlParameter("@firstname", newClient.FirstName));
                cmd.Parameters.Add(new NpgsqlParameter("@lastname", newClient.LastName));
                try
                {
                    cmd.Parameters.Add(new NpgsqlParameter("@email", newClient.Email));
                }
                catch (Exception ex)
                {
                    var random = new Random();
                    cmd.Parameters.Add(new NpgsqlParameter("@email", $"zloiChuvak{random.Next(10000)}"));
                }

                cmd.Parameters.Add(new NpgsqlParameter("@password", newClient.Password));
                cmd.Parameters.Add(new NpgsqlParameter("@phonenumber", newClient.PhoneNumber == null ? DBNull.Value : newClient.PhoneNumber));


                cmd.ExecuteNonQuery();
            }
        }
        public void InsertNewProduct(Product newProduct)
        {
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO product (name, price, Category_ID) " +
                                  "Values (@name, @price, @category_id);";

                cmd.Parameters.Add(new NpgsqlParameter("@name", newProduct.Name));
                cmd.Parameters.Add(new NpgsqlParameter("@price", newProduct.Price));
                cmd.Parameters.Add(new NpgsqlParameter("@category_id", newProduct.CategoryID));

                cmd.ExecuteNonQuery();
            }
        }
        public void InsertNewOrder(AnOrder newOrder)
        {
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO anorder (client_id, orderdate, delivereddate) " +
                                  "Values (@client_id, @orderdate, @delivereddate);";

                cmd.Parameters.Add(new NpgsqlParameter("@client_id", newOrder.ClientID));
                cmd.Parameters.Add(new NpgsqlParameter("@orderdate", newOrder.OrderDate));
                cmd.Parameters.Add(new NpgsqlParameter("@delivereddate", newOrder.DeliveredDate == null ? DBNull.Value : newOrder.DeliveredDate));

                cmd.ExecuteNonQuery();
            }
        }
        public void InsertNewProductInOrder(ProductInOrder newProductInOrder)
        {
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO product_in_order (order_id, product_id, amount) " +
                                  "Values (@order_id, @product_id, @amount);";

                cmd.Parameters.Add(new NpgsqlParameter("@order_id", newProductInOrder.OrderID));
                cmd.Parameters.Add(new NpgsqlParameter("@product_id", newProductInOrder.ProductID));
                cmd.Parameters.Add(new NpgsqlParameter("@amount", newProductInOrder.Amount));

                cmd.ExecuteNonQuery();
            }
        }
        public void InsertNewReview(Review newReview)
        {
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO Review (Client_ID, Product_ID, Rating, Review_text, ReviewDate) " +
                                  "VALUES (@client_id, @product_id, @rating, @review_text, @review_date);";

                cmd.Parameters.Add(new NpgsqlParameter("@client_id", newReview.ClientID));
                cmd.Parameters.Add(new NpgsqlParameter("@product_id", newReview.ProductID));
                cmd.Parameters.Add(new NpgsqlParameter("@rating", newReview.Rating));
                cmd.Parameters.Add(new NpgsqlParameter("@review_text", newReview.ReviewText));
                cmd.Parameters.Add(new NpgsqlParameter("@review_date", newReview.ReviewDate));

                cmd.ExecuteNonQuery();
            }
        }
        public void InsertNewCategory(Category newCategory)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO category (name) " +
                              "Values (@name);";

            cmd.Parameters.Add(new NpgsqlParameter("@name", newCategory.Name));

            cmd.ExecuteNonQuery();
        }

    }

}
