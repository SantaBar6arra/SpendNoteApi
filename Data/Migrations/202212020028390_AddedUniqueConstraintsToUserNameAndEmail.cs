namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedUniqueConstraintsToUserNameAndEmail : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "Name", c => c.String(maxLength: 30));
            AlterColumn("dbo.Users", "Email", c => c.String(maxLength: 50));
            CreateIndex("dbo.Users", "Name", unique: true);
            CreateIndex("dbo.Users", "Email", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Users", new[] { "Email" });
            DropIndex("dbo.Users", new[] { "Name" });
            AlterColumn("dbo.Users", "Email", c => c.String());
            AlterColumn("dbo.Users", "Name", c => c.String());
        }
    }
}
