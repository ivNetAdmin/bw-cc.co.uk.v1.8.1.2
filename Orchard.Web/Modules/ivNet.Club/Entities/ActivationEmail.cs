﻿
using FluentNHibernate.Mapping;

namespace ivNet.Club.Entities
{
    public class ActivationEmail : BaseEntity
    {
        public virtual string Email { get; set; }
        public virtual string Firstname { get; set; }
        public virtual string Surname { get; set; }
        public virtual string UserName { get; set; }
        public virtual string DateSent { get; set; }
        public virtual string DatePWChange { get; set; }
        public virtual bool SentSuccess { get; set; }
    }

     public class ActivationEmailMap : ClassMap<ActivationEmail>
    {
        public ActivationEmailMap()
        {
            Id(x => x.Id);
        
            Map(x => x.Email).Not.Nullable().Length(50);
            Map(x => x.Firstname).Not.Nullable().Length(50);
            Map(x => x.Surname).Not.Nullable().Length(50);
            Map(x => x.UserName).Not.Nullable().Length(50);
            Map(x => x.DateSent).Nullable();
            Map(x => x.DatePWChange).Nullable();
            Map(x => x.SentSuccess);

            Map(x => x.IsActive);

            Map(x => x.CreatedBy).Not.Nullable().Length(50);
            Map(x => x.CreateDate).Not.Nullable();
            Map(x => x.ModifiedBy).Not.Nullable().Length(50);
            Map(x => x.ModifiedDate).Not.Nullable();
        }
    }
}