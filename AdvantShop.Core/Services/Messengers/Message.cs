using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvantShop.Core.Services.Messengers
{
	public class Message
	{
		public Guid CustomerId { get; set; }

		public string Email { get; set; }

		public long Phone { get; set; }

		public string Text { get; set; }
	}
}
