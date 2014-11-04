
using System;
using FluentNHibernate.Mapping;

namespace ivNet.Club.Entities
{
    public class ConfigurationItem : BaseEntity
    {
        public virtual string Name { get; set; }
        public virtual string ItemGroup { get; set; }
        public virtual string Text { get; set; }
        public virtual DateTime? Date { get; set; }
        public virtual int Number { get; set; }
    }

    public class ConfigurationItemMap : ClassMap<ConfigurationItem>
    {
        public ConfigurationItemMap()
        {
            Id(x => x.Id);            
            Map(x => x.Name).Not.Nullable().Length(120).UniqueKey("ix_ConfigurationItem_Unique");

            Map(x => x.ItemGroup);
            Map(x => x.Text);
            Map(x => x.Date);
            Map(x => x.Number);

            Map(x => x.IsActive);

            Map(x => x.CreatedBy).Not.Nullable().Length(50);
            Map(x => x.CreateDate).Not.Nullable();
            Map(x => x.ModifiedBy).Not.Nullable().Length(50);
            Map(x => x.ModifiedDate).Not.Nullable();
        }
    }
}