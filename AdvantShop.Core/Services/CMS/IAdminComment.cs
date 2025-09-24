using System;
using System.Collections.Generic;
using AdvantShop.Customers;
using AdvantShop.Core.Services.Attachments;
namespace AdvantShop.CMS
{
    public interface IAdminComment
    {
        int Id { get; set; }
        int? ParentId { get; set; }

        int ObjId { get; set; }

        AdminCommentType Type { get; set; }

        Guid? CustomerId { get; set; }
        string Name { get; set; }
        string Email { get; set; }
        string Avatar { get; set; }

        string Text { get; set; }

        DateTime DateCreated { get; set; }
        DateTime DateModified { get; set; }

        string ObjUrl { get; set; }

        Customer Customer { get; }



    }


}