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

    public class UserTypeMap : IEntityTypeConfiguration<UserType>
    {
        public void Configure(EntityTypeBuilder<UserType> builder)
        {
            builder.ToTable("UserTypes");

            builder.HasKey(ut => ut.UserTypeId);
            builder.Property(ut => ut.UserTypeName)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}
