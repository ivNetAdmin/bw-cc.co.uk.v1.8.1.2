
using System;

namespace ivNet.Club.ViewModel
{
    public class ConfigurationItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ItemGroup { get; set; }
        public string Text { get; set; }
        public DateTime? Date { get; set; }
        public int Number { get; set; }
        public byte IsActive { get; set; }
    }
}