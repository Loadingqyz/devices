using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Equipment.Models.User
{
	public class UserLoginResultModel
	{
		public string UserId { get; set; }
		public string UserName { get; set; }
		public string AuthInfo { get; set; }
	}
}
