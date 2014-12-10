
using FluentNHibernate.Mapping;

namespace ivNet.Club.Entities
{
    public class Location : BaseEntity
    {

        public virtual string Name { get; set; }
        public virtual string Postcode { get; set; }
        public virtual decimal Longitude { get; set; }
        public virtual decimal Latitude { get; set; }
        public virtual Team Team { get; set; }
        public virtual Opponent Opponent { get; set; }
    }

    public class LocationMap : ClassMap<Location>
    {
        public LocationMap()
        {
            Id(x => x.Id);
            Map(x => x.Name).Not.Nullable().Length(120).UniqueKey("ix_Location_Unique");
          
            Map(x => x.Postcode).Not.Nullable().Length(12);
            Map(x => x.Longitude).Nullable().Length(50);
            Map(x => x.Latitude).Nullable().Length(50);            

            Map(x => x.IsActive);

            References(x => x.Team);
            References(x => x.Opponent);

            Map(x => x.CreatedBy).Not.Nullable().Length(50);
            Map(x => x.CreateDate).Not.Nullable();
            Map(x => x.ModifiedBy).Not.Nullable().Length(50);
            Map(x => x.ModifiedDate).Not.Nullable();
        }
    }
}