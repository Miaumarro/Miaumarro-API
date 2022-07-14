using MiauDatabase.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MiauDatabase.Config;

/// <summary>
/// Configures relationships for <see cref="PetEntity"/>.
/// </summary>
internal sealed class PetEntityTypeConfiguration : IEntityTypeConfiguration<PetEntity>
{
    public void Configure(EntityTypeBuilder<PetEntity> builder)
    {
        // Pets -> Appointments
        builder.HasMany(x => x.AppointmentsRel)
            .WithOne(x => x.PetRel)
            .HasForeignKey(x => x.PetIdFK)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}