using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySQLDemo.Core.DB;
using MySQLDemo.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySQLDemo.Core.Mapper
{
    public partial class UserMapper : NopEntityTypeConfiguration<User>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable(nameof(User));
            builder.HasKey(t => t.Id);

            builder.Property(category => category.Name).HasMaxLength(256).IsRequired();
            builder.Property(category => category.Password).HasMaxLength(256).IsRequired();
          
            base.Configure(builder);
        }

        #endregion
    }
}
