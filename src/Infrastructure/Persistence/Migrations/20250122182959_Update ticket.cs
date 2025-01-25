using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Updateticket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseHistory_Movie_MovieId",
                table: "purchase_histories");

            migrationBuilder.DropPrimaryKey(
                name: "pk_tickets",
                table: "tickets");

            migrationBuilder.RenameColumn(
                name: "movie_id",
                table: "purchase_histories",
                newName: "ticket_id");

            migrationBuilder.RenameIndex(
                name: "ix_purchase_histories_movie_id",
                table: "purchase_histories",
                newName: "ix_purchase_histories_ticket_id");

            migrationBuilder.AddColumn<Guid>(
                name: "id",
                table: "tickets",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "pk_tickets",
                table: "tickets",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_tickets_seat_id",
                table: "tickets",
                column: "seat_id");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseHistory_Ticket_TicketId",
                table: "purchase_histories",
                column: "ticket_id",
                principalTable: "tickets",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseHistory_Ticket_TicketId",
                table: "purchase_histories");

            migrationBuilder.DropPrimaryKey(
                name: "pk_tickets",
                table: "tickets");

            migrationBuilder.DropIndex(
                name: "ix_tickets_seat_id",
                table: "tickets");

            migrationBuilder.DropColumn(
                name: "id",
                table: "tickets");

            migrationBuilder.RenameColumn(
                name: "ticket_id",
                table: "purchase_histories",
                newName: "movie_id");

            migrationBuilder.RenameIndex(
                name: "ix_purchase_histories_ticket_id",
                table: "purchase_histories",
                newName: "ix_purchase_histories_movie_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_tickets",
                table: "tickets",
                columns: new[] { "seat_id", "session_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseHistory_Movie_MovieId",
                table: "purchase_histories",
                column: "movie_id",
                principalTable: "movies",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
