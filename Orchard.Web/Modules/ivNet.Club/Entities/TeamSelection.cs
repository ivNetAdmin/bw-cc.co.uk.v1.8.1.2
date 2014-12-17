
using System.Collections.Generic;
using FluentNHibernate.Mapping;

namespace ivNet.Club.Entities
{
    public class TeamSelection : BaseEntity
    {
        public virtual Fixture Fixture { get; set; }       
        public virtual List<Player> Players { get; set; }

        public virtual void Init()
        {
            Players = new List<Player>();
            Fixture = new Fixture();          
        }

        public virtual void AddPlayer(Player player)
        {
            player.TeamSelections.Add(this);
            Players.Add(player);
        }

        public virtual void RemovePlayer(Player player)
        {
            player.TeamSelections.Remove(this);
            Players.Remove(player);
        }
    }

    public class TeamSelectionMap : ClassMap<TeamSelection>
    {
        public TeamSelectionMap()
        {
            Id(x => x.Id);
          
            References(x => x.Fixture);

            HasManyToMany(x => x.Players)
              .Cascade.SaveUpdate()
              .Table("ivNetTeamSelectionPlayer");

            Map(x => x.IsActive);

            Map(x => x.CreatedBy).Not.Nullable().Length(50);
            Map(x => x.CreateDate).Not.Nullable();
            Map(x => x.ModifiedBy).Not.Nullable().Length(50);
            Map(x => x.ModifiedDate).Not.Nullable();
        }
    }
}