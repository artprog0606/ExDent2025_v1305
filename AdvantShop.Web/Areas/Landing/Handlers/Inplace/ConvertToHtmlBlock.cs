using System;
using System.Net;
using System.Text;
using AdvantShop.App.Landing.Controllers;
using AdvantShop.App.Landing.Extensions;
using AdvantShop.App.Landing.Models.Inplace;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Services.Landing.Blocks;
using AdvantShop.Core.Services.Landing.Forms;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Diagnostics;

namespace AdvantShop.App.Landing.Handlers.Inplace
{
    public class ConvertToHtmlBlock
    {
        private readonly int _blockId;
        private readonly LandingBaseController _controller;
        private readonly LpBlockService _blockService;
        private readonly LpFormService _formService;

        public ConvertToHtmlBlock(int blockId, LandingBaseController controller)
        {
            _blockId = blockId;
            _controller = controller;
            _blockService = new LpBlockService();
            _formService = new LpFormService();
        }

        public bool Execute()
        {
            var block = new LpBlockService().Get(_blockId);
            if (block == null)
                return false;

            try
            {
                var model = new AddBlockModel()
                {
                    LpId = block.LandingId,
                    Name = "html",
                    SortOrder = block.SortOrder
                };

                var result = new AddBlock(model, true, _blockId).Execute();
                if (result.Block == null)
                    throw new BlException("Ошибка при конвертации блока");

                var newBlock = result.Block;

                var wc = new WebClient() {Encoding = Encoding.UTF8};
                wc.Headers.Add(UrlRewriteExtensions.TechnicalHeaderName, SettingsLic.AdvId);

                var html = wc.DownloadString(UrlService.GetUrl("landing/landing/block/" + _blockId + "?convertToHtml=true"));
                var preparedHtml = html.Replace("block_" + _blockId, "block_" + newBlock.Id);


                var form = _formService.GetByBlock(block.Id);
                int oldFormId = 0, newFormId = 0;

                if (form != null)
                {
                    oldFormId = form.Id;

                    form.BlockId = newBlock.Id;
                    form.LpId = newBlock.LandingId;

                    newFormId = _formService.Add(form);
                }

                if (oldFormId != 0 && newFormId != 0)
                {
                    preparedHtml =
                        preparedHtml
                            .Replace("form_" + oldFormId, "form_" + newFormId)
                            .Replace(string.Format("lpForm.init({0}, {1},", oldFormId, block.Id), string.Format("lpForm.init({0}, {1},", newFormId, newBlock.Id));
                }

                newBlock.TrySetSetting("html", preparedHtml);

                _blockService.Update(newBlock);
                _blockService.Delete(_blockId);

                return true;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return false;
        }
    }
}
