using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using GoldRoger.Entity.Entities;

public class MatchRefereeMap : IEntityTypeConfiguration<MatchReferee>
{
    public void Configure(EntityTypeBuilder<MatchReferee> builder)
    {
        builder.ToTable("MatchReferees");

        // Define que MatchId y RefereeId no son claves primarias ahora
        builder.HasKey(mr => new { mr.MatchId, mr.RefereeId });

        // Relación con la tabla 'Match' sin claves foráneas (sin eliminación en cascada)
        builder.HasOne(mr => mr.Match)
            .WithMany(m => m.MatchReferees)
            .HasForeignKey(mr => mr.MatchId) // Relación con MatchId
            .OnDelete(DeleteBehavior.Restrict);  // El comportamiento en borrado puede ser configurado

        // Relación con la tabla 'Referee' sin claves foráneas (sin eliminación en cascada)
        builder.HasOne(mr => mr.Referee)
            .WithMany(r => r.MatchReferees)
            .HasForeignKey(mr => mr.RefereeId) // Relación con RefereeId
            .OnDelete(DeleteBehavior.Restrict);  // El comportamiento en borrado puede ser configurado
    }
}