﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PlaceholderYacht.Models.Entities
{
    public partial class WindCatchersContext : DbContext
    {
        

        public virtual DbSet<Boat> Boat { get; set; }
        public virtual DbSet<Vpp> Vpp { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Boat>(entity =>
            {
                entity.ToTable("Boat", "Sai");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Boatname)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('Standard')");

                entity.Property(e => e.Manufacturer)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Modelname)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Uid)
                    .HasColumnName("UID")
                    .HasMaxLength(450);
            });

            modelBuilder.Entity<Vpp>(entity =>
            {
                entity.ToTable("VPP", "Sai");

                entity.HasIndex(e => new { e.BoatId, e.Tws, e.WindDegree })
                    .HasName("vpp_uq")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.BoatId).HasColumnName("BoatID");

                entity.Property(e => e.Knot).HasColumnType("decimal(4, 2)");

                entity.Property(e => e.Tws).HasColumnName("TWS");

                entity.HasOne(d => d.Boat)
                    .WithMany(p => p.Vpp)
                    .HasForeignKey(d => d.BoatId)
                    .HasConstraintName("FK__VPP__BoatID__02FC7413");
            });
        }
    }
}