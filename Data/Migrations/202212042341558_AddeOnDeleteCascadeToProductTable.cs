namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddeOnDeleteCascadeToProductTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Products", "List_Id", "dbo.Lists");
            AddForeignKey("dbo.Products", "List_Id", "dbo.Lists", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Products", "List_Id", "dbo.Lists");
            AddForeignKey("dbo.Products", "List_Id", "dbo.Lists", "Id");
        }
    }
}
