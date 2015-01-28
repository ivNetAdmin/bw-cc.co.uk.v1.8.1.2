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
            //rdl.AddRange(AdminDocumentationRoutes());
            //rdl.AddRange(MemberRoutes());
            //rdl.AddRange(AdminConfigurationRoutes());
            //rdl.AddRange(AdminMemberRoutes());

            //rewok
            rdl.AddRange(SiteRoutes());
            rdl.AddRange(SecureSiteRoutes());
            rdl.AddRange(AdminSiteMemberRoutes());
            return rdl;
        }

        //#region admin documentation
        //private IEnumerable<RouteDescriptor> AdminDocumentationRoutes()
        //{
        //    return new[]
        //    {
        //        new RouteDescriptor
        //        {
        //            Route = new Route(
        //                "club/admin/user-stories",
        //                new RouteValueDictionary
        //                {
        //                    {"area", "ivNet.Club"},
        //                    {"controller", "AdminDocumentation"},
        //                    {"action", "UserStories"}
        //                },
        //                new RouteValueDictionary(),
        //                new RouteValueDictionary
        //                {
        //                    {"area", "ivNet.Club"}
        //                },
        //                new MvcRouteHandler())
        //        }
        //    };
        //}
        //#endregion 

        //#region admin configuration
        //private IEnumerable<RouteDescriptor> AdminConfigurationRoutes()
        //{
        //    return new[]
        //    {
        //        new RouteDescriptor
        //        {
        //            Route = new Route(
        //                "club/admin/configuration",
        //                new RouteValueDictionary
        //                {
        //                    {"area", "ivNet.Club"},
        //                    {"controller", "AdminConfiguration"},
        //                    {"action", "Index"}
        //                },
        //                new RouteValueDictionary(),
        //                new RouteValueDictionary
        //                {
        //                    {"area", "ivNet.Club"}
        //                },
        //                new MvcRouteHandler())
        //        }, 
        //        //new RouteDescriptor
        //        //{
        //        //    Route = new Route(
        //        //        "club/admin/configuration/new",
        //        //        new RouteValueDictionary
        //        //        {
        //        //            {"area", "ivNet.Club"},
        //        //            {"controller", "AdminConfiguration"},
        //        //            {"action", "New"}
        //        //        },
        //        //        new RouteValueDictionary(),
        //        //        new RouteValueDictionary
        //        //        {
        //        //            {"area", "ivNet.Club"}
        //        //        },
        //        //        new MvcRouteHandler())
        //        //}
        //    };
        //}
        //#endregion

        //#region admin member
        //private IEnumerable<RouteDescriptor> AdminMemberRoutes()
        //{
        //    return new[]
        //    {
        //        //new RouteDescriptor
        //        //{
        //        //    Route = new Route(
        //        //        "club/admin/member/activate-new",
        //        //        new RouteValueDictionary
        //        //        {
        //        //            {"area", "ivNet.Club"},
        //        //            {"controller", "ClubAdmin"},
        //        //            {"action", "ActivateNewMembers"}
        //        //        },
        //        //        new RouteValueDictionary(),
        //        //        new RouteValueDictionary
        //        //        {
        //        //            {"area", "ivNet.Club"}
        //        //        },
        //        //        new MvcRouteHandler())
        //        //},
        //        //new RouteDescriptor
        //        //{
        //        //    Route = new Route(
        //        //        "club/admin/member/list",
        //        //        new RouteValueDictionary
        //        //        {
        //        //            {"area", "ivNet.Club"},
        //        //            {"controller", "AdminMember"},
        //        //            {"action", "List"}
        //        //        },
        //        //        new RouteValueDictionary(),
        //        //        new RouteValueDictionary
        //        //        {
        //        //            {"area", "ivNet.Club"}
        //        //        },
        //        //        new MvcRouteHandler())
        //        //} 
        //    };
        //}
        //#endregion

        //#region member
        //private IEnumerable<RouteDescriptor> MemberRoutes()
        //{
        //    return new[]
        //    {
        //        new RouteDescriptor
        //        {
        //            Route = new Route(
        //                "club/member/new",
        //                new RouteValueDictionary
        //                {
        //                    {"area", "ivNet.Club"},
        //                    {"controller", "Member"},
        //                    {"action", "New"}
        //                },
        //                new RouteValueDictionary(),
        //                new RouteValueDictionary
        //                {
        //                    {"area", "ivNet.Club"}
        //                },
        //                new MvcRouteHandler())
        //        },
        //         new RouteDescriptor
        //        {
        //            Route = new Route(
        //                "club/member/new/fee",
        //                new RouteValueDictionary
        //                {
        //                    {"area", "ivNet.Club"},
        //                    {"controller", "Member"},
        //                    {"action", "NewFee"}
        //                },
        //                new RouteValueDictionary(),
        //                new RouteValueDictionary
        //                {
        //                    {"area", "ivNet.Club"}
        //                },
        //                new MvcRouteHandler())
        //        },
        //        new RouteDescriptor
        //        {
        //            Route = new Route(
        //                "club/member/validate",
        //                new RouteValueDictionary
        //                {
        //                    {"area", "ivNet.Club"},
        //                    {"controller", "Member"},
        //                    {"action", "ValidateCaptcha"}
        //                },
        //                new RouteValueDictionary(),
        //                new RouteValueDictionary
        //                {
        //                    {"area", "ivNet.Club"}
        //                },
        //                new MvcRouteHandler())
        //        },
        //        new RouteDescriptor
        //        {
        //            Route = new Route(
        //                "club/member/registraion",
        //                new RouteValueDictionary
        //                {
        //                    {"area", "ivNet.Club"},
        //                    {"controller", "Member"},
        //                    {"action", "MemberRegistration"}
        //                },
        //                new RouteValueDictionary(),
        //                new RouteValueDictionary
        //                {
        //                    {"area", "ivNet.Club"}
        //                },
        //                new MvcRouteHandler())
        //        },

        //        new RouteDescriptor
        //        {
        //            Route = new Route(
        //                "club/member/registraion-details",
        //                new RouteValueDictionary
        //                {
        //                    {"area", "ivNet.Club"},
        //                    {"controller", "Member"},
        //                    {"action", "RegistrationDetails"}
        //                },
        //                new RouteValueDictionary(),
        //                new RouteValueDictionary
        //                {
        //                    {"area", "ivNet.Club"}
        //                },
        //                new MvcRouteHandler())
        //        }
        //    };
        //}
        //#endregion

        #region site
        private IEnumerable<RouteDescriptor> SiteRoutes()
        {
            return new[]
            {
                new RouteDescriptor
                {
                    Route = new Route(
                        "club/membership/new-registration",
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.Club"},
                            {"controller", "Site"},
                            {"action", "NewRegistration"}
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
                        "club/fee/new-member",
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.Club"},
                            {"controller", "Site"},
                            {"action", "NewMemberFee"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.Club"}
                        },
                        new MvcRouteHandler())
                },
            };
        }
        #endregion

        #region secure site
        private IEnumerable<RouteDescriptor> SecureSiteRoutes()
        {
            return new[]
            {
                new RouteDescriptor
                {
                    Route = new Route(
                        "club/member/my-registration",
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.Club"},
                            {"controller", "SecureSite"},
                            {"action", "MyRegistration"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.Club"}
                        },
                        new MvcRouteHandler())
                },
               
               
            };
        }
        #endregion

        #region club admin
        private IEnumerable<RouteDescriptor> AdminSiteMemberRoutes()
        {
            return new[]
            {
                new RouteDescriptor
                {
                    Route = new Route(
                        "club/admin/member/activate-new",
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.Club"},
                            {"controller", "AdminSite"},
                            {"action", "ActivateNewMembers"}
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
                        "club/admin/member/list-all",
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.Club"},
                            {"controller", "AdminSite"},
                            {"action", "ListAllMembers"}
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
                        "club/admin/member/contact-list/{id}",
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.Club"},
                            {"controller", "AdminSite"},
                            {"action", "ListContacts"}
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
                        "club/admin/fixture/new",
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.Club"},
                            {"controller", "AdminSite"},
                            {"action", "CreateNewFixture"}
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
                        "club/admin/fixture/team-selection",
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.Club"},
                            {"controller", "AdminSite"},
                            {"action", "SelectTeamForFixture"}
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
                        "club/admin/fixture/match-stats",
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.Club"},
                            {"controller", "AdminSite"},
                            {"action", "RecordMatchStats"}
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
                        "club/admin/fixture/match-report",
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.Club"},
                            {"controller", "AdminSite"},
                            {"action", "RecordMatchReport"}
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
                        "club/admin/configuration/general",
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.Club"},
                            {"controller", "AdminSite"},
                            {"action", "ConfigurationGeneral"}
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
                        "club/admin/configuration/fixture",
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.Club"},
                            {"controller", "AdminSite"},
                            {"action", "ConfigurationFixture"}
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
                        "club/admin/user-stories",
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.Club"},
                            {"controller", "AdminSite"},
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
    }
}