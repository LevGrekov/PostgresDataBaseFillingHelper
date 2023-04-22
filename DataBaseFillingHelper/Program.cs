using Npgsql;
using System.Diagnostics;
using System.Text;

namespace DataBaseFillingHelper
{

    public static class Program
    {
        public static void ReGenerateDB()
        {

            var connString = "Host=localhost;Username=postgres;Password=19374562";
            using (var connection = new NpgsqlConnection(connString))
            {
                try
                {
                    connection.Open();

                    // Остановить все соединения с базой данных.
                    var stopConnectionsCmd = connection.CreateCommand();
                    stopConnectionsCmd.CommandText = "SELECT pg_terminate_backend(pid) FROM pg_stat_activity WHERE datname = 'onlineshopdb';";
                    stopConnectionsCmd.ExecuteNonQuery();
                    Console.WriteLine("Соединения c БД уничтожены");

                    // Удалить существующую базу данных.
                    var dropDatabaseCmd = connection.CreateCommand();
                    dropDatabaseCmd.CommandText = "DROP DATABASE IF EXISTS onlineshopdb;";
                    dropDatabaseCmd.ExecuteNonQuery();
                    Console.WriteLine("DROP DATABASE IF EXISTS onlineshopdb; --> DONE");

                    // Создать новую базу данных.
                    var createDatabaseCmd = connection.CreateCommand();
                    createDatabaseCmd.CommandText = "CREATE DATABASE onlineshopdb;";
                    createDatabaseCmd.ExecuteNonQuery();
                    Console.WriteLine("CREATE DATABASE onlineshopdb; --> DONE");
                    Console.WriteLine();
                }
                catch(Exception ex)
                {
                    connection.Close();
                    Console.WriteLine(ex.Message);
                    throw ex;
                }

            }
        }
        private static void Main()
        {
            Console.WriteLine("NOTICE: Подключение к БД Postgres осуществлено через:");
            Console.WriteLine("server = \"localhost\", port = \"5432\", database = \"onlineshopdb\", userID = \"postgres\", password = \"19374562\"");
            Console.WriteLine("\nДля продолжения нажмите любую кнопку");
            Console.WriteLine("WARRNING: Все данные в БД удалятся, она будет создана заново...");
            Console.ReadKey();

            ReGenerateDB();

            var dbHelper = DBHelper.getInstance("localhost", "5432", "onlineshopdb", "postgres", "19374562");
            Console.WriteLine(dbHelper.TestConnect());
            
            dbHelper.CreateDataBase();

            Console.WriteLine();
            Console.WriteLine("База Данных полностью обнулена, создана заного и готова к заполнению:");
            int clients = 0;
            while (clients <= 0)
            {
                Console.WriteLine("Введите количество желаемых клиентов:");
                string input = Console.ReadLine();
                if (int.TryParse(input, out clients) && clients > 0)
                {
                    break;
                }
                Console.WriteLine("Ошибка: Введенное значение не является положительным целым числом.");
            }

            int orders = 0;
            while (orders <= 0)
            {
                Console.WriteLine("Введите количество желаемых заказов:");
                string input = Console.ReadLine();
                if (int.TryParse(input, out orders) && orders > 0)
                {
                    break;
                }
                Console.WriteLine("Ошибка: Введенное значение не является положительным целым числом.");
            }

            int productsInOrders = 0;
            while (productsInOrders <= 0)
            {
                Console.WriteLine("Введите количество продутов которые будут распределены между заказами:");
                string input = Console.ReadLine();
                if (int.TryParse(input, out productsInOrders) && productsInOrders > 0)
                {
                    break;
                }
                Console.WriteLine("Ошибка: Введенное значение не является положительным целым числом.");
            }

            int reviews = 0;
            while (reviews <= 0)
            {
                Console.WriteLine("Введите количество отзывов от покупателей:");
                string input = Console.ReadLine();
                if (int.TryParse(input, out reviews) && reviews > 0)
                {
                    break;
                }
                Console.WriteLine("Ошибка: Введенное значение не является положительным целым числом.");
            }

            Console.WriteLine("\nЗаполнение началось...\n");
            var a = new Stopwatch();
            EntitiesGenerator EG = new EntitiesGenerator(clients,orders,productsInOrders, reviews);
            a.Start();
            dbHelper.FillDataBase(EG);
            a.Stop();
            Console.WriteLine($"Заполнение Завершено... за {a.Elapsed}  \n") ;
        }
    }
   
}
