namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenamedSomeColumnsInProductsTable : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Products", name: "ProductCategory_Id", newName: "Category_Id");
            RenameIndex(table: "dbo.Products", name: "IX_ProductCategory_Id", newName: "IX_Category_Id");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Products", name: "IX_Category_Id", newName: "IX_ProductCategory_Id");
            RenameColumn(table: "dbo.Products", name: "Category_Id", newName: "ProductCategory_Id");
        }
    }
}
