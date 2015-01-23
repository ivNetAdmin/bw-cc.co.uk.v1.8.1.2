
using Recaptcha;
using System;
using System.Net;
using System.Net.Mail;
using System.Web.Configuration;
using System.Web.Mvc;
using Orchard;
using Orchard.Email.Models;
using Orchard.ContentManagement;

namespace ivNet.Mail.Controllers
{
    public class MailController : Controller
    {
        private readonly IOrchardServices _orchardServices;


        public MailController(IOrchardServices orchardServices)
        {
            _orchardServices = orchardServices;
        }

        [HttpPost, RecaptchaControlMvc.CaptchaValidator]
        public ActionResult Send(FormCollection viewModel, bool captchaValid, string captchaErrorMessage)
        {
            try
            {
                if (!captchaValid)
                    ModelState.AddModelError("captcha", captchaErrorMessage);

                if (ModelState.IsValid)
                {

                    SendMessage(viewModel["to"], viewModel["name"], viewModel["email"], "Website Contact eMail",
                        viewModel["message"]);
                    return Redirect("~/contact-success");
                }

                return Redirect("~/contact");
            }
            catch (Exception ex)
            {
                return Redirect(string.Format("~/contact-failed/?error={0}", ex.Message));
            }
        }

        private void SendMessage(string toEmail, string fromName, string fromEmail, string subject, string message)
        {
            var smtpSettings = _orchardServices.WorkContext.CurrentSite.As<SmtpSettingsPart>();

            // can't process emails if the Smtp settings have not yet been set
            if (smtpSettings == null || !smtpSettings.IsValid())
            {
                throw new Exception("Site SMTP settings not configured");
            }

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.UseDefaultCredentials = !smtpSettings.RequireCredentials;
                smtpClient.Credentials = CredentialCache.DefaultNetworkCredentials;

                if (smtpSettings.Host != null)
                    smtpClient.Host = smtpSettings.Host;

                smtpClient.Port = smtpSettings.Port;
                smtpClient.EnableSsl = smtpSettings.EnableSsl;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                var mailMessage = new MailMessage();

                try
                {
                    mailMessage = new MailMessage
                    {
                        Subject = string.Format("{0}", subject),
                        Body = string.Format("{0} [{1}] <br/> {2}", fromName, fromEmail, message),
                        From = new MailAddress(smtpSettings.Address)
                    };
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("ivNet.Mail: Bad eMail message [{0}] - {1}.", fromEmail, ex.Message));                  
                }

                var webConfig =
                WebConfigurationManager.OpenWebConfiguration("~");
                if (webConfig.AppSettings.Settings.Count > 0)
                {
                     var customSetting =
                        webConfig.AppSettings.Settings["ivNetEmail"];
                     
                    if (!string.IsNullOrEmpty(customSetting.Value))
                        mailMessage.Bcc.Add(new MailAddress(customSetting.Value));

                    if (string.IsNullOrEmpty(toEmail))
                    {
                        customSetting =
                            webConfig.AppSettings.Settings["receiverEmail"];

                        if (!string.IsNullOrEmpty(customSetting.Value))
                            mailMessage.To.Add(new MailAddress(customSetting.Value));                      
                    }
                    else
                    {
                        mailMessage.To.Add(new MailAddress(toEmail));
                    }
                }

                mailMessage.IsBodyHtml = mailMessage.Body.Contains("<") && mailMessage.Body.Contains(">");

                try
                {
                    smtpClient.Send(mailMessage);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("ivNet.Mail: Failed to send eMail from [{0}] to [{1}]({2}) - {3}.", mailMessage.From.Address, mailMessage.To[0].Address, mailMessage.To.Count, ex.Message));
                }

            }
        }
    }
}