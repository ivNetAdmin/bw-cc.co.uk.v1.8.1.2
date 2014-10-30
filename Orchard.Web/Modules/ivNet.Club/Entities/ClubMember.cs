
using System;
using System.Collections.Generic;
using FluentNHibernate.Mapping;

namespace ivNet.Club.Entities
{
    public class ClubMember : BaseEntity
    {
        public virtual string ClubMemberKey { get; set; }
       
        public virtual string Surname { get; set; }
        public virtual string Firstname { get; set; }
        public virtual string NickName { get; set; }
        public virtual int UserId { get; set; }
        public virtual byte IsNewReg { get; set; }       
    }

    public class ClubMemberMap : ClassMap<ClubMember>
    {
        public ClubMemberMap()
        {
            Id(x => x.Id);
            Map(x => x.ClubMemberKey).Not.Nullable().Length(120).UniqueKey("ix_ClubMember_Unique");

            Map(x => x.UserId);
            Map(x => x.IsNewReg);

            Map(x => x.Surname).Not.Nullable().Length(50);
            Map(x => x.Firstname).Not.Nullable().Length(50);
            Map(x => x.NickName).Nullable().Length(50);

            Map(x => x.IsActive);

            Map(x => x.CreatedBy).Not.Nullable().Length(50);
            Map(x => x.CreateDate).Not.Nullable();
            Map(x => x.ModifiedBy).Not.Nullable().Length(50);
            Map(x => x.ModifiedDate).Not.Nullable();
        }
    }
}
