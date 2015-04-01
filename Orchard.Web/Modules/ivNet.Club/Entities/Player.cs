
using System.Collections.Generic;
using FluentNHibernate.Mapping;

namespace ivNet.Club.Entities
{
    public class Player : BaseEntity
    {
        public virtual string Number { get; set; }
        public virtual string Name { get; set; }        
        public virtual string Team { get; set; }
        public virtual Kit Kit { get; set; }
        public virtual IList<Fee> Fees { get; set; }
        public virtual IList<TeamSelection> TeamSelections { get; set; }

        public virtual void Init()
        {
            TeamSelections = new List<TeamSelection>();
            Fees = new List<Fee>();
            Kit = new Kit();
        }
    }

    public class PlayerMap : ClassMap<Player>
    {
        public PlayerMap()
        {
            Id(x => x.Id);
            Map(x => x.Number).Not.Nullable().Length(50).Not.Nullable().Length(120).UniqueKey("ix_Player_Unique");
            Map(x => x.Name).Not.Nullable().Length(50);   
            Map(x => x.Team).Nullable().Length(50);

            References(x => x.Kit);

            HasManyToMany(x => x.TeamSelections)
               .Inverse()
               .Cascade.SaveUpdate()
               .Table("ivNetTeamSelectionPlayer");

            HasMany(x => x.Fees)
                .Inverse()
                .Cascade.All();

            Map(x => x.IsActive);

            Map(x => x.CreatedBy).Not.Nullable().Length(50);
            Map(x => x.CreateDate).Not.Nullable();
            Map(x => x.ModifiedBy).Not.Nullable().Length(50);
            Map(x => x.ModifiedDate).Not.Nullable();
        }
    }
}