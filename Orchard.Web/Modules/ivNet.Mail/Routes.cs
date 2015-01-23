
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;

namespace ivNet.Mail
{
    public class Routes : IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            var rdl = new List<RouteDescriptor>();         
            rdl.AddRange(SiteRoutes());
            return rdl;
        }

        #region site
        private IEnumerable<RouteDescriptor> SiteRoutes()
        {
            return new[]
            {
                new RouteDescriptor
                {
                    Route = new Route(
                        "mail/send",
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.Mail"},
                            {"controller", "Mail"},
                            {"action", "Send"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.Mail"}
                        },
                        new MvcRouteHandler())
                }
            };
        }
        #endregion
    }
}