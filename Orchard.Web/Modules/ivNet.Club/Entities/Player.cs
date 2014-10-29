
using System.Collections.Generic;
using FluentNHibernate.Mapping;
using NHibernate.Mapping;

namespace ivNet.Club.Entities
{
    public class Player : BaseEntity
    {
        public virtual string Number { get; set; }
        public virtual string Team { get; set; }
        public virtual Kit Kit { get; set; }
        public virtual List<Fee> Fees { get; set; }
    }

    public class PlayerMap : ClassMap<Player>
    {
        public PlayerMap()
        {
            Id(x => x.Id);
            Map(x => x.Number).Not.Nullable().Length(50);
            Map(x => x.Team).Nullable().Length(50);

            References(x => x.Kit);
            HasMany(x => x.Fees)
                .Inverse();
               // .Cascade.All();

            Map(x => x.IsActive);

            Map(x => x.CreatedBy).Not.Nullable().Length(50);
            Map(x => x.CreateDate).Not.Nullable();
            Map(x => x.ModifiedBy).Not.Nullable().Length(50);
            Map(x => x.ModifiedDate).Not.Nullable();
        }
    }
}