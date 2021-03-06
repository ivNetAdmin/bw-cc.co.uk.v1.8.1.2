﻿
using System;

namespace ivNet.Club.ViewModel
{
    public class FixtureViewModel
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public string HomeAway { get; set; }
        public string Team { get; set; }
        public int TeamId { get; set; }
        public string Opponent { get; set; }
        public int OpponentId { get; set; }        
        public string FixtureType { get; set; }
        public int FixtureTypeId { get; set; }
        public string Location { get; set; }
        public int LocationId { get; set; }
        public int TeamSelectionId { get; set; }
        public string ResultType { get; set; }
        public int ResultTypeId { get; set; }
        public string Score { get; set; }
    }
}