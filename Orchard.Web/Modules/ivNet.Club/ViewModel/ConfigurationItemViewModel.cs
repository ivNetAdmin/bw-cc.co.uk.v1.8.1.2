
using System;

namespace ivNet.Club.ViewModel
{
    public class ConfigurationItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
        public DateTime? Date { get; set; }
        public int Number { get; set; }
        public byte IsActive { get; set; }
    }
}