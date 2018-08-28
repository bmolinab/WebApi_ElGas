namespace WebApi_ElGas.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ApplicationDbContext : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "CodeRecovery", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "CodeRecovery");
        }
    }
}
