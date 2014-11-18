
using Orchard.UI.Resources;

namespace ivNet.Club
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();

            manifest.DefineStyle("trNgGrid").SetUrl("trNgGrid.min.css").SetDependencies("Bootstrap");
            manifest.DefineStyle("Club.New.Member").SetUrl("new.member.min.css");

            manifest.DefineScript("trNgGrid").SetUrl("trNgGrid.min.js").SetVersion("1.2.9").SetDependencies("AngularJS");
          
            manifest.DefineScript("AngularJS").SetUrl("anjular.min.js").SetVersion("1.2.9").SetDependencies("jQueryUI");
            manifest.DefineScript("AngularJS-Resource").SetUrl("angular-resource.min.js").SetVersion("1.2.18").SetDependencies("AngularJS");
            manifest.DefineScript("AngularJS-UI").SetUrl("angular-ui-utils.min.js").SetVersion("0.1.1").SetDependencies("AngularJS");
            manifest.DefineScript("AngularJS-Autocomplete").SetUrl("angular-autocomplete.js").SetVersion("0.1.1").SetDependencies("AngularJS");            
           
            manifest.DefineScript("Club.New.Member").SetUrl("app/new.member.js").SetVersion("1.0").SetDependencies("AngularJS");
            manifest.DefineScript("Club.New.Member.Fee").SetUrl("app/new.member.fee.js").SetVersion("1.0").SetDependencies("trNgGrid");

            manifest.DefineScript("Club.Member.Registration.Details").SetUrl("app/member.registration.details.js").SetVersion("1.0").SetDependencies("trNgGrid");           
            
            manifest.DefineScript("Club.Configuration").SetUrl("app/club.configuration.js").SetVersion("1.0").SetDependencies("trNgGrid");

            manifest.DefineScript("Club.Admin.Member.Activate").SetUrl("app/admin.member.activate.js").SetVersion("1.0").SetDependencies("trNgGrid");
            manifest.DefineScript("Club.Admin.Member.List").SetUrl("app/admin.member.list.js").SetVersion("1.0").SetDependencies("trNgGrid");
            
        }
    }
}

