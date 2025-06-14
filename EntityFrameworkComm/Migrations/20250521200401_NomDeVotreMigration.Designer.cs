﻿// <auto-generated />
using System;
using EntityFrameworkComm.EfModel.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EntityFrameworkComm.Migrations
{
    [DbContext(typeof(Context))]
    [Migration("20250521200401_NomDeVotreMigration")]
    partial class NomDeVotreMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.2");

            modelBuilder.Entity("EntityFrameworkComm.EfModel.Models.EncryptedData", b =>
                {
                    b.Property<int>("IdEncryptedData")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("IdEncryptedData");

                    b.Property<byte[]>("CryptedData")
                        .IsRequired()
                        .HasColumnType("BLOB");

                    b.Property<int>("EntrieId")
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("Iv")
                        .IsRequired()
                        .HasColumnType("BLOB");

                    b.Property<byte[]>("Tag")
                        .IsRequired()
                        .HasColumnType("BLOB");

                    b.HasKey("IdEncryptedData");

                    b.HasIndex("EntrieId");

                    b.ToTable("EncryptedData");
                });

            modelBuilder.Entity("EntityFrameworkComm.EfModel.Models.Entrie", b =>
                {
                    b.Property<int>("IdEntrie")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("IdEntrie");

                    b.Property<int>("CommentDataId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDesactivated")
                        .HasColumnType("INTEGER");

                    b.Property<int>("NameDataId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PasswordDataId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("UrlDataId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserNameDataId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("VaultId")
                        .HasColumnType("INTEGER");

                    b.HasKey("IdEntrie");

                    b.HasIndex("VaultId");

                    b.ToTable("Entrie");
                });

            modelBuilder.Entity("EntityFrameworkComm.EfModel.Models.Log", b =>
                {
                    b.Property<int>("IdLog")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("IdLog");

                    b.Property<DateTime>("ActionDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("ActionType")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int?>("DataId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Details")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int?>("EntryId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("VaultId")
                        .HasColumnType("INTEGER");

                    b.HasKey("IdLog");

                    b.HasIndex("DataId");

                    b.HasIndex("EntryId");

                    b.HasIndex("UserId");

                    b.HasIndex("VaultId");

                    b.ToTable("Log");
                });

            modelBuilder.Entity("EntityFrameworkComm.EfModel.Models.User", b =>
                {
                    b.Property<int>("IdUser")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("IdUser");

                    b.Property<Guid>("EntraIdUser")
                        .HasColumnType("TEXT");

                    b.HasKey("IdUser");

                    b.ToTable("User");
                });

            modelBuilder.Entity("EntityFrameworkComm.EfModel.Models.Vault", b =>
                {
                    b.Property<int>("IdVault")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("IdVault");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDesactivated")
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("KeyHash")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("BLOB");

                    b.Property<byte[]>("PrivateKey")
                        .IsRequired()
                        .HasColumnType("BLOB");

                    b.Property<byte[]>("Salt")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("BLOB");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("VaultName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.HasKey("IdVault");

                    b.ToTable("Vault");
                });

            modelBuilder.Entity("UserVault", b =>
                {
                    b.Property<int>("UsersIdUser")
                        .HasColumnType("INTEGER");

                    b.Property<int>("VaultsIdVault")
                        .HasColumnType("INTEGER");

                    b.HasKey("UsersIdUser", "VaultsIdVault");

                    b.HasIndex("VaultsIdVault");

                    b.ToTable("UserVault");
                });

            modelBuilder.Entity("EntityFrameworkComm.EfModel.Models.EncryptedData", b =>
                {
                    b.HasOne("EntityFrameworkComm.EfModel.Models.Entrie", "Entrie")
                        .WithMany("EncryptedData")
                        .HasForeignKey("EntrieId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Entrie");
                });

            modelBuilder.Entity("EntityFrameworkComm.EfModel.Models.Entrie", b =>
                {
                    b.HasOne("EntityFrameworkComm.EfModel.Models.Vault", "Vault")
                        .WithMany("Entries")
                        .HasForeignKey("VaultId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Vault");
                });

            modelBuilder.Entity("EntityFrameworkComm.EfModel.Models.Log", b =>
                {
                    b.HasOne("EntityFrameworkComm.EfModel.Models.EncryptedData", "EncryptedData")
                        .WithMany("Logs")
                        .HasForeignKey("DataId");

                    b.HasOne("EntityFrameworkComm.EfModel.Models.Entrie", "Entrie")
                        .WithMany("Logs")
                        .HasForeignKey("EntryId");

                    b.HasOne("EntityFrameworkComm.EfModel.Models.User", "User")
                        .WithMany("Logs")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EntityFrameworkComm.EfModel.Models.Vault", "Vault")
                        .WithMany("Logs")
                        .HasForeignKey("VaultId");

                    b.Navigation("EncryptedData");

                    b.Navigation("Entrie");

                    b.Navigation("User");

                    b.Navigation("Vault");
                });

            modelBuilder.Entity("UserVault", b =>
                {
                    b.HasOne("EntityFrameworkComm.EfModel.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UsersIdUser")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EntityFrameworkComm.EfModel.Models.Vault", null)
                        .WithMany()
                        .HasForeignKey("VaultsIdVault")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("EntityFrameworkComm.EfModel.Models.EncryptedData", b =>
                {
                    b.Navigation("Logs");
                });

            modelBuilder.Entity("EntityFrameworkComm.EfModel.Models.Entrie", b =>
                {
                    b.Navigation("EncryptedData");

                    b.Navigation("Logs");
                });

            modelBuilder.Entity("EntityFrameworkComm.EfModel.Models.User", b =>
                {
                    b.Navigation("Logs");
                });

            modelBuilder.Entity("EntityFrameworkComm.EfModel.Models.Vault", b =>
                {
                    b.Navigation("Entries");

                    b.Navigation("Logs");
                });
#pragma warning restore 612, 618
        }
    }
}
