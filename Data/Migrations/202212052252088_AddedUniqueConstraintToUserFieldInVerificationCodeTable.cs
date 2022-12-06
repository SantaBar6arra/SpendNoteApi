namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedUniqueConstraintToUserFieldInVerificationCodeTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.VerificationCodes", "User_Id", "dbo.Users");
            DropIndex("dbo.VerificationCodes", new[] { "User_Id" });
            AlterColumn("dbo.VerificationCodes", "User_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.VerificationCodes", "User_Id", unique: true);
            AddForeignKey("dbo.VerificationCodes", "User_Id", "dbo.Users", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VerificationCodes", "User_Id", "dbo.Users");
            DropIndex("dbo.VerificationCodes", new[] { "User_Id" });
            AlterColumn("dbo.VerificationCodes", "User_Id", c => c.Int());
            CreateIndex("dbo.VerificationCodes", "User_Id");
            AddForeignKey("dbo.VerificationCodes", "User_Id", "dbo.Users", "Id");
        }
    }
}
