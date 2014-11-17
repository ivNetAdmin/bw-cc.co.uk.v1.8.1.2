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
            rdl.AddRange(AdminDocumentationRoutes());
            rdl.AddRange(MemberRoutes());
            rdl.AddRange(AdminConfigurationRoutes());
            rdl.AddRange(AdminMemberRoutes());
            return rdl;
        }

        #region admin documentation
        private IEnumerable<RouteDescriptor> AdminDocumentationRoutes()
        {
            return new[]
            {
                new RouteDescriptor
                {
                    Route = new Route(
                        "club/admin/user-stories",
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.Club"},
                            {"controller", "AdminDocumentation"},
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
        
        #region admin configuration
        private IEnumerable<RouteDescriptor> AdminConfigurationRoutes()
        {
            return new[]
            {
                new RouteDescriptor
                {
                    Route = new Route(
                        "club/admin/configuration",
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.Club"},
                            {"controller", "AdminConfiguration"},
                            {"action", "Index"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.Club"}
                        },
                        new MvcRouteHandler())
                }, 
                //new RouteDescriptor
                //{
                //    Route = new Route(
                //        "club/admin/configuration/new",
                //        new RouteValueDictionary
                //        {
                //            {"area", "ivNet.Club"},
                //            {"controller", "AdminConfiguration"},
                //            {"action", "New"}
                //        },
                //        new RouteValueDictionary(),
                //        new RouteValueDictionary
                //        {
                //            {"area", "ivNet.Club"}
                //        },
                //        new MvcRouteHandler())
                //}
            };
        }
        #endregion

        #region admin member
        private IEnumerable<RouteDescriptor> AdminMemberRoutes()
        {
            return new[]
            {
                new RouteDescriptor
                {
                    Route = new Route(
                        "club/admin/member/activate",
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.Club"},
                            {"controller", "AdminMember"},
                            {"action", "Activate"}
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
                        "club/admin/member/list",
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.Club"},
                            {"controller", "AdminMember"},
                            {"action", "List"}
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

        #region member
        private IEnumerable<RouteDescriptor> MemberRoutes()
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
                            {"controller", "Member"},
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
                        "club/member/new/fee",
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.Club"},
                            {"controller", "Member"},
                            {"action", "NewFee"}
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
                            {"controller", "Member"},
                            {"action", "ValidateCaptcha"}
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
                        "club/member/registraion",
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.Club"},
                            {"controller", "Member"},
                            {"action", "MemberRegistration"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.Club"}
                        },
                        new MvcRouteHandler())
                },
                
                //new RouteDescriptor
                //{
                //    Route = new Route(
                //        "club/member/duplicates",
                //        new RouteValueDictionary
                //        {
                //            {"area", "ivNet.Club"},
                //            {"controller", "Member"},
                //            {"action", "ValidateDuplicates"}
                //        },
                //        new RouteValueDictionary(),
                //        new RouteValueDictionary
                //        {
                //            {"area", "ivNet.Club"}
                //        },
                //        new MvcRouteHandler())
                //}
            };
        }
        #endregion
    }
}