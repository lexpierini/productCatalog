using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace productCatalog.Migrations
{
    public partial class TableDataProducts : Migration
    {
        protected override void Up(MigrationBuilder mb)
        {
            mb.Sql("INSERT INTO public.\"Products\"(\"Name\", \"Description\", \"Price\", \"ImageUrl\", \"Inventory\", \"RegistrationDate\", \"CategoryId\") " +
                   "VALUES ('Coca-Cola Diet', 'Cola soft drink 350ml', 5.45, 'cocacola.jpg', 50, now(), 2), " +
                          "('Tuna Sandwich', 'Tuna sandwich with mayonnaise', 8.50, 'tuna.jpg', 10, now(), 3), " +
                          "('Pudim', 'Condensed milk pudding 100g', 6.75, 'pudim.jpg', 20, now(), 4)");
        }

        protected override void Down(MigrationBuilder mb)
        {
            mb.Sql("Delete from Products");
        }
    }
}
