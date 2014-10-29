﻿
using System.Collections.Generic;
using FluentNHibernate.Mapping;

namespace ivNet.Club.Entities
{
    public class Guardian : BaseEntity
    {
        public virtual string GuardianKey { get; set; }

        public virtual ClubMember ClubMember { get; set; }
        public virtual ContactDetail ContactDetail { get; set; }

        public virtual IList<Junior> Juniors { get; protected set; }

        public virtual void Init()
        {
            Juniors = new List<Junior>();
            ClubMember = new ClubMember();
            ContactDetail = new ContactDetail();
        }

        public virtual void AddJunior(Junior junior)
        {
            junior.Guardians.Add(this);
            Juniors.Add(junior);
        }

        public virtual void RemoveJunior(Junior junior)
        {
            junior.Guardians.Remove(this);
            Juniors.Remove(junior);
        }
    }

    public class GuardianMap : ClassMap<Guardian>
    {
        public GuardianMap()
        {
            Id(x => x.Id);

            Map(x => x.GuardianKey).Not.Nullable().Length(120).UniqueKey("ix_Guardian_Unique");

            References(x => x.ClubMember); //.Cascade.SaveUpdate();
            References(x => x.ContactDetail); //.Cascade.SaveUpdate();
            HasManyToMany(x => x.Juniors)
                 .Cascade.SaveUpdate()
                 .Table("ivNetJuniorGuardian");

            Map(x => x.IsActive);

            Map(x => x.CreatedBy).Nullable().Length(125);
            Map(x => x.CreateDate).Nullable();
            Map(x => x.ModifiedBy).Nullable().Length(125);
            Map(x => x.ModifiedDate).Nullable();
        }
    }
}
