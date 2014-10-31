
using Orchard.UI.Resources;

namespace ivNet.Club
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();

            manifest.DefineStyle("trNgGrid").SetUrl("trNgGrid.min.css").SetDependencies("Bootstrap");
            manifest.DefineStyle("Club.Member.Registration").SetUrl("member.registration.min.css");

            manifest.DefineScript("trNgGrid").SetUrl("trNgGrid.min.js").SetVersion("1.2.9").SetDependencies("AngularJS");
            manifest.DefineScript("AngularJS").SetUrl("anjular.min.js").SetVersion("1.2.9").SetDependencies("jQuery");
            manifest.DefineScript("AngularJS-Resource").SetUrl("angular-resource.min.js").SetVersion("1.2.18").SetDependencies("AngularJS");

            

            manifest.DefineScript("Club.Member.Registration").SetUrl("member.registration.js").SetVersion("1.0").SetDependencies("AngularJS");
            manifest.DefineScript("Club.Member.Registration.Payment").SetUrl("member.registration.payment.js").SetVersion("1.0").SetDependencies("AngularJS");

            manifest.DefineScript("Club.Configuration").SetUrl("club.configuration.js").SetVersion("1.0").SetDependencies("trNgGrid");
        }
    }
}

