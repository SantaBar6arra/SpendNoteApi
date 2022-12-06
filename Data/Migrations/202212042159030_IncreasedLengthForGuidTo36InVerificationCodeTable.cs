namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IncreasedLengthForGuidTo36InVerificationCodeTable : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.VerificationCodes", new[] { "Code" });
            AlterColumn("dbo.VerificationCodes", "Code", c => c.String(maxLength: 36));
            CreateIndex("dbo.VerificationCodes", "Code", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.VerificationCodes", new[] { "Code" });
            AlterColumn("dbo.VerificationCodes", "Code", c => c.String(maxLength: 30));
            CreateIndex("dbo.VerificationCodes", "Code", unique: true);
        }
    }
}
