using System.Linq;
using AdvantShop.CMS;
using AdvantShop.Core.Services.Customers.AdminInformers;
using AdvantShop.Web.Admin.Models.Shared.AdminComments;

namespace AdvantShop.Web.Admin.Handlers.Shared.AdminComments
{
    public class GetAdminCommentsHandler
    {
        private int _objId;
        private AdminCommentType _type;

        public GetAdminCommentsHandler(int objId, AdminCommentType type)
        {
            _objId = objId;
            _type = type;
        }

        public AdminCommentsListModel Execute()
        {
            var model = new AdminCommentsListModel();

            var comments = AdminCommentService.GetAdminComments(_objId, _type);

            model.Comments = comments.Where(x => !x.Deleted).Select(x => (AdminCommentModel)x).ToList();

            foreach (var comment in model.Comments.Where(x => x.ParentId.HasValue))
            {
                var parent = comments.FirstOrDefault(x => x.Id == comment.ParentId.Value);
                if (parent != null)
                    comment.ParentComment = (AdminCommentModel)parent;
            }

            if (comments.Count > 0)
            {
                var events =
                    AdminInformerService.GetList()
                        .Where(x => !x.Seen && 
                                    x.EntityId == _objId && x.Type == AdminInformerType.Comment && 
                                    comments.Find(c => c.Id == x.ObjId) != null);

                foreach (var item in events)
                    AdminInformerService.SetSeen(item.Id);
            }

            return model;
        }
    }
}
