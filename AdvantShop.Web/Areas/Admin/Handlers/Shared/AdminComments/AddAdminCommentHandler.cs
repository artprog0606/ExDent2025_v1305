using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.CMS;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Core.Services.Customers.AdminInformers;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Customers;
using AdvantShop.Mails;
using AdvantShop.Orders;
using AdvantShop.Web.Admin.Handlers.Shared.AdminNotifications;
using AdvantShop.Web.Admin.Models.Shared.AdminComments;

namespace AdvantShop.Web.Admin.Handlers.Shared.AdminComments
{
    public class AddAdminCommentHandler
    {
        private readonly AdminCommentModel _commentModel;
        private readonly Customer _author;
        private readonly bool _massAction;
        
        private bool _validationSuccess = true;
        private string _errorReason;

        public AddAdminCommentHandler(AdminCommentModel model, bool massAction = false)
        {
            _commentModel = model;
            _author = CustomerContext.CurrentCustomer;
            _massAction = massAction;
        }

        public AddAdminCommentResult Execute()
        {
            Validate();

            if (!_validationSuccess)
                return new AddAdminCommentResult
                {
                    Result = _validationSuccess,
                    Error = _errorReason,
                };
            
            var comment = new AdminComment
            {
                ParentId = _commentModel.ParentId,
                ObjId = _commentModel.ObjId,
                Type = _commentModel.Type,
                CustomerId = _author.Id,
                Name = string.Join(" ", _author.FirstName, _author.LastName),
                Email = _author.EMail,
                Text = _commentModel.Text,
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now,
                ObjUrl = _commentModel.ObjUrl
            };

            comment.Id = AdminCommentService.AddAdminComment(comment);
            comment.Avatar = _author.Avatar;

            var result = new AddAdminCommentResult
            {
                Result = true,
                Comment = (AdminCommentModel)comment,
            };

            AdminComment parentComment = null;
            if (_commentModel.ParentId.HasValue)
            {
                parentComment = AdminCommentService.GetAdminComment(_commentModel.ParentId.Value);
                result.Comment.ParentComment = (AdminCommentModel)parentComment;
            }

            if (!_massAction)
                ProcessNotifications(comment, parentComment);

            return result;
        }

        private void Validate()
        {
            switch (_commentModel.Type)
            {
                case AdminCommentType.Task:
                    var manager = ManagerService.GetManager(_author.Id);
                    
                    if (manager == null)
                    {
                        _validationSuccess = false;
                        _errorReason = LocalizationService.GetResource("Admin.AdminComments.Add.ValidationError.Task.EmptyManager");
                        return;
                    }
                    
                    return;
            }
        }

