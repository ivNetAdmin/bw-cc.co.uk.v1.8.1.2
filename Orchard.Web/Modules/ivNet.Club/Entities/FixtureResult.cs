
using FluentNHibernate.Mapping;

namespace ivNet.Club.Entities
{
    public class FixtureResult : BaseEntity
    {
        public virtual string Name { get; set; }
    }

    public class FixtureResulteMap : ClassMap<FixtureResult>
    {
        public FixtureResulteMap()
        {
            Id(x => x.Id);
            Map(x => x.Name).Not.Nullable().Length(120).UniqueKey("ix_FixtureResult_Unique");

            Map(x => x.IsActive);

            Map(x => x.CreatedBy).Not.Nullable().Length(50);
            Map(x => x.CreateDate).Not.Nullable();
            Map(x => x.ModifiedBy).Not.Nullable().Length(50);
            Map(x => x.ModifiedDate).Not.Nullable();
        }
    }
}