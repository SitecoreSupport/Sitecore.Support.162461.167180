using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using Sitecore.EmailCampaign.Server.Controllers;
using Sitecore.Services.Core;
using Sitecore.Support.ExM.Framework.Formatters;

namespace Sitecore.Support.EmailCampaign.Server.Controllers.Localization
{
  [ServicesController("EXM.DateTimeInfo")]
  public class DateTimeInfoController : ServicesApiControllerBase
  {
    [ActionName("DefaultAction")]
    public HttpResponseMessage Get()
    {
      SetContextLanguageToClientLanguage();

      string json = JsonConvert.SerializeObject(new DateTimeInfo());
      string response = $"define([], function() {{ return {json}; }});";

      return new HttpResponseMessage(HttpStatusCode.OK)
      {
        Content = new StringContent(response, System.Text.Encoding.UTF8, "text/javascript")
      };
    }
  }
}