        private void ProcessNotifications(AdminComment comment, AdminComment parentComment)
        {
            // уведомление об ответе на комментарий
            AdminNotification parentCommentNotification = null;
            // уведомление о новом комментарии
            AdminNotification commentNotification = null;
            // id сотрудников для отправки уведомлений
            var customerIds = new List<Guid>();

            var emailsDict = new Dictionary<Guid, string>();
            MailTemplate commentMailTpl = null;

            switch (comment.Type)
            {
                case AdminCommentType.Task:
                    {
                        var task = TaskService.GetTask(comment.ObjId);
                        if (task == null)
                            break;
                        if (parentComment != null && CheckAccess(parentComment.Customer, RoleAction.Tasks))
                            parentCommentNotification = new OnTaskCommentAnswerNotification(task, _author, comment);

                        foreach (var manager in task.Managers.Where(x => CheckAccess(x.Customer, RoleAction.Tasks)))
                            customerIds.Add(manager.CustomerId);

                        if (task.AppointedManager != null && CheckAccess(task.AppointedManager.Customer, RoleAction.Tasks))
                            customerIds.Add(task.AppointedManager.CustomerId);

                        if (customerIds.Any())
                            commentNotification = new OnTaskCommentNotification(task, _author, comment);

                        TaskService.OnTaskCommentAdded(comment, task);

                        Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Tasks_CommentAdded);

                        break;
                    }

                case AdminCommentType.TaskHidden:
                    {
                        var task = TaskService.GetTask(comment.ObjId);
                        if (task == null)
                            break;

                        if (parentComment != null && CheckAccess(parentComment.Customer, RoleAction.Tasks))
                            parentCommentNotification = new OnTaskCommentAnswerNotification(task, _author, comment);

                        commentNotification = new OnTaskCommentNotification(task, _author, comment);
                        commentMailTpl = new TaskCommentAddedMailTemplate(task, comment.Name, comment.Text.Replace("\n", "<br/>"));

                        if (task.AppointedManager != null && CheckAccess(task.AppointedManager.Customer, RoleAction.Tasks))
                        {
                            customerIds.Add(task.AppointedManager.CustomerId);
                            emailsDict.TryAddValue(task.AppointedManager.CustomerId, task.AppointedManager.Email);
                        }

                        if (comment.ParentId == null && comment.Customer.IsAdmin && task.ManagerIds != null && task.ManagerIds.Count > 0)
                        {
                            foreach (var taskManager in task.Managers.Where(x => CheckAccess(x.Customer, RoleAction.Tasks)))
                            {
                                customerIds.Add(taskManager.CustomerId);
                                emailsDict.TryAddValue(taskManager.CustomerId, taskManager.Email);
                            }
                        }

                        var watchers = TaskGroupService.GetTaskGroupManagers(task.TaskGroupId, true);

                        if (parentComment != null || !comment.Customer.IsAdmin)
                            watchers = watchers.Where(x => x.Customer != null && x.Customer.IsAdmin).ToList();

                        foreach (var watcher in watchers.Where(x => CheckAccess(x.Customer, RoleAction.Tasks)))
                        {
                            customerIds.Add(watcher.CustomerId);
                            emailsDict.TryAddValue(watcher.CustomerId, watcher.Email);
                        }

                        Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Tasks_CommentAdded);

                        break;
                    }

                case AdminCommentType.Order:
                    var order = OrderService.GetOrder(comment.ObjId);
                    if (order == null)
                        break;

                    if (parentComment != null && CheckAccess(parentComment.Customer, RoleAction.Orders))
                        parentCommentNotification = new OrderCommentAnswerNotification(order, _author, comment);

                    commentMailTpl = new OrderCommentAddedMailTemplate(comment.Name, comment.Text, order.OrderID.ToString(), order.Number);

                    if (order.Manager != null && CheckAccess(order.Manager.Customer, RoleAction.Orders))
                    {
                        customerIds.Add(order.Manager.CustomerId);
                        commentNotification = new OrderCommentNotification(order, _author, comment);

                        if (!emailsDict.ContainsKey(order.Manager.CustomerId))
                            emailsDict.Add(order.Manager.CustomerId, order.Manager.Email);
                    }

                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Orders_AddComment_Discussion);

                    break;

                case AdminCommentType.Customer:
                    var customer = CustomerService.GetCustomer(comment.ObjId);
                    if (customer == null)
                        break;

                    if (parentComment != null && CheckAccess(parentComment.Customer, RoleAction.Customers))
                        parentCommentNotification = new CustomerCommentAnswerNotification(customer, _author, comment);

                    commentMailTpl = new CustomerCommentAddedMailTemplate(comment.Name, comment.Text, customer.Id.ToString(), customer.GetFullName());

                    if (customer.Manager != null && CheckAccess(customer.Manager.Customer, RoleAction.Customers))
                    {
                        customerIds.Add(customer.Manager.CustomerId);
                        commentNotification = new CustomerCommentNotification(customer, _author, comment);

                        if (!emailsDict.ContainsKey(customer.Manager.CustomerId))
                            emailsDict.Add(customer.Manager.CustomerId, customer.Manager.Email);
                    }

                    break;

                case AdminCommentType.Booking:
                    {
                        var booking = BookingService.Get(comment.ObjId);
                        if (booking == null)
                            break;

                        if (parentComment != null && CheckAccess(parentComment.Customer, RoleAction.Booking))
                            parentCommentNotification = new OnBookingCommentAnswerNotification(booking, _author, comment);

                        commentMailTpl = new BookingCommentAddedMailTemplate(booking, comment.Name, comment.Text.Replace("\n", "<br/>"));

                        if (booking.Manager != null && CheckAccess(booking.Manager.Customer, RoleAction.Booking))
                        {
                            customerIds.Add(booking.Manager.CustomerId);
                            commentNotification = new OnBookingCommentNotification(booking, _author, comment);

                            if (!emailsDict.ContainsKey(booking.Manager.CustomerId))
                                emailsDict.Add(booking.Manager.CustomerId, booking.Manager.Email);
                        }

                        break;
                    }

