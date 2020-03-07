using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Equipment.Models.User
{
	public class UserEntity
	{
		public long Id { get; set; }
		public DateTime Tdate { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
		public string Phone { get; set; }
	}
}
