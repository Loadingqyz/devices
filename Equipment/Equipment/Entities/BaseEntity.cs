using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Equipment.Entities
{
	public class BaseEntity
	{
		public long Id { get; set; }
		public DateTime Tdate { get; set; }
		public long CreateUserId { get; set; }
	}
}
