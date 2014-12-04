
using System.Configuration;
using System.Web.Hosting;
using ivNet.Club.ViewModel;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace ivNet.Club.Controllers.Api
{
    public class CaptchaController : ApiController
    {          
        public HttpResponseMessage Post(CaptchaViewModel viewModel)
        {

            var fileMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename = HostingEnvironment.MapPath("~/Modules/ivNet.Club/web.config")
            };

            var configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            var appsettings = configuration.AppSettings.Settings;

            var cientIP = ((System.Web.HttpContextWrapper)Request.Properties["MS_HttpContext"]).Request.ServerVariables["REMOTE_ADDR"];// Request.GetClientIpAddress();
            var privateKey = appsettings["ReCaptchaPrivateKey"].Value;
        
            var data = string.Format("privatekey={0}&remoteip={1}&challenge={2}&response={3}",
                privateKey, cientIP, viewModel.CaptchaChallenge, viewModel.CaptchaResponse);

            var byteArray = new ASCIIEncoding().GetBytes(data);

            var request = WebRequest.Create("http://www.google.com/recaptcha/api/verify");
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;
            var dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            var response = request.GetResponse();
            var status = (((HttpWebResponse) response).StatusCode);

            dataStream = response.GetResponseStream();
            
            if (dataStream == null) return Request.CreateResponse(HttpStatusCode.InternalServerError);

            var reader = new StreamReader(dataStream);
            var responseFromServer = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();

            var responseLines = responseFromServer.Split(new string[] {"\n"}, StringSplitOptions.None);
            var success = responseLines[0].Equals("true");

            return Request.CreateResponse(status,
                success);
        }
    }
}