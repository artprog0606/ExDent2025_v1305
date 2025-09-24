<%@ WebHandler Language="C#" Class="CurrentTemplate" %>

using System.Web;

public class CurrentTemplate : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        context.Response.Write(AdvantShop.Configuration.SettingsDesign.Template);
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}