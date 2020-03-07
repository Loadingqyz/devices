using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Equipment.Models.User
{
	public class UserLoginModel
	{
		[Required]
		public string Phone { get; set; }
		[Required]
		public string Password { get; set; }
	}
}
