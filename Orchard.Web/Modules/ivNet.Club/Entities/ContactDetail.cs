
using FluentNHibernate.Mapping;

namespace ivNet.Club.Entities
{
    public class ContactDetail : BaseEntity
    {
        public virtual string ContactDetailKey { get; set; }
       
        public virtual string Address { get; set; }
        public virtual string Town { get; set; }
        public virtual string Postcode { get; set; }
        public virtual string Email { get; set; }
        public virtual string Telephone { get; set; }
    }

    public class ContactDetailMap : ClassMap<ContactDetail>
    {
        public ContactDetailMap()
        {
            Id(x => x.Id);
            Map(x => x.ContactDetailKey).Not.Nullable().Length(120).UniqueKey("ix_ContactDetails_Unique");

            Map(x => x.Address).Not.Nullable().Length(100);
            Map(x => x.Postcode).Not.Nullable().Length(12);
            Map(x => x.Town).Nullable().Length(50);
            Map(x => x.Email).Not.Nullable().Length(50);
            Map(x => x.Telephone).Not.Nullable().Length(50);

            Map(x => x.IsActive);

            Map(x => x.CreatedBy).Not.Nullable().Length(50);
            Map(x => x.CreateDate).Not.Nullable();
            Map(x => x.ModifiedBy).Not.Nullable().Length(50);
            Map(x => x.ModifiedDate).Not.Nullable();
        }
    }
}
