namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedVerificationCodeTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.VerificationCodes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 30),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.Code, unique: true)
                .Index(t => t.User_Id);
            
            AlterColumn("dbo.IncomeCategories", "Name", c => c.String(maxLength: 30));
            AlterColumn("dbo.Incomes", "Name", c => c.String(maxLength: 30));
            AlterColumn("dbo.Lists", "Name", c => c.String(maxLength: 30));
            AlterColumn("dbo.ProductCategories", "Name", c => c.String(maxLength: 30));
            AlterColumn("dbo.Products", "Name", c => c.String(maxLength: 30));
            CreateIndex("dbo.IncomeCategories", "Name", unique: true);
            CreateIndex("dbo.Incomes", "Name", unique: true);
            CreateIndex("dbo.Lists", "Name", unique: true);
            CreateIndex("dbo.ProductCategories", "Name", unique: true);
            CreateIndex("dbo.Products", "Name", unique: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VerificationCodes", "User_Id", "dbo.Users");
            DropIndex("dbo.VerificationCodes", new[] { "User_Id" });
            DropIndex("dbo.VerificationCodes", new[] { "Code" });
            DropIndex("dbo.Products", new[] { "Name" });
            DropIndex("dbo.ProductCategories", new[] { "Name" });
            DropIndex("dbo.Lists", new[] { "Name" });
            DropIndex("dbo.Incomes", new[] { "Name" });
            DropIndex("dbo.IncomeCategories", new[] { "Name" });
            AlterColumn("dbo.Products", "Name", c => c.String());
            AlterColumn("dbo.ProductCategories", "Name", c => c.String());
            AlterColumn("dbo.Lists", "Name", c => c.String());
            AlterColumn("dbo.Incomes", "Name", c => c.String());
            AlterColumn("dbo.IncomeCategories", "Name", c => c.String());
            DropTable("dbo.VerificationCodes");
        }
    }
}
