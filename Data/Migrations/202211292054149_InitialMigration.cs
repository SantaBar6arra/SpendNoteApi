namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.IncomeCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        PasswordHash = c.String(),
                        Email = c.String(),
                        Salt = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Incomes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AddTime = c.DateTime(nullable: false),
                        ExpirationTime = c.DateTime(),
                        Category_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.IncomeCategories", t => t.Category_Id)
                .Index(t => t.Category_Id);
            
            CreateTable(
                "dbo.Lists",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.ProductCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsBought = c.Boolean(nullable: false),
                        AddDate = c.DateTime(nullable: false),
                        PurchaseDate = c.DateTime(),
                        BuyUntilDate = c.DateTime(),
                        List_Id = c.Int(),
                        ProductCategory_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Lists", t => t.List_Id)
                .ForeignKey("dbo.ProductCategories", t => t.ProductCategory_Id)
                .Index(t => t.List_Id)
                .Index(t => t.ProductCategory_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Products", "ProductCategory_Id", "dbo.ProductCategories");
            DropForeignKey("dbo.Products", "List_Id", "dbo.Lists");
            DropForeignKey("dbo.ProductCategories", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Lists", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Incomes", "Category_Id", "dbo.IncomeCategories");
            DropForeignKey("dbo.IncomeCategories", "User_Id", "dbo.Users");
            DropIndex("dbo.Products", new[] { "ProductCategory_Id" });
            DropIndex("dbo.Products", new[] { "List_Id" });
            DropIndex("dbo.ProductCategories", new[] { "User_Id" });
            DropIndex("dbo.Lists", new[] { "User_Id" });
            DropIndex("dbo.Incomes", new[] { "Category_Id" });
            DropIndex("dbo.IncomeCategories", new[] { "User_Id" });
            DropTable("dbo.Products");
            DropTable("dbo.ProductCategories");
            DropTable("dbo.Lists");
            DropTable("dbo.Incomes");
            DropTable("dbo.Users");
            DropTable("dbo.IncomeCategories");
        }
    }
}
