using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseFillingHelper
{
 

    public class Client
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? PhoneNumber { get; set; }

        public override string ToString()
        {
            return $"{FirstName} {LastName} {Email} {Password} {PhoneNumber}";
        }
    }

    public class AnOrder
    {
        public int ID { get; set; }
        public int ClientID { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
    }

    public class ProductInOrder
    {

        public int ID { get; set; }
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public int Amount { get; set; }
    }

    public class Product
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int CategoryID { get; set; }
        public decimal Price { get; set; }
    }

    public class Category
    {
       
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class Information
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public int ProductID { get; set; }
    }

    public class Review
    {
        public int ID { get; set; }
        public int ClientID { get; set; }
        public int ProductID { get; set; }
        public DateTime ReviewDate { get; set; }
        public int Rating { get; set; }
        public string ReviewText { get; set; }
    }

}
