//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------


using System;
using System.Web;

namespace Tools
{
    public partial class Default : System.Web.UI.Page
    {
        // nothing here...

        protected void Page_Load(object sender, EventArgs e)
        {
            throw new HttpException(404, "Not found");
        }
    }
}