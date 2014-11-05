
using System;
using System.Collections.Generic;
using FluentNHibernate.Mapping;

namespace ivNet.Club.Entities
{
    public class Junior : BaseEntity 
    {
        public virtual string JuniorKey { get; set; }
        public virtual string JuniorGuardianKey { get; set; }
        public virtual DateTime Dob { get; set; }

        public virtual ClubMember ClubMember { get; set; }    
        public virtual Player Player { get; set; }
        public virtual JuniorInfo JuniorInfo { get; set; }        

        public virtual IList<Guardian> Guardians { get; protected set; }

        public virtual byte IsVetted { get; set; }

        public virtual void Init()
        {
            Guardians = new List<Guardian>();
            ClubMember = new ClubMember();
            Player = new Player();
            JuniorInfo= new JuniorInfo();            
        }

    }

    public class JuniorMap : ClassMap<Junior>
    {
        public JuniorMap()
        {
            Id(x => x.Id);
            Map(x => x.JuniorGuardianKey).Not.Nullable().Length(120).UniqueKey("ix_JuniorGuardian_Unique");
            Map(x => x.JuniorKey).Not.Nullable().Length(120).UniqueKey("ix_Junior_Unique");

            Map(x => x.Dob).Not.Nullable();

            References(x => x.Player);
            References(x => x.ClubMember);
            References(x => x.JuniorInfo);
            
            HasManyToMany(x => x.Guardians)
                .Inverse()
                .Cascade.SaveUpdate()
                .Table("ivNetJuniorGuardian");

            Map(x => x.IsVetted);

            Map(x => x.IsActive);

            Map(x => x.CreatedBy).Nullable().Length(125);
            Map(x => x.CreateDate).Nullable();
            Map(x => x.ModifiedBy).Nullable().Length(125);
            Map(x => x.ModifiedDate).Nullable();
        }
    }
}