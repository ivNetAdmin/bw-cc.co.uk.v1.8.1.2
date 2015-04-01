
using FluentNHibernate.Mapping;
using Orchard.Data.Conventions;

namespace ivNet.Club.Entities
{
    public class MatchReport : BaseEntity
    {
        [StringLengthMax]
        public virtual string Report { get; set; }
        public virtual Fixture Fixture { get; set; }

        public virtual void Init()
        {
            Fixture = new Fixture();
        }
    }

    public class MatchReportMap : ClassMap<MatchReport>
    {
        public MatchReportMap()
        {
            Id(x => x.Id);

            //Map(x => x.Report).CustomType("StringClob").CustomSqlType("nvarchar(max)");
            Map(x => x.Report).Not.Nullable().Length(10000);

            References(x => x.Fixture);

            Map(x => x.IsActive);

            Map(x => x.CreatedBy).Not.Nullable().Length(50);
            Map(x => x.CreateDate).Not.Nullable();
            Map(x => x.ModifiedBy).Not.Nullable().Length(50);
            Map(x => x.ModifiedDate).Not.Nullable();
        }
    }
}