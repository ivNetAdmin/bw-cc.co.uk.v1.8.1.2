﻿
using Orchard.UI.Resources;

namespace ivNet.Club
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();

            #region rework

            manifest.DefineStyle("Club.Base").SetUrl("club.base.min.css");

            manifest.DefineStyle("Membership.New.Fee").SetUrl("membership.new.fee.min.css").SetDependencies("Club.Base");
            manifest.DefineStyle("Membership.New.Registration").SetUrl("membership.new.registration.min.css").SetDependencies("Club.Base");
            manifest.DefineStyle("Membership.My.Registration").SetUrl("membership.my.registration.min.css").SetDependencies("Club.Base");

            manifest.DefineStyle("Admin.Member.Activate").SetUrl("admin.member.activate.min.css").SetDependencies("Club.Base");
            manifest.DefineStyle("Admin.Member.List.All").SetUrl("admin.member.list.all.min.css").SetDependencies("Club.Base");
            manifest.DefineStyle("Admin.Member.List.Contacts").SetUrl("admin.member.list.contacts.min.css").SetDependencies("Club.Base");

            manifest.DefineStyle("Admin.Fixture.List.All").SetUrl("admin.fixture.list.all.min.css").SetDependencies("Club.Base");
            manifest.DefineStyle("Admin.Fixture.New").SetUrl("admin.fixture.new.min.css").SetDependencies("Club.Base");
            manifest.DefineStyle("Admin.Team.New").SetUrl("admin.team.new.min.css").SetDependencies("Club.Base");
            manifest.DefineStyle("Admin.Stats.New").SetUrl("admin.stats.new.min.css").SetDependencies("Club.Base");
            manifest.DefineStyle("Admin.Report.New").SetUrl("admin.report.new.min.css").SetDependencies("Club.Base");

            manifest.DefineStyle("Club.Configuration.General").SetUrl("club.configuration.general.min.css").SetDependencies("Club.Base");
            manifest.DefineStyle("Club.Configuration.Fixture").SetUrl("club.configuration.fixture.min.css").SetDependencies("Club.Base");

            manifest.DefineScript("CKEditor").SetUrl("ckeditor/ckeditor.js").SetDependencies("jQueryUI");
            manifest.DefineScript("AngularSanitize").SetUrl("angular-sanitize.js").SetDependencies("AngularJS");
            manifest.DefineScript("NgCsv").SetUrl("ng-csv.min.js").SetDependencies("AngularSanitize");

            manifest.DefineScript("Membership.New.Fee").SetUrl("app/membership.new.fee.js").SetVersion("1.0").SetDependencies("trNgGrid");
            manifest.DefineScript("Membership.New.Registration").SetUrl("app/membership.new.registration.js").SetVersion("1.0").SetDependencies("AngularJS");
            manifest.DefineScript("Membership.My.Registration").SetUrl("app/membership.my.registration.js").SetVersion("1.0").SetDependencies("trNgGrid");

            manifest.DefineScript("Admin.Member.Activate").SetUrl("app/admin.member.activate.js").SetVersion("1.0").SetDependencies("trNgGrid");
            manifest.DefineScript("Admin.Member.List.All").SetUrl("app/admin.member.list.all.js").SetVersion("1.0").SetDependencies("trNgGrid");
            manifest.DefineScript("Admin.Member.List.Contacts").SetUrl("app/admin.member.list.contacts.js").SetVersion("1.0").SetDependencies("trNgGrid");

            manifest.DefineScript("Admin.Fixture.List.All").SetUrl("app/admin.fixture.list.all.js").SetVersion("1.0").SetDependencies("trNgGrid"); 
            manifest.DefineScript("Admin.Fixture.New").SetUrl("app/admin.fixture.new.js").SetVersion("1.0").SetDependencies("trNgGrid");
            manifest.DefineScript("Admin.Team.New").SetUrl("app/admin.team.new.js").SetVersion("1.0").SetDependencies("trNgGrid");
            manifest.DefineScript("Admin.Stats.New").SetUrl("app/admin.stats.new.js").SetVersion("1.0").SetDependencies("trNgGrid");
            manifest.DefineScript("Admin.Report.New").SetUrl("app/admin.report.new.js").SetVersion("1.0").SetDependencies("trNgGrid");

            manifest.DefineScript("Club.Configuration.General").SetUrl("app/club.configuration.general.js").SetVersion("1.0").SetDependencies("trNgGrid");
            manifest.DefineScript("Club.Configuration.Fixture").SetUrl("app/club.configuration.fixture.js").SetVersion("1.0").SetDependencies("trNgGrid");

            manifest.DefineScript("UI.Bootstrap").SetUrl("ui-bootstrap-tpls-0.12.0.js").SetVersion("0.12.0").SetDependencies("AngularJS");


            #endregion

            manifest.DefineStyle("trNgGrid").SetUrl("trNgGrid.min.css").SetDependencies("Bootstrap");
            manifest.DefineStyle("Club.New.Member").SetUrl("new.member.min.css");

            manifest.DefineScript("trNgGrid").SetUrl("trNgGrid.min.js").SetVersion("1.2.9").SetDependencies("AngularJS");           

            manifest.DefineScript("AngularJS").SetUrl("anjular.min.js").SetVersion("1.2.9").SetDependencies("jQueryUI");
            manifest.DefineScript("AngularJS-Resource").SetUrl("angular-resource.min.js").SetVersion("1.2.18").SetDependencies("AngularJS");
            manifest.DefineScript("AngularJS-UI").SetUrl("angular-ui-utils.min.js").SetVersion("0.1.1").SetDependencies("AngularJS");
            manifest.DefineScript("AngularJS-Autocomplete").SetUrl("angular-autocomplete.js").SetVersion("0.1.1").SetDependencies("AngularJS");

            manifest.DefineScript("Club.New.Member").SetUrl("app/new.member.js").SetVersion("1.0").SetDependencies("AngularJS");


            manifest.DefineScript("Club.Member.Registration.Details").SetUrl("app/member.registration.details.js").SetVersion("1.0").SetDependencies("trNgGrid");


            manifest.DefineScript("Club.Admin.Member.List").SetUrl("app/admin.member.list.js").SetVersion("1.0").SetDependencies("trNgGrid");

        }
    }
}