                case AdminCommentType.Lead:
                    {
                        var lead = LeadService.GetLead(comment.ObjId);
                        if (lead == null)
                            break;

                        if (parentComment != null && CheckAccess(parentComment.Customer, RoleAction.Crm))
                            parentCommentNotification = new OnLeadCommentAnswerNotification(lead, _author, comment);

                        commentMailTpl = new LeadCommentAddedMailTemplate(lead, comment.Name, comment.Text.Replace("\n", "<br/>"));
                        commentNotification = new OnLeadCommentNotification(lead, _author, comment);

                        if (lead.ManagerId.HasValue)
                        {
                            var leadManager = ManagerService.GetManager(lead.ManagerId.Value);
                            if (leadManager != null && CheckAccess(leadManager.Customer, RoleAction.Crm))
                            {
                                customerIds.Add(leadManager.CustomerId);

                                if (!emailsDict.ContainsKey(leadManager.CustomerId))
                                    emailsDict.Add(leadManager.CustomerId, leadManager.Email);
                            }
                        }

                        var salesFunnel = SalesFunnelService.Get(lead.SalesFunnelId);
                        if (salesFunnel != null && !salesFunnel.NotSendNotificationsOnLeadChanged) // 
                        {
                            SalesFunnelService.GetSalesFunnelManagers(lead.SalesFunnelId)
                                .ForEach((x) =>
                                    {
                                        if (CheckAccess(x.Customer, RoleAction.Crm))
                                        {
                                            customerIds.Add(x.CustomerId);
                                            if (!emailsDict.ContainsKey(x.CustomerId))
                                                emailsDict.Add(x.CustomerId, x.Email);
                                        }
                                    });
                        }


                        break;
                    }

                default:
                    if (parentComment != null && CheckAccess(parentComment.Customer))
                        parentCommentNotification = new AdminCommentAnswerNotification(comment, _author);
                    break;
            }


            var notificationsHandler = new AdminNotificationsHandler();

            notificationsHandler.UpdateAdminComment(comment, "added");

            if (parentCommentNotification != null && parentComment.Customer.Id != _author.Id)
            {
                notificationsHandler.NotifyCustomers(parentCommentNotification, parentComment.Customer.Id);

                if (!emailsDict.ContainsKey(parentComment.Customer.Id))
                    emailsDict.Add(parentComment.Customer.Id, parentComment.Customer.EMail);

                // не оповещать о комментарии, если отправлено уведомление об ответе на комментарий
                customerIds.RemoveAll(x => x == parentComment.CustomerId.Value);
            }

            if (commentNotification != null)
            {
                notificationsHandler.NotifyCustomers(commentNotification, customerIds.Where(x => x != _author.Id).ToArray());

                foreach (var customerId in customerIds.Distinct().Where(x => x != _author.Id))
                {
                    AdminInformerService.Add(new AdminInformer(AdminInformerType.Comment, comment.Id, null)
                    {
                        EntityId = comment.ObjId,
                        PrivateCustomerId = customerId,
                        Title = commentNotification.Title,
                        Body = comment.Text,
                        Link = commentNotification.Data != null ? ((AdminNotificationData)commentNotification.Data).Url : null
                    });
                }
            }

            if (commentMailTpl != null && emailsDict.Count > 0)
            {
                foreach (var customerId in emailsDict.Keys.Where(x => x != _author.Id))
                {
                    MailService.SendMailNow(customerId, emailsDict[customerId], commentMailTpl);
                }
            }
        }

        private bool CheckAccess(Customer customer, RoleAction roleAction = RoleAction.None)
        {
            return customer != null && customer.Enabled && (roleAction == RoleAction.None || customer.HasRoleAction(roleAction));
        }
    }
}
