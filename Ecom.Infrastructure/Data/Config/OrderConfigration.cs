using Microsoft.EntityFrameworkCore;
using Ecom.Core.Entites.Order;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecom.Infrastructure.Data.Config
{
    public class OrderConfigration : IEntityTypeConfiguration<Orders>
    {
        public void Configure(EntityTypeBuilder<Orders> builder)
        {
            builder.OwnsOne(o => o.shippingAddress,
                x => { x.WithOwner(); });

            builder.HasMany(x=>x.orderItems).WithOne().OnDelete(DeleteBehavior.Cascade);

            builder.Property(x=>x.status).HasConversion(o=>o.ToString(),
                               o=>(Status)Enum.Parse(typeof(Status),o));

            builder.Property(o => o.SubTotal).HasColumnType("decimal(18,2)");

        }
    }
}
