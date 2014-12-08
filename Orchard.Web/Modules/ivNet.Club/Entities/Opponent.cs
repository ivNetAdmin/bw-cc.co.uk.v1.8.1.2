
using FluentNHibernate.Mapping;

namespace ivNet.Club.Entities
{
    public class Opponent : BaseEntity
    {
        public virtual string Name { get; set; }
    }

    public class OpponentMap : ClassMap<Opponent>
    {
        public OpponentMap()
        {
            Id(x => x.Id);
            Map(x => x.Name).Not.Nullable().Length(120).UniqueKey("ix_Opponent_Unique");

            Map(x => x.IsActive);

            Map(x => x.CreatedBy).Not.Nullable().Length(50);
            Map(x => x.CreateDate).Not.Nullable();
            Map(x => x.ModifiedBy).Not.Nullable().Length(50);
            Map(x => x.ModifiedDate).Not.Nullable();
        }
    }
}