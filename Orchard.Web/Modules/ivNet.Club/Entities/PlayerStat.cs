
using FluentNHibernate.Mapping;

namespace ivNet.Club.Entities
{
    public class PlayerStat : BaseEntity
    {
        public virtual Fixture Fixture { get; set; }
        public virtual Player Player { get; set; }
        public virtual CricketStat CricketStat { get; set; }
    }

    public class PlayerStatMap : ClassMap<PlayerStat>
    {
        public PlayerStatMap()
        {
            Id(x => x.Id);

            References(x => x.Fixture);
            References(x => x.Player);
            References(x => x.CricketStat); 

            Map(x => x.IsActive);

            Map(x => x.CreatedBy).Not.Nullable().Length(50);
            Map(x => x.CreateDate).Not.Nullable();
            Map(x => x.ModifiedBy).Not.Nullable().Length(50);
            Map(x => x.ModifiedDate).Not.Nullable();
        }
    }
}