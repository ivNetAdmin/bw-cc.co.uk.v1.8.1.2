
namespace ivNet.Club.ViewModel
{
    public class FixtureItemConfigViewModel
    {
        public int Id { get; set; }
         public string Type { get; set; }
        public byte IsActive { get; set; }

        public string Name { get; set; }
        public string Postcode { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }        
    }
}