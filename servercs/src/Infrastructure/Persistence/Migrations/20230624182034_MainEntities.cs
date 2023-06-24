using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace Serenity.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MainEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "AspNetUserTokens",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "AspNetUsers",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BackgroundUrl",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordLock",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PersonDetails_Gender",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PersonDetails_Pronouns",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StatsId",
                table: "AspNetUsers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "RoleId",
                table: "AspNetUserRoles",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "AspNetUserRoles",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "AspNetUserLogins",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "AspNetUserClaims",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "AspNetRoles",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "RoleId",
                table: "AspNetRoleClaims",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "Illness",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    ApplicationUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<Instant>(type: "timestamp with time zone", nullable: false, defaultValue: NodaTime.Instant.FromUnixTimeTicks(0L)),
                    CreationAt = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<Instant>(type: "timestamp with time zone", nullable: false, defaultValue: NodaTime.Instant.FromUnixTimeTicks(0L))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Illness", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Illness_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Mood",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<Instant>(type: "timestamp with time zone", nullable: false, defaultValue: NodaTime.Instant.FromUnixTimeTicks(0L)),
                    CreationAt = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<Instant>(type: "timestamp with time zone", nullable: false, defaultValue: NodaTime.Instant.FromUnixTimeTicks(0L))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mood", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserStats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LastBreathingCheckup = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<Instant>(type: "timestamp with time zone", nullable: false, defaultValue: NodaTime.Instant.FromUnixTimeTicks(0L)),
                    CreationAt = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<Instant>(type: "timestamp with time zone", nullable: false, defaultValue: NodaTime.Instant.FromUnixTimeTicks(0L))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JournalEntry",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QuickNote = table.Column<string>(type: "text", nullable: true),
                    MoodId = table.Column<Guid>(type: "uuid", nullable: true),
                    ApplicationUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<Instant>(type: "timestamp with time zone", nullable: false, defaultValue: NodaTime.Instant.FromUnixTimeTicks(0L)),
                    CreationAt = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<Instant>(type: "timestamp with time zone", nullable: false, defaultValue: NodaTime.Instant.FromUnixTimeTicks(0L))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JournalEntry_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_JournalEntry_Mood_MoodId",
                        column: x => x.MoodId,
                        principalTable: "Mood",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Activity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<Instant>(type: "timestamp with time zone", nullable: false, defaultValue: NodaTime.Instant.FromUnixTimeTicks(0L)),
                    JournalEntryId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreationAt = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<Instant>(type: "timestamp with time zone", nullable: false, defaultValue: NodaTime.Instant.FromUnixTimeTicks(0L))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Activity_JournalEntry_JournalEntryId",
                        column: x => x.JournalEntryId,
                        principalTable: "JournalEntry",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Feeling",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<Instant>(type: "timestamp with time zone", nullable: false, defaultValue: NodaTime.Instant.FromUnixTimeTicks(0L)),
                    JournalEntryId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreationAt = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<Instant>(type: "timestamp with time zone", nullable: false, defaultValue: NodaTime.Instant.FromUnixTimeTicks(0L))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feeling", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Feeling_JournalEntry_JournalEntryId",
                        column: x => x.JournalEntryId,
                        principalTable: "JournalEntry",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_StatsId",
                table: "AspNetUsers",
                column: "StatsId");

            migrationBuilder.CreateIndex(
                name: "IX_Activity_JournalEntryId",
                table: "Activity",
                column: "JournalEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_Feeling_JournalEntryId",
                table: "Feeling",
                column: "JournalEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_Illness_ApplicationUserId",
                table: "Illness",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntry_ApplicationUserId",
                table: "JournalEntry",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntry_MoodId",
                table: "JournalEntry",
                column: "MoodId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_UserStats_StatsId",
                table: "AspNetUsers",
                column: "StatsId",
                principalTable: "UserStats",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_UserStats_StatsId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Activity");

            migrationBuilder.DropTable(
                name: "Feeling");

            migrationBuilder.DropTable(
                name: "Illness");

            migrationBuilder.DropTable(
                name: "UserStats");

            migrationBuilder.DropTable(
                name: "JournalEntry");

            migrationBuilder.DropTable(
                name: "Mood");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_StatsId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "BackgroundUrl",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PasswordLock",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PersonDetails_Gender",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PersonDetails_Pronouns",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "StatsId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AspNetUserTokens",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "RoleId",
                table: "AspNetUserRoles",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AspNetUserRoles",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AspNetUserLogins",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AspNetUserClaims",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "AspNetRoles",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "RoleId",
                table: "AspNetRoleClaims",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");
        }
    }
}
