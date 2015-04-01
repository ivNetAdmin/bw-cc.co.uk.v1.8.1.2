using System;
using System.Collections.Generic;
using FluentNHibernate.Mapping;

namespace ivNet.Club.Entities
{
    public class Senior : BaseEntity
    {
        public virtual string SeniorKey { get; set; }

        public virtual Member Member { get; set; }
        public virtual Player Player { get; set; }
        public virtual ContactDetail ContactDetail { get; set; }
        public virtual AddressDetail AddressDetail { get; set; }

        public virtual byte IsVetted { get; set; }

        public virtual void Init()
        {
            Member = new Member();
            Player = new Player();
            ContactDetail = new ContactDetail();
            AddressDetail = new AddressDetail();
        }

    }

    public class SeniorMap : ClassMap<Senior>
    {
        public SeniorMap()
        {
            Id(x => x.Id);
            Map(x => x.SeniorKey).Not.Nullable().Length(120).UniqueKey("ix_Senior_Unique");

            References(x => x.Player);
            References(x => x.Member);
            References(x => x.ContactDetail);
            References(x => x.AddressDetail);

            Map(x => x.IsVetted);

            Map(x => x.IsActive);

            Map(x => x.CreatedBy).Nullable().Length(125);
            Map(x => x.CreateDate).Nullable();
            Map(x => x.ModifiedBy).Nullable().Length(125);
            Map(x => x.ModifiedDate).Nullable();
        }
    }
}