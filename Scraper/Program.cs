using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Scraper
{
    class Program
    {
        static void Main(string[] args)
        {
            UpdateDatabase();
        }

        static void UpdateDatabase()
        {
            using (var db = new ItemContext())
            {
                Console.WriteLine("Fetching data...");
                List<Item> items = Scraper.FetchItemsKaufland();

                // Clear the old data first
                Console.WriteLine("Clearing the old data...");
                db.Database.ExecuteSqlCommand("DELETE FROM Items");

                Console.WriteLine("Adding data to the database...");
                db.Items.AddRange(items);
                db.SaveChanges();
            }
        }
    }
}
