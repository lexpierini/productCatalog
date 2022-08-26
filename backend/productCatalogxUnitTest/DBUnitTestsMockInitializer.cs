using productCatalog.Context;
using productCatalog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace productCatalogxUnitTest
{
    public  class DBUnitTestsMockInitializer
    {
        public DBUnitTestsMockInitializer() { }

        public void Seed(AppDbContext context)
        {
            context.Categories.Add
                (new Category { CategoryId = 999, Name = "Drink999", ImageUrl = "drink999.jpg" });

            context.Categories.Add
                (new Category { CategoryId = 2, Name = "Juice", ImageUrl = "juice2.jpg" });

            context.Categories.Add
                (new Category { CategoryId = 3, Name = "Candy777", ImageUrl = "candy3.jpg" });

            context.Categories.Add
                (new Category { CategoryId = 4, Name = "Snacks", ImageUrl = "snacks4.jpg" });

            context.Categories.Add
                (new Category { CategoryId = 5, Name = "Pie", ImageUrl = "pie5.jpg" });

            context.Categories.Add
                (new Category { CategoryId = 6, Name = "Cake", ImageUrl = "cake6.jpg" });

            context.SaveChanges();

        }
    }
}
