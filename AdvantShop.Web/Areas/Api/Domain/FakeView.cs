using System;
using System.IO;
using System.Web.Mvc;

namespace AdvantShop.Areas.Api.Domain
{
    public class FakeView : IView
    {
        public void Render(ViewContext viewContext, TextWriter writer)
        {
            throw new InvalidOperationException();
        }
    }
}