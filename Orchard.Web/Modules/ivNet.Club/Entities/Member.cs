
using FluentNHibernate.Mapping;

namespace ivNet.Club.Entities
{
    public class Member : BaseEntity
    {        
        public virtual string MemberKey { get; set; }

        public virtual string Surname { get; set; }
        public virtual string Firstname { get; set; }
        public virtual string Nickname { get; set; }
        public virtual int LegacyId { get; set; }
        public virtual int UserId { get; set; }
        public virtual int DuplicateCounter { get; set; }       
    }

    public class MemberMap : ClassMap<Member>
    {
        public MemberMap()
        {
            Id(x => x.Id);

            Map(x => x.LegacyId);
            Map(x => x.MemberKey).Not.Nullable().Length(120).UniqueKey("ix_Member_Unique");            

            Map(x => x.UserId);
            Map(x => x.DuplicateCounter);

            Map(x => x.Surname).Not.Nullable().Length(50);
            Map(x => x.Firstname).Not.Nullable().Length(50);
            Map(x => x.Nickname).Nullable().Length(50);            

            Map(x => x.IsActive);

            Map(x => x.CreatedBy).Not.Nullable().Length(50);
            Map(x => x.CreateDate).Not.Nullable();
            Map(x => x.ModifiedBy).Not.Nullable().Length(50);
            Map(x => x.ModifiedDate).Not.Nullable();
        }
    }
}
