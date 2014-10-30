
namespace ivNet.Club.ViewModel
{
    public class RecaptchaViewModel
    {
        public string PrivateKey { get; set; }
        public string PublicKey { get; set; }
        public string Challenge { get; set; }
        public string Response { get; set; }
        
    }
}