using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PM.Core.Entities;

namespace PM.DataAccess.EntityConfiguration
{
    public class ParkingSessionConfiguration : IEntityTypeConfiguration<ParkingSession>
    {
        public void Configure(EntityTypeBuilder<ParkingSession> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Registration)
                .IsRequired()
                .HasMaxLength(20);
            builder.Property(p => p.VehicleType)
                .IsRequired();

            builder.Property(p => p.TimeIn).IsRequired();
            builder.Property(p => p.TimeOut).IsRequired(false);

            builder.HasOne(x => x.ParkingSpace)
                .WithMany()
                .HasForeignKey(x => x.ParkingSpaceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
