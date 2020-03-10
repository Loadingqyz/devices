using Equipment.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Equipment.Models.User
{
	public class UserEntity: BaseEntity
	{
		public string UserName { get; set; }
		public string Password { get; set; }
		public string Phone { get; set; }
		public int IsSuperAdmin { get; set; }
	}
}
