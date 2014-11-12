
using System.Collections.Generic;
using ivNet.Club.Helpers;
using ivNet.Club.Services;
using ivNet.Club.ViewModel;
using Orchard.Logging;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ivNet.Club.Controllers.Api
{
    public class ConfigurationController : ApiController
    {
        private readonly IConfigurationServices _configurationServices;

        public ConfigurationController(IConfigurationServices configurationServices)
        {
            _configurationServices = configurationServices;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public HttpResponseMessage Get()
        {
            try
            {
                var configurationItemList = _configurationServices.Get();

                var returnList = (from configurationItem in configurationItemList 
                                  let configurationItemViewModel = new ConfigurationItemViewModel() 
                                  select MapperHelper.Map(configurationItemViewModel, configurationItem)).ToList();              

                returnList.Insert(0,new ConfigurationItemViewModel());

                return Request.CreateResponse(HttpStatusCode.OK,
                    returnList);
            }
            catch (Exception ex)
            {
                var errorId = Guid.NewGuid();
                Logger.Error(string.Format("{0}: {1}{2} [{3}]", Request.RequestUri, ex.Message,
                    ex.InnerException == null ? string.Empty : string.Format(" - {0}", ex.InnerException), errorId));

                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    "An Error has occurred. Report to bp@ivnet.co.uk quoting: " + errorId);
              
            }
        }

        public HttpResponseMessage Get(string id)
        {
            try
            {
                switch (id)
                {
                    case "seasons":
                        return GetSeasons();                  
                    case "extrareg":
                        return GetExtraFeeData();   
                    default:
                        return Get();
                }

            }
            catch (Exception ex)
            {
                var errorId = Guid.NewGuid();
                Logger.Error(string.Format("{0}: {1}{2} [{3}]", Request.RequestUri, ex.Message,
                    ex.InnerException == null ? string.Empty : string.Format(" - {0}", ex.InnerException), errorId));

                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    "An Error has occurred. Report to bp@ivnet.co.uk quoting: " + errorId);

            }
        }

        private HttpResponseMessage GetSeasons()
        {
            //var currentSeason = _configurationServices.GetCurrentSeason();
            var seasonFormat = _configurationServices.GetRegistrationSeasonList();  
         
           return Request.CreateResponse(HttpStatusCode.OK,
                seasonFormat);
        }

        private HttpResponseMessage GetExtraFeeData()
        {
            var configurationItemList = _configurationServices.GetExtraFeeData();

            var returnList = (from configurationItem in configurationItemList
                              let configurationItemViewModel = new ConfigurationItemViewModel()
                              select MapperHelper.Map(configurationItemViewModel, configurationItem)).ToList();

            return Request.CreateResponse(HttpStatusCode.OK,
                returnList);
        }

        [HttpPut]
        public HttpResponseMessage Put(int id, ConfigurationItemViewModel item)
        {

            try
            {                
                _configurationServices.Save(id, item);

                return Request.CreateResponse(HttpStatusCode.OK,
                    "Success");
            }
            catch (Exception ex)
            {
                var errorId = Guid.NewGuid();
                Logger.Error(string.Format("{0}: {1}{2} [{3}]", Request.RequestUri, ex.Message,
                    ex.InnerException == null ? string.Empty : string.Format(" - {0}", ex.InnerException), errorId));              

                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    "An Error has occurred. Report to bp@ivnet.co.uk quoting: " + errorId);

            }
        }
    }
}