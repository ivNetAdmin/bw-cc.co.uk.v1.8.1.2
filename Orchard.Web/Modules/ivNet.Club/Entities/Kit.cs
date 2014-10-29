
using FluentNHibernate.Mapping;

namespace ivNet.Club.Entities
{
    public class Kit : BaseEntity
    {
        public virtual string ShirtSize { get; set; }
        public virtual string ShortSize { get; set; }
        public virtual string BootSize { get; set; }
    }

    public class KitMap : ClassMap<Kit>
    {
        public KitMap()
        {
            Id(x => x.Id);
            Map(x => x.ShirtSize).Nullable().Length(25);
            Map(x => x.ShortSize).Nullable().Length(25);
            Map(x => x.BootSize).Nullable().Length(25);

            Map(x => x.IsActive);

            Map(x => x.CreatedBy).Not.Nullable().Length(50);
            Map(x => x.CreateDate).Not.Nullable();
            Map(x => x.ModifiedBy).Not.Nullable().Length(50);
            Map(x => x.ModifiedDate).Not.Nullable();
        }
    }
}
