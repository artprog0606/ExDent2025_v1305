using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm.LeadFields;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Landing.Forms
{
    public class LpFormService
    {
        #region Form

        #region CRUD

        public LpForm Get(int id)
        {
            return
                SQLDataAccess.Query<LpForm>("Select * from CMS.LandingForm Where id=@id", new {id})
                    .FirstOrDefault();
        }

        public LpForm GetByBlock(int blockId)
        {
            return
                SQLDataAccess.Query<LpForm>("Select * from CMS.LandingForm Where BlockId=@blockId", new {blockId})
                    .FirstOrDefault();
        }

        public List<LpForm> GetListByLandingPageId(int landingPageId)
        {
            return SQLDataAccess.Query<LpForm>(
                "Select LandingForm.* From CMS.LandingForm " +
                "Inner Join CMS.LandingBlock On LandingBlock.Id = LandingForm.BlockId and LandingBlock.LandingId = @landingPageId",
                new {landingPageId}).ToList();
        }

        public int Add(LpForm form)
        {
            form.Id =
                SQLDataAccess.ExecuteScalar<int>(
                    "Insert Into CMS.LandingForm (LpId,BlockId,Title,SubTitle,ButtonText,PostAction,PostMessageText,PostMessageRedirectUrl,PostMessageRedirectLpId,YaMetrikaEventName,GaEventCategory,GaEventAction,FieldsJson,ShowAgreement,AgreementText,PayProductOfferId,SalesFunnelId,IsHidden,EmailText,EmailSubject,OfferId,OfferPrice,InputTextPosition,DontSendLeadId,OfferItemsJson,ActionUpsellLpId,PostMessageRedirectDelay,PostMessageRedirectShowMessage) " +
                    "Values (@LpId,@BlockId,@Title,@SubTitle,@ButtonText,@PostAction,@PostMessageText,@PostMessageRedirectUrl,@PostMessageRedirectLpId,@YaMetrikaEventName,@GaEventCategory,@GaEventAction,@FieldsJson,@ShowAgreement,@AgreementText,@PayProductOfferId,@SalesFunnelId,@IsHidden,@EmailText,@EmailSubject,@OfferId,@OfferPrice,@InputTextPosition,@DontSendLeadId,@OfferItemsJson,@ActionUpsellLpId,@PostMessageRedirectDelay,@PostMessageRedirectShowMessage); " +
                    "Select scope_identity(); ",
                    CommandType.Text,
                    new SqlParameter("@LpId", form.LpId),
                    new SqlParameter("@BlockId", form.BlockId ?? (object)DBNull.Value),
                    new SqlParameter("@Title", form.Title ?? (object)DBNull.Value),
                    new SqlParameter("@SubTitle", form.SubTitle ?? (object)DBNull.Value),
                    new SqlParameter("@ButtonText", form.ButtonText ?? (object)DBNull.Value),
                    new SqlParameter("@PostAction", (int) form.PostAction),
                    new SqlParameter("@PostMessageText", form.PostMessageText ?? (object)DBNull.Value),
                    new SqlParameter("@PostMessageRedirectUrl", form.PostMessageRedirectUrl ?? (object)DBNull.Value),
                    new SqlParameter("@PostMessageRedirectLpId", form.PostMessageRedirectLpId ?? (object)DBNull.Value),
                    new SqlParameter("@YaMetrikaEventName", form.YaMetrikaEventName ?? (object)DBNull.Value),
                    new SqlParameter("@GaEventCategory", form.GaEventCategory ?? (object)DBNull.Value),
                    new SqlParameter("@GaEventAction", form.GaEventAction ?? (object)DBNull.Value),
                    new SqlParameter("@FieldsJson", JsonConvert.SerializeObject(form.Fields ?? new List<LpFormField>())),
                    new SqlParameter("@ShowAgreement", form.ShowAgreement ?? true),
                    new SqlParameter("@AgreementDefaultChecked", form.AgreementDefaultChecked ?? false),
                    new SqlParameter("@AgreementText", form.AgreementText ?? (object)DBNull.Value),
                    new SqlParameter("@PayProductOfferId", form.PayProductOfferId ?? (object)DBNull.Value),
                    new SqlParameter("@SalesFunnelId", form.SalesFunnelId ?? (object)DBNull.Value),
                    new SqlParameter("@EmailText", form.EmailText ?? (object)DBNull.Value),
                    new SqlParameter("@EmailSubject", form.EmailSubject ?? (object)DBNull.Value),
                    new SqlParameter("@IsHidden", form.IsHidden),
                    new SqlParameter("@OfferId", form.OfferId ?? (object)DBNull.Value),
                    new SqlParameter("@OfferPrice", form.OfferPrice ?? (object)DBNull.Value),
                    new SqlParameter("@InputTextPosition", form.InputTextPosition),
                    new SqlParameter("@DontSendLeadId", form.DontSendLeadId ?? false),
                    new SqlParameter("@OfferItemsJson", JsonConvert.SerializeObject(form.OfferItems ?? new List<LpFormOfferItem>())),
                    new SqlParameter("@ActionUpsellLpId", form.ActionUpsellLpId ?? (object)DBNull.Value),
                    new SqlParameter("@PostMessageRedirectDelay", form.PostMessageRedirectDelay ?? (object)DBNull.Value),
                    new SqlParameter("@PostMessageRedirectShowMessage", form.PostMessageRedirectShowMessage ?? (object)DBNull.Value)
                    );

            return form.Id;
        }

        public void Update(LpForm form)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update CMS.LandingForm " +
                "Set BlockId=@BlockId, Title=@Title, SubTitle = @SubTitle, ButtonText=@ButtonText, PostAction=@PostAction, PostMessageText=@PostMessageText, " +
                "PostMessageRedirectUrl=@PostMessageRedirectUrl, PostMessageRedirectLpId=@PostMessageRedirectLpId, YaMetrikaEventName=@YaMetrikaEventName, GaEventCategory=@GaEventCategory, " +
                "GaEventAction=@GaEventAction, FieldsJson=@FieldsJson, ShowAgreement=@ShowAgreement, AgreementDefaultChecked=@AgreementDefaultChecked, AgreementText=@AgreementText," +
                "PayProductOfferId=@PayProductOfferId, SalesFunnelId=@SalesFunnelId, IsHidden=@IsHidden, EmailText=@EmailText, EmailSubject=@EmailSubject, OfferId=@OfferId, OfferPrice=@OfferPrice, " +
                "InputTextPosition = @InputTextPosition, DontSendLeadId=@DontSendLeadId, OfferItemsJson=@OfferItemsJson, ActionUpsellLpId=@ActionUpsellLpId, " +
                "PostMessageRedirectDelay=@PostMessageRedirectDelay, PostMessageRedirectShowMessage=@PostMessageRedirectShowMessage " +
                "Where Id=@Id",
                CommandType.Text,
                new SqlParameter("@Id", form.Id),
                new SqlParameter("@BlockId", form.BlockId ?? (object)DBNull.Value),
                new SqlParameter("@Title", form.Title ?? (object)DBNull.Value),
                new SqlParameter("@SubTitle", form.SubTitle ?? (object)DBNull.Value),
                new SqlParameter("@ButtonText", form.ButtonText ?? (object)DBNull.Value),
                new SqlParameter("@PostAction", (int)form.PostAction),
                new SqlParameter("@PostMessageText", form.PostMessageText ?? (object)DBNull.Value),
                new SqlParameter("@PostMessageRedirectUrl", form.PostMessageRedirectUrl ?? (object)DBNull.Value),
                new SqlParameter("@PostMessageRedirectLpId", form.PostMessageRedirectLpId ?? (object)DBNull.Value),
                new SqlParameter("@YaMetrikaEventName", form.YaMetrikaEventName ?? (object)DBNull.Value),
                new SqlParameter("@GaEventCategory", form.GaEventCategory ?? (object)DBNull.Value),
                new SqlParameter("@GaEventAction", form.GaEventAction ?? (object)DBNull.Value),
                new SqlParameter("@FieldsJson", JsonConvert.SerializeObject(form.Fields ?? new List<LpFormField>())),
                new SqlParameter("@ShowAgreement", form.ShowAgreement ?? true),
                new SqlParameter("@AgreementDefaultChecked", form.AgreementDefaultChecked ?? false),
                new SqlParameter("@AgreementText", form.AgreementText ?? (object)DBNull.Value),
                new SqlParameter("@PayProductOfferId", form.PayProductOfferId ?? (object)DBNull.Value),
                new SqlParameter("@SalesFunnelId", form.SalesFunnelId ?? (object)DBNull.Value),
                new SqlParameter("@EmailText", form.EmailText ?? (object)DBNull.Value),
                new SqlParameter("@EmailSubject", form.EmailSubject ?? (object)DBNull.Value),
                new SqlParameter("@IsHidden", form.IsHidden),
                new SqlParameter("@OfferId", form.OfferId ?? (object)DBNull.Value),
                new SqlParameter("@OfferPrice", form.OfferPrice ?? (object)DBNull.Value),
                new SqlParameter("@InputTextPosition", form.InputTextPosition),
                new SqlParameter("@DontSendLeadId", form.DontSendLeadId ?? false),
                new SqlParameter("@OfferItemsJson", JsonConvert.SerializeObject(form.OfferItems ?? new List<LpFormOfferItem>())),
                new SqlParameter("@ActionUpsellLpId", form.ActionUpsellLpId ?? (object)DBNull.Value),
                new SqlParameter("@PostMessageRedirectDelay", form.PostMessageRedirectDelay ?? (object)DBNull.Value),
                new SqlParameter("@PostMessageRedirectShowMessage", form.PostMessageRedirectShowMessage ?? (object)DBNull.Value)
                );
        }

        #endregion

        public LpForm GetDefaultForm()
        {
            return new LpForm()
            {
                Title = "Оставьте заявку",
                ButtonText = "Отправить",
                Fields = new List<LpFormField>()
                {
                    new LpFormField() {Title = "Имя", TitleCrm = "Имя", FieldType = ELpFormFieldType.FirstName, Required = true},
                    new LpFormField() {Title = "Телефон", TitleCrm = "Телефон", FieldType = ELpFormFieldType.Phone, Required = true},
                    new LpFormField() {Title = "Email", TitleCrm = "Email", FieldType = ELpFormFieldType.Email, Required = true},
                },
                PostAction = FormPostAction.ShowMessage,
                PostMessageText = "Спасибо за заявку! С Вами свяжется наш менеджер!",
                ShowAgreement = true,
                AgreementText = "Я согласен на обработку персональных данных",
            };
        }

        public LpForm AddAndGetDefaultForm(int? blockId, int lpId)
        {
            var form = GetDefaultForm();

            form.BlockId = blockId;
            form.LpId = lpId;

            Add(form);

            return form;
        }

        public void DeleteByBlock(int blockId)
        {
            SQLDataAccess.ExecuteNonQuery("Delete From CMS.LandingForm Where BlockId=@BlockId", CommandType.Text,
                new SqlParameter("@BlockId", blockId));
        }

        #endregion

        #region Form Fields

        public List<LpFormField> GetAllFieldsList(int? salesFunnelId = null, bool ignoreLeadFields = false)
        {
            var fields = 
                Enum.GetValues(typeof(ELpFormFieldType)).Cast<ELpFormFieldType>()
                .Where(x => x != ELpFormFieldType.None && x != ELpFormFieldType.CustomerField && x != ELpFormFieldType.LeadField)
                .Select(x => new LpFormField()
                {
                    Title = x.Localize(),
                    TitleCrm = x.Localize(),
                    FieldType = x,
                }).ToList();

            var customerFields = CustomerFieldService.GetCustomerFields().Select(x => new LpFormField()
            {
                Title = x.Name,
                TitleCrm = x.Name,
                FieldType = ELpFormFieldType.CustomerField,
                CustomFieldId = x.Id,
            });

            fields.AddRange(customerFields);

            if (!ignoreLeadFields)
            {
                var leadFields = LeadFieldService.GetLeadFields(salesFunnelId).Select(x => new LpFormField()
                {
                    Title = x.Name,
                    TitleCrm = x.Name,
                    FieldType = ELpFormFieldType.LeadField,
                    CustomFieldId = x.Id,
                });

                fields.AddRange(leadFields);
            }

            return fields;
        }

        #endregion

    }
}
