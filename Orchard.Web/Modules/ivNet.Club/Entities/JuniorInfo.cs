
using FluentNHibernate.Mapping;

namespace ivNet.Club.Entities
{
    public class JuniorInfo : BaseEntity
    {
        public virtual string School { get; set; }
        public virtual string Team { get; set; }
        public virtual string Notes { get; set; }
    }

    public class JuniorInfoMap : ClassMap<JuniorInfo>
    {
        public JuniorInfoMap()
        {
            Id(x => x.Id);
            Map(x => x.School).Nullable().Length(50);
            Map(x => x.Team).Nullable().Length(50);
            Map(x => x.Notes).Nullable().Length(255);            

            Map(x => x.IsActive);

            Map(x => x.CreatedBy).Not.Nullable().Length(50);
            Map(x => x.CreateDate).Not.Nullable();
            Map(x => x.ModifiedBy).Not.Nullable().Length(50);
            Map(x => x.ModifiedDate).Not.Nullable();
        }
    }
}