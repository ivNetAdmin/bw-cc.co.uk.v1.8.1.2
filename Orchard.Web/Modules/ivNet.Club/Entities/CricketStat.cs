
using FluentNHibernate.Mapping;

namespace ivNet.Club.Entities
{
    public class CricketStat : BaseEntity
    {
        public virtual int Innings { get; set; }
        public virtual int Runs { get; set; }
        public virtual int NotOut { get; set; }

        public virtual int Overs { get; set; }
        public virtual int Maidens { get; set; }
        public virtual int RunsConceeded { get; set; }
        public virtual int Wickets { get; set; }

        public virtual int Catches { get; set; }
        public virtual int Keeper { get; set; }

        public virtual int Captain { get; set; }

    }

    public class CricketStatMap : ClassMap<CricketStat>
    {
        public CricketStatMap()
        {
            Id(x => x.Id);

            Map(x => x.Innings);
            Map(x => x.Runs);
            Map(x => x.NotOut);

            Map(x => x.Overs);
            Map(x => x.Maidens);
            Map(x => x.RunsConceeded);
            Map(x => x.Wickets);

            Map(x => x.Catches);

            Map(x => x.Keeper);
            Map(x => x.Captain);

            Map(x => x.IsActive);

            Map(x => x.CreatedBy).Not.Nullable().Length(50);
            Map(x => x.CreateDate).Not.Nullable();
            Map(x => x.ModifiedBy).Not.Nullable().Length(50);
            Map(x => x.ModifiedDate).Not.Nullable();
        }
    }
}