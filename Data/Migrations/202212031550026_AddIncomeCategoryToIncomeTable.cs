namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIncomeCategoryToIncomeTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Incomes", "User_Id", c => c.Int());
            CreateIndex("dbo.Incomes", "User_Id");
            AddForeignKey("dbo.Incomes", "User_Id", "dbo.Users", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Incomes", "User_Id", "dbo.Users");
            DropIndex("dbo.Incomes", new[] { "User_Id" });
            DropColumn("dbo.Incomes", "User_Id");
        }
    }
}
