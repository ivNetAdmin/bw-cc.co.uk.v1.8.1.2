using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;

namespace ivNet.Club
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
            rdl.AddRange(DocumentationRoutes());
            rdl.AddRange(MembershipRoutes());
            rdl.AddRange(ConfigurationRoutes());
            return rdl;
        }

        #region documentation
        private IEnumerable<RouteDescriptor> DocumentationRoutes()
        {
            return new[]
            {
                new RouteDescriptor
                {
                    Route = new Route(
                        "club/user-stories",
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.Club"},
                            {"controller", "Documentation"},
                            {"action", "UserStories"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.Club"}
                        },
                        new MvcRouteHandler())
                }
            };
        }
        #endregion 
        
        #region configuration
        private IEnumerable<RouteDescriptor> ConfigurationRoutes()
        {
            return new[]
            {
                new RouteDescriptor
                {
                    Route = new Route(
                        "club/configuration",
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.Club"},
                            {"controller", "Configuration"},
                            {"action", "Index"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.Club"}
                        },
                        new MvcRouteHandler())
                }, 
                new RouteDescriptor
                {
                    Route = new Route(
                        "club/configuration/new",
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.Club"},
                            {"controller", "Configuration"},
                            {"action", "New"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.Club"}
                        },
                        new MvcRouteHandler())
                }
            };
        }
        #endregion

        #region membership
        private IEnumerable<RouteDescriptor> MembershipRoutes()
        {
            return new[]
            {
                new RouteDescriptor
                {
                    Route = new Route(
                        "club/member/new",
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.Club"},
                            {"controller", "ClubMember"},
                            {"action", "New"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.Club"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor
                {
                    Route = new Route(
                        "club/member/validate",
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.Club"},
                            {"controller", "ClubMember"},
                            {"action", "ValidateCaptcha"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.Club"}
                        },
                        new MvcRouteHandler())
                }
            };
        }
        #endregion
    }
}