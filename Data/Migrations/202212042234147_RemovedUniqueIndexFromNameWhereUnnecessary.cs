namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedUniqueIndexFromNameWhereUnnecessary : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.IncomeCategories", new[] { "Name" });
            DropIndex("dbo.Incomes", new[] { "Name" });
            DropIndex("dbo.Lists", new[] { "Name" });
            DropIndex("dbo.ProductCategories", new[] { "Name" });
            DropIndex("dbo.Products", new[] { "Name" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.Products", "Name", unique: true);
            CreateIndex("dbo.ProductCategories", "Name", unique: true);
            CreateIndex("dbo.Lists", "Name", unique: true);
            CreateIndex("dbo.Incomes", "Name", unique: true);
            CreateIndex("dbo.IncomeCategories", "Name", unique: true);
        }
    }
}
