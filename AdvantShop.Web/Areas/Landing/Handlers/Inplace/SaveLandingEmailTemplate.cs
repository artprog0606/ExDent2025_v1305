using System.Linq;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Landing.LandingEmails;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.App.Landing.Handlers.Inplace
{
    public class SaveLandingEmailTemplate : ICommandHandler<bool>
    {
        #region Ctor

        private readonly LandingEmailTemplate _model;
        private readonly LandingEmailTemplateService _service;

        public SaveLandingEmailTemplate(LandingEmailTemplate model)
        {
            _model = model;
            _service = new LandingEmailTemplateService();
        }

        #endregion
        
        public bool Execute()
        {
            if (string.IsNullOrWhiteSpace(_model.Subject) || string.IsNullOrWhiteSpace(_model.Body))
                throw new BlException("Укажите заголовок и текст письма");

            if (_model.BlockId == 0)
                throw new BlException("Не найден блок");

            if (_model.SendingTime <= 0)
                throw new BlException("Укажите через сколько минут отсылать письмо");

            var template = new LandingEmailTemplate()
            {
                Id = _model.Id,
                BlockId = _model.BlockId,
                Subject = _model.Subject.DefaultOrEmpty(),
                Body = _model.Body,
                SendingTime = _model.SendingTime
            };

            if (_model.Id == 0)
            {
                var templates = _service.GetList(_model.BlockId);
                if (templates.Any(x=> x.SendingTime == _model.SendingTime))
                    throw new BlException("Уже существует письмо в назначенное время. Пожалуйста измените время.");

                _service.Add(template);
            }
            else
            {
                _service.Update(template);
            }

            return true;
        }
    }
}
