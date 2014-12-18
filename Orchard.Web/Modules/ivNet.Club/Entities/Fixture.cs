
using System;
using FluentNHibernate.Mapping;

namespace ivNet.Club.Entities
{
    public class Fixture : BaseEntity
    {
        public virtual Team Team { get; set; }
        public virtual Opponent Opponent { get; set; }        
        public virtual Location Location { get; set; }
        public virtual FixtureType FixtureType { get; set; }        
        public virtual TeamSelection TeamSelection { get; set; }        

        public virtual DateTime Date { get; set; }
        public virtual string HomeAway { get; set; }
        public virtual string FixtureKey { get; set; }
    }

    public class FixtureMap : ClassMap<Fixture>
    {
        public FixtureMap()
        {
            Id(x => x.Id);
            Map(x => x.FixtureKey)
                .Not.Nullable()
                .Length(120)
                .UniqueKey("ix_Fixture_Unique");

            References(x => x.Team);
            References(x => x.Opponent);
            References(x => x.Location);
            References(x => x.FixtureType);
            References(x => x.TeamSelection);

            Map(x => x.Date);
            Map(x => x.HomeAway).Length(10);  

            Map(x => x.IsActive);

            Map(x => x.CreatedBy).Not.Nullable().Length(50);
            Map(x => x.CreateDate).Not.Nullable();
            Map(x => x.ModifiedBy).Not.Nullable().Length(50);
            Map(x => x.ModifiedDate).Not.Nullable();
        }
    }
}