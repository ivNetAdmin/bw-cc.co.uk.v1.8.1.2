
using Orchard.UI.Resources;

namespace ivNet.Club
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();

            manifest.DefineStyle("Club.Member.Registration").SetUrl("member.registration.min.css");
            
            manifest.DefineScript("AngularJS").SetUrl("anjular.min.js").SetVersion("1.2.9").SetDependencies("jQuery");
            
            manifest.DefineScript("Club.Member.Registration").SetUrl("member.registration.js").SetVersion("1.0").SetDependencies("AngularJS");
            manifest.DefineScript("Club.Member.Registration.Payment").SetUrl("member.registration.payment.js").SetVersion("1.0").SetDependencies("AngularJS");
        }
    }
}