
using FluentNHibernate.Mapping;

namespace ivNet.Club.Entities
{
    public class MatchReport : BaseEntity
    {
        public virtual string Report { get; set; }
        public virtual Fixture Fixture { get; set; }
    }

    public class MatchReportMap : ClassMap<MatchReport>
    {
        public MatchReportMap()
        {
            Id(x => x.Id);

            Map(x => x.Report).CustomSqlType("ntext");

            References(x => x.Fixture);

            Map(x => x.IsActive);

            Map(x => x.CreatedBy).Not.Nullable().Length(50);
            Map(x => x.CreateDate).Not.Nullable();
            Map(x => x.ModifiedBy).Not.Nullable().Length(50);
            Map(x => x.ModifiedDate).Not.Nullable();
        }
    }
}