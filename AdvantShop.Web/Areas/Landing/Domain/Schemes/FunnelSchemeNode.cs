using System.Collections.Generic;

namespace AdvantShop.App.Landing.Domain.Schemes
{
    public enum FunnelSchemeType
    {
        Page,
        ThanksPage,
        Trigger,
        TriggerDelay,
        LinkedNode,
        LinkedButton,
        Mail,
        Sms,
    }

    public class FunnelSchemeNode
    {
        public string Title { get; set; }
        public string Name { get; set; }

        public string Content { get; set; }

        public bool IsMain { get; set; }

        public FunnelSchemeType Type { get; set; }

        public string TypeStr { get { return Type.ToString().ToLower(); } }

        private string _id;

        public string Id
        {
            get
            {
                if (_id == null)
                    _id = (Name + Title + Content).GetHashCode().ToString();
                return _id + TypeStr;
            }
            set { _id = value; }
        }

        public List<FunnelSchemeNode> Nodes { get; set; }


        public int Level { get; set; }
        public int SubLevel { get; set; }

        public bool NeedLinkToThanksPage { get; set; }


        public FunnelSchemeNode()
        {
            Nodes = new List<FunnelSchemeNode>();
        }
    }

    public class FunnelSchemeEdge
    {
        public int Id { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }
    }

    public class FunnelSchemeResult
    {
        public List<FunnelSchemeNode> Nodes { get; set; }
        public List<FunnelSchemeEdge> Edges { get; set; }
    }
}
