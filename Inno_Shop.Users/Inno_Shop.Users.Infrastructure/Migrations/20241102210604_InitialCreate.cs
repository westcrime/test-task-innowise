using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inno_Shop.Users.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM pg_tables WHERE tablename = 'Users') THEN
                        CREATE TABLE ""Users"" (
                            ""Id"" uuid NOT NULL,
                            ""Name"" text NOT NULL,
                            ""Email"" text NOT NULL,
                            ""Password"" text NOT NULL,
                            ""Role"" integer NOT NULL,
                            ""EmailToken"" text NOT NULL,
                            ""IsVerified"" boolean NOT NULL,
                            CONSTRAINT ""PK_Users"" PRIMARY KEY (""Id"")
                        );
                    END IF;
                END
                $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
