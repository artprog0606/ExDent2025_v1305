using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Services.Landing.Forms;
using System.Web;

namespace AdvantShop.App.Landing.Models.Landing
{
    public class LpFormModel
    {
        public LpForm Form { get; set; }
        public bool IsVertical { get; internal set; }
        public eFormAlign Align { get; internal set; }
        public string NgModel { get; set; }
        public string ObjId { get; set; }
    }

    public enum eFormAlign
    {
        [StringName("")]
        None = 0,
        [StringName("start-xs")]
        Left = 1,
        [StringName("center-xs")]
        Center = 2,
        [StringName("end-xs")]
        Right = 3
    }
}
