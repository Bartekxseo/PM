using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PM.Core.Entities;
using PM.DataAccess.EntityConfiguration.Seed;

namespace PM.DataAccess.EntityConfiguration
{
    public class ParkingSpaceConfiguration : IEntityTypeConfiguration<ParkingSpace>
    {
        public void Configure(EntityTypeBuilder<ParkingSpace> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.SpaceNumber)
                .IsRequired();
            builder.Property(p => p.Status)
                .IsRequired();

            builder.HasData(ParkingSpaceSeed.Entries);
        }
    }
}
