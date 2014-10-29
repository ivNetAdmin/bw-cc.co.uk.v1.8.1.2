using System;
using FluentNHibernate.Mapping;

namespace ivNet.Club.Entities
{
    public class Fee : BaseEntity
    {      
        public virtual DateTime PaidDate { get; set; }
        public virtual Decimal Amount { get; set; }
        public virtual int FeeType { get; set; }
        public virtual string Season { get; set; }
        public virtual string Notes { get; set; }

        public virtual Player Player { get; set; }
    }

    public class FeeMap : ClassMap<Fee>
    {
        public FeeMap()
        {
            Id(x => x.Id);
            Map(x => x.PaidDate).Not.Nullable();
            Map(x => x.Amount).Not.Nullable();
            Map(x => x.FeeType).Not.Nullable();
            Map(x => x.Season).Not.Nullable();
            Map(x => x.Notes);

            Map(x => x.IsActive);

            Map(x => x.CreatedBy).Nullable().Length(125);
            Map(x => x.CreateDate).Nullable();
            Map(x => x.ModifiedBy).Nullable().Length(125);
            Map(x => x.ModifiedDate).Nullable();
        }
    }
}