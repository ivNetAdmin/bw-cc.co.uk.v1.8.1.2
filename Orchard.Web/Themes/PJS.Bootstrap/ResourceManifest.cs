using Orchard.UI.Resources;

namespace PJS.Bootstrap {
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            var manifest = builder.Add();

            manifest.DefineScript("Bootstrap").SetUrl("bootstrap-3.2.0/js/bootstrap.min.js", "bootstrap-3.2.0/js/bootstrap.js").SetVersion("3.2.0").SetDependencies("jQuery");
            manifest.DefineScript("HoverDropdown").SetUrl("hover-dropdown.js").SetDependencies("Bootstrap");
            manifest.DefineScript("Custom").SetUrl("custom.js").SetDependencies("jQuery");
            manifest.DefineScript("Theme-ValidationEngine-en").SetUrl("validationEngine/jquery.validationEngine-en.js").SetVersion("2.6.2").SetDependencies("jQuery");
            manifest.DefineScript("Theme-ValidationEngine").SetUrl("validationEngine/jquery.validationEngine.js").SetVersion("2.6.2").SetDependencies("ValidationEngine-en");
            manifest.DefineScript("Slider-Revolution-Plugins").SetUrl("rs-plugin/jquery.themepunch.plugins.min.js").SetVersion("4.5.6").SetDependencies("jQuery");
            manifest.DefineScript("Slider-Revolution").SetUrl("rs-plugin/jquery.themepunch.revolution.min.js").SetVersion("4.5.6").SetDependencies("Slider-Revolution-Plugins");
            manifest.DefineScript("Isotope").SetUrl("isotope/jquery.isotope.min.js").SetVersion("1.5.25").SetDependencies("jQuery");
            manifest.DefineScript("prettyPhoto").SetUrl("prettyPhoto/jquery.prettyPhoto.min.js").SetVersion("3.1.5").SetDependencies("jQuery");
            manifest.DefineScript("Styler").SetUrl("jquery.styler.js").SetDependencies("jQuery");

            manifest.DefineStyle("prettyPhoto").SetUrl("prettyPhoto/prettyPhoto.css");
            manifest.DefineStyle("Slider-Revolution-Settings").SetUrl("rs-plugin/css/settings.css").SetVersion("4.1.1");
            manifest.DefineStyle("Slider-Revolution-Dynamic-Captions").SetUrl("rs-plugin/css/dynamic-captions.css").SetVersion("4.1.1").SetDependencies("Slider-Revolution-Settings");
            manifest.DefineStyle("Slider-Revolution-Static-Captions").SetUrl("rs-plugin/css/static-captions.css").SetVersion("4.1.1").SetDependencies("Slider-Revolution-Settings");
        }
    }
}
