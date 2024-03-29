﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Lineage2.Database.Migrations
{
    [DbContext(typeof(Lineage2DbContext))]
    partial class Lineage2DbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("Lineage2.Repositories.Spawn", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("Heading")
                        .HasColumnType("int");

                    b.Property<int>("LocX")
                        .HasColumnType("int");

                    b.Property<int>("LocY")
                        .HasColumnType("int");

                    b.Property<int>("LocZ")
                        .HasColumnType("int");

                    b.Property<int>("PeriodOfDay")
                        .HasColumnType("int");

                    b.Property<int>("RespawnDelay")
                        .HasColumnType("int");

                    b.Property<int>("RespawnRand")
                        .HasColumnType("int");

                    b.Property<int>("SpanwnTemplateId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Spawns");
                });
#pragma warning restore 612, 618
        }
    }
}
