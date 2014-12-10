
using System.Collections.Generic;
using FluentNHibernate.Mapping;

namespace ivNet.Club.Entities
{
    public class Team : BaseEntity
    {
        public virtual void Init()
        {
            Locations = new List<Location>();           
        }

        public virtual string Name { get; set; }
        public virtual IList<Location> Locations { get; set; }
    }

    public class TeamMap : ClassMap<Team>
    {
        public TeamMap()
        {
            Id(x => x.Id);
            Map(x => x.Name).Not.Nullable().Length(120).UniqueKey("ix_Team_Unique");

            HasMany(x => x.Locations)
               .Inverse()
               .Cascade.All();

            Map(x => x.IsActive);

            Map(x => x.CreatedBy).Not.Nullable().Length(50);
            Map(x => x.CreateDate).Not.Nullable();
            Map(x => x.ModifiedBy).Not.Nullable().Length(50);
            Map(x => x.ModifiedDate).Not.Nullable();
        }
    }
}