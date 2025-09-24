//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Web;

namespace AdvantShop.Core.Modules.Interfaces
{
    [Obsolete("use ISmsService")]
    public interface IModuleSms : IModule
    {
        void SendSms(Guid customerId, long phoneNumber, string text);

        /// <summary>
        /// for old admin area
        /// </summary>
        string RenderSendSmsButton(Guid customerId, long phoneNumber);

        [Obsolete("use new admin modal ModalSendSmsAdvCtrl")]
        IHtmlString GetSendSmsButton(Guid customerId, long phoneNumber, string ngPhone, string ngOnClose = null);

        [Obsolete("use new admin modal ModalSendSmsAdvCtrl")]
        IHtmlString GetSendSmsButton(Guid customerId, long phoneNumber, int orderid, string ngPhone, string ngOnClose = null);

        [Obsolete("use new admin modal ModalSendSmsAdvCtrl")]
        IHtmlString GetSendSmsButton(Dictionary<string, object> modalParams, string ngOnClose = null);
    }
}