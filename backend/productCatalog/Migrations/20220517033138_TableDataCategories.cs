using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace productCatalog.Migrations
{
    public partial class TableDataCategories : Migration
    {
        protected override void Up(MigrationBuilder mb)
        {
            mb.Sql("INSERT INTO public.\"Categories\"(\"Name\", \"ImageUrl\") " +
                   "Values ('Drinks', 'drinks.jpg'), " +
                          "('Sandwiches', 'sandwiches.jpg'), " +
                          "('Desserts', 'desserts.jpg')");            
        }

        protected override void Down(MigrationBuilder mb)
        {
            mb.Sql("Delete from Categories");
        }
    }
}
