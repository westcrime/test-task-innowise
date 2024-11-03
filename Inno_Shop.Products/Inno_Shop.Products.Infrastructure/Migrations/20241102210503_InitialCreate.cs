using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inno_Shop.Products.Infrastructure.Migrations
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
                    IF NOT EXISTS (SELECT 1 FROM pg_tables WHERE tablename = 'Products') THEN
                        CREATE TABLE ""Products"" (
                            ""Id"" uuid NOT NULL,
                            ""Name"" text NOT NULL,
                            ""Description"" text NOT NULL,
                            ""Cost"" double precision NOT NULL,
                            ""UserId"" uuid NOT NULL,
                            CONSTRAINT ""PK_Products"" PRIMARY KEY (""Id"")
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
                name: "Products");
        }
    }
}
