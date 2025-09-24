using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using AdvantShop.Web.Admin.Handlers.Settings.System;

namespace Tools.core
{
    public partial class SystemNetLoggingPage : Page
    {
        protected void Page_PreRender(object sender, EventArgs e)
        {
            btnActivateOneMinute.Visible = !SystemNetLogging.IsActivated;
            btnActivateThreeMinute.Visible = !SystemNetLogging.IsActivated;
            btnActivateFiveMinute.Visible = !SystemNetLogging.IsActivated;

            btnDeactivateLoggin.Visible = SystemNetLogging.IsActivated;

            LogRepeater.DataSource = SystemNetLogging.GetLogFiles();
            LogRepeater.DataBind();
        }

        protected void btnActivateOneMinute_Click(object sender, EventArgs e)
        {
            ActivateSystemNetLog(1);
        }

        protected void btnActivateThreeMinute_Click(object sender, EventArgs e)
        {
            ActivateSystemNetLog(3);
        }

        protected void btnActivateFiveMinute_Click(object sender, EventArgs e)
        {
            ActivateSystemNetLog(5);
        }

        private void ActivateSystemNetLog(byte? forMinutes)
        {
            if (!SystemNetLogging.IsActivated)
            {
                SystemNetLogging.Activate();

                if (forMinutes.HasValue)
                    Task.Factory.StartNew(async () =>
                    {
                        await Task.Delay(forMinutes.Value * 1000 * 60);
                        SystemNetLogging.Deactivate();
                        return true;
                    });
            }
        }

        protected void btnDeactivateLoggin_Click(object sender, EventArgs e)
        {
            if (SystemNetLogging.IsActivated)
                SystemNetLogging.Deactivate();
        }

        protected void LogRepeater_OnItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                SystemNetLogging.DeleteLogFile(e.CommandArgument.ToString());
            }
            else if (e.CommandName == "Download")
            {
                var filePath = Path.Combine(SystemNetLogging.PathFiles, e.CommandArgument.ToString());
                var file = new FileInfo(filePath);
                Response.Clear();
                Response.Charset = "utf-8";
                Response.ContentType = "application/octet-stream";
                Response.AddHeader("Connection", "Keep-Alive");
                Response.AddHeader("Content-Length", file.Length.ToString());
                Response.AddHeader("content-disposition", string.Format("attachment; filename={0}", file.Name));
                Response.WriteFile(filePath);
                Response.Flush();

            }
        }
    }
}