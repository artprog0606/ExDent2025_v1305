using System;
using System.Collections.Generic;

namespace AdvantShop.Core.Services.Landing.Reviews
{
    public class LpReview : IConvertibleBlockModel
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public bool Enabled { get; set; }

        public string Text { get; set; }

        public int? Rating { get; set; }

        public DateTime? Date { get; set; }

        public List<LpReview> Childs { get; set; }

        public IConvertibleBlockModel ConvertFromType(object obj, Type type)
        {
            throw new NotImplementedException();
        }

        public IConvertibleBlockModel ConvertToType(Type type)
        {
            throw new NotImplementedException();
        }

        public bool IsNull()
        {
            throw new NotImplementedException();
        }
    }
}
