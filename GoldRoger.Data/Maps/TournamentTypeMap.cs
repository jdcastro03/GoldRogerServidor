using GoldRoger.Entity.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldRoger.Data.Maps
{
    public class TournamentTypeMap : IEntityTypeConfiguration<TournamentType>
    {
        public void Configure(EntityTypeBuilder<TournamentType> builder)
        {
            builder.ToTable("TournamentTypes");

            builder.HasKey(tt => tt.TournamentTypeId); // Establece TypeId como clave primaria

            builder.Property(tt => tt.TournamentTypeName)
                .IsRequired()
                .HasMaxLength(50); // Nombre del tipo de torneo, requerido y con longitud máxima de 50 caracteres
        }
    }
}
