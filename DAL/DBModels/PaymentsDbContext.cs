﻿using Microsoft.EntityFrameworkCore;

namespace DAL.DBModels
{
    public partial class PaymentsDbContext : DbContext
    {
        public PaymentsDbContext()
        {
        }

        public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TransactionDTO> Transactions { get; set; }
        public virtual DbSet<UserDTO> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<TransactionDTO>(entity =>
            {
                entity.HasKey(e => e.TransactionId)
                    .HasName("PK__Transacctions_$Id");

                entity.HasIndex(e => new { e.UserId, e.VendorId, e.ExternalId, e.OrderId })
                    .HasName("in_Transactions_$orderId$userId$vendorId$externalId");

                entity.Property(e => e.TransactionId)
                    .HasColumnName("TransactionID");

                    entity.Property(e => e.ExternalId)
                        .HasColumnName("ExternalID")
                    .HasMaxLength(50);

                entity.Property(e => e.Instrument)
                    .HasMaxLength(20);

                entity.Property(e => e.Metadata)
                    .HasMaxLength(1000);

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.Response).HasMaxLength(4000); ;

                entity.Property(e => e.Status)
                    .HasMaxLength(30);

                entity.Property(e => e.TransactionTime).HasColumnType("datetime");

                entity.Property(e => e.TransactionType)
                    .HasMaxLength(20);

                entity.Property(e => e.UserId).
                    HasMaxLength(450); ;

                entity.Property(e => e.VendorId).HasColumnName("VendorID");
            });

            modelBuilder.Entity<UserDTO>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK__Users__Id");

                entity.Property(e => e.UserId)
                    .HasColumnName("UserID");

                entity.Property(e => e.Email)
                    .HasColumnName("Email")
                    .HasMaxLength(50);

                entity.Property(e => e.ExternalId)
                    .HasColumnName("ExternalID")
                    .HasMaxLength(50);

                entity.Property(e => e.UserToken).HasMaxLength(50);
            });
        }
    }
}
