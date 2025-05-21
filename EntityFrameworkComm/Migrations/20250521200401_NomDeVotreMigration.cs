using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFrameworkComm.Migrations
{
    /// <inheritdoc />
    public partial class NomDeVotreMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    IdUser = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EntraIdUser = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.IdUser);
                });

            migrationBuilder.CreateTable(
                name: "Vault",
                columns: table => new
                {
                    IdVault = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    VaultName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    KeyHash = table.Column<byte[]>(type: "BLOB", maxLength: 255, nullable: false),
                    Salt = table.Column<byte[]>(type: "BLOB", maxLength: 255, nullable: false),
                    PrivateKey = table.Column<byte[]>(type: "BLOB", nullable: false),
                    IsDesactivated = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vault", x => x.IdVault);
                });

            migrationBuilder.CreateTable(
                name: "Entrie",
                columns: table => new
                {
                    IdEntrie = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VaultId = table.Column<int>(type: "INTEGER", nullable: false),
                    NameDataId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserNameDataId = table.Column<int>(type: "INTEGER", nullable: false),
                    PasswordDataId = table.Column<int>(type: "INTEGER", nullable: false),
                    UrlDataId = table.Column<int>(type: "INTEGER", nullable: false),
                    CommentDataId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDesactivated = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entrie", x => x.IdEntrie);
                    table.ForeignKey(
                        name: "FK_Entrie_Vault_VaultId",
                        column: x => x.VaultId,
                        principalTable: "Vault",
                        principalColumn: "IdVault",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserVault",
                columns: table => new
                {
                    UsersIdUser = table.Column<int>(type: "INTEGER", nullable: false),
                    VaultsIdVault = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserVault", x => new { x.UsersIdUser, x.VaultsIdVault });
                    table.ForeignKey(
                        name: "FK_UserVault_User_UsersIdUser",
                        column: x => x.UsersIdUser,
                        principalTable: "User",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserVault_Vault_VaultsIdVault",
                        column: x => x.VaultsIdVault,
                        principalTable: "Vault",
                        principalColumn: "IdVault",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EncryptedData",
                columns: table => new
                {
                    IdEncryptedData = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EntrieId = table.Column<int>(type: "INTEGER", nullable: false),
                    Iv = table.Column<byte[]>(type: "BLOB", nullable: false),
                    CryptedData = table.Column<byte[]>(type: "BLOB", nullable: false),
                    Tag = table.Column<byte[]>(type: "BLOB", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EncryptedData", x => x.IdEncryptedData);
                    table.ForeignKey(
                        name: "FK_EncryptedData_Entrie_EntrieId",
                        column: x => x.EntrieId,
                        principalTable: "Entrie",
                        principalColumn: "IdEntrie",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Log",
                columns: table => new
                {
                    IdLog = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ActionDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ActionType = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Details = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    VaultId = table.Column<int>(type: "INTEGER", nullable: true),
                    EntryId = table.Column<int>(type: "INTEGER", nullable: true),
                    DataId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log", x => x.IdLog);
                    table.ForeignKey(
                        name: "FK_Log_EncryptedData_DataId",
                        column: x => x.DataId,
                        principalTable: "EncryptedData",
                        principalColumn: "IdEncryptedData");
                    table.ForeignKey(
                        name: "FK_Log_Entrie_EntryId",
                        column: x => x.EntryId,
                        principalTable: "Entrie",
                        principalColumn: "IdEntrie");
                    table.ForeignKey(
                        name: "FK_Log_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Log_Vault_VaultId",
                        column: x => x.VaultId,
                        principalTable: "Vault",
                        principalColumn: "IdVault");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EncryptedData_EntrieId",
                table: "EncryptedData",
                column: "EntrieId");

            migrationBuilder.CreateIndex(
                name: "IX_Entrie_VaultId",
                table: "Entrie",
                column: "VaultId");

            migrationBuilder.CreateIndex(
                name: "IX_Log_DataId",
                table: "Log",
                column: "DataId");

            migrationBuilder.CreateIndex(
                name: "IX_Log_EntryId",
                table: "Log",
                column: "EntryId");

            migrationBuilder.CreateIndex(
                name: "IX_Log_UserId",
                table: "Log",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Log_VaultId",
                table: "Log",
                column: "VaultId");

            migrationBuilder.CreateIndex(
                name: "IX_UserVault_VaultsIdVault",
                table: "UserVault",
                column: "VaultsIdVault");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Log");

            migrationBuilder.DropTable(
                name: "UserVault");

            migrationBuilder.DropTable(
                name: "EncryptedData");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Entrie");

            migrationBuilder.DropTable(
                name: "Vault");
        }
    }
}
