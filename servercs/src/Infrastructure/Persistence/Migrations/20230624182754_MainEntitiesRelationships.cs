using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Serenity.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MainEntitiesRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activity_JournalEntry_JournalEntryId",
                table: "Activity");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_UserStats_StatsId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Feeling_JournalEntry_JournalEntryId",
                table: "Feeling");

            migrationBuilder.DropForeignKey(
                name: "FK_Illness_AspNetUsers_ApplicationUserId",
                table: "Illness");

            migrationBuilder.DropForeignKey(
                name: "FK_JournalEntry_AspNetUsers_ApplicationUserId",
                table: "JournalEntry");

            migrationBuilder.DropIndex(
                name: "IX_Illness_ApplicationUserId",
                table: "Illness");

            migrationBuilder.DropIndex(
                name: "IX_Feeling_JournalEntryId",
                table: "Feeling");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_StatsId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_Activity_JournalEntryId",
                table: "Activity");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Illness");

            migrationBuilder.DropColumn(
                name: "JournalEntryId",
                table: "Feeling");

            migrationBuilder.DropColumn(
                name: "StatsId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "JournalEntryId",
                table: "Activity");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserId",
                table: "JournalEntry",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_JournalEntry_ApplicationUserId",
                table: "JournalEntry",
                newName: "IX_JournalEntry_UserId");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "UserStats",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "ActivityJournalEntry",
                columns: table => new
                {
                    ActivitiesId = table.Column<Guid>(type: "uuid", nullable: false),
                    JournalEntriesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityJournalEntry", x => new { x.ActivitiesId, x.JournalEntriesId });
                    table.ForeignKey(
                        name: "FK_ActivityJournalEntry_Activity_ActivitiesId",
                        column: x => x.ActivitiesId,
                        principalTable: "Activity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActivityJournalEntry_JournalEntry_JournalEntriesId",
                        column: x => x.JournalEntriesId,
                        principalTable: "JournalEntry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationUserIllness",
                columns: table => new
                {
                    IllnessesId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsersId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserIllness", x => new { x.IllnessesId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserIllness_AspNetUsers_UsersId",
                        column: x => x.UsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserIllness_Illness_IllnessesId",
                        column: x => x.IllnessesId,
                        principalTable: "Illness",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FeelingJournalEntry",
                columns: table => new
                {
                    FeelingsId = table.Column<Guid>(type: "uuid", nullable: false),
                    JournalEntriesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeelingJournalEntry", x => new { x.FeelingsId, x.JournalEntriesId });
                    table.ForeignKey(
                        name: "FK_FeelingJournalEntry_Feeling_FeelingsId",
                        column: x => x.FeelingsId,
                        principalTable: "Feeling",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FeelingJournalEntry_JournalEntry_JournalEntriesId",
                        column: x => x.JournalEntriesId,
                        principalTable: "JournalEntry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserStats_UserId",
                table: "UserStats",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActivityJournalEntry_JournalEntriesId",
                table: "ActivityJournalEntry",
                column: "JournalEntriesId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserIllness_UsersId",
                table: "ApplicationUserIllness",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_FeelingJournalEntry_JournalEntriesId",
                table: "FeelingJournalEntry",
                column: "JournalEntriesId");

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntry_AspNetUsers_UserId",
                table: "JournalEntry",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserStats_AspNetUsers_UserId",
                table: "UserStats",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JournalEntry_AspNetUsers_UserId",
                table: "JournalEntry");

            migrationBuilder.DropForeignKey(
                name: "FK_UserStats_AspNetUsers_UserId",
                table: "UserStats");

            migrationBuilder.DropTable(
                name: "ActivityJournalEntry");

            migrationBuilder.DropTable(
                name: "ApplicationUserIllness");

            migrationBuilder.DropTable(
                name: "FeelingJournalEntry");

            migrationBuilder.DropIndex(
                name: "IX_UserStats_UserId",
                table: "UserStats");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserStats");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "JournalEntry",
                newName: "ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_JournalEntry_UserId",
                table: "JournalEntry",
                newName: "IX_JournalEntry_ApplicationUserId");

            migrationBuilder.AddColumn<Guid>(
                name: "ApplicationUserId",
                table: "Illness",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "JournalEntryId",
                table: "Feeling",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StatsId",
                table: "AspNetUsers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "JournalEntryId",
                table: "Activity",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Illness_ApplicationUserId",
                table: "Illness",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Feeling_JournalEntryId",
                table: "Feeling",
                column: "JournalEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_StatsId",
                table: "AspNetUsers",
                column: "StatsId");

            migrationBuilder.CreateIndex(
                name: "IX_Activity_JournalEntryId",
                table: "Activity",
                column: "JournalEntryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Activity_JournalEntry_JournalEntryId",
                table: "Activity",
                column: "JournalEntryId",
                principalTable: "JournalEntry",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_UserStats_StatsId",
                table: "AspNetUsers",
                column: "StatsId",
                principalTable: "UserStats",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Feeling_JournalEntry_JournalEntryId",
                table: "Feeling",
                column: "JournalEntryId",
                principalTable: "JournalEntry",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Illness_AspNetUsers_ApplicationUserId",
                table: "Illness",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntry_AspNetUsers_ApplicationUserId",
                table: "JournalEntry",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
