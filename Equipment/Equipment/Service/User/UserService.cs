using Equipment.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Equipment.Service.User
{
	public class UserService
	{
		public UserEntity CheckUserLogin(UserLoginModel userLoginModel)
		{
			UserEntity user = MySqlContext.QueryFirstOrDefault<UserEntity>($"select * from `ttl`.`ttl_user` where `Phone` =@phone and `Password`=@password and isdelete=0;", userLoginModel);
			return user;
		}

		public UserEntity GetUserById(string userId)
		{
			UserEntity user = MySqlContext.QueryFirstOrDefault<UserEntity>($"select * from `ttl`.`ttl_user` where `Id` ={userId} and isdelete=0;");
			return user;
		}

		public List<UserEntity> GetUserList()
		{
			return MySqlContext.Query<UserEntity>($"select * from `ttl`.`ttl_user` where  isdelete=0;").ToList();
		}

		public int AddUser(UserEntity userEntity)
		{
			return MySqlContext.Execute("INSERT INTO `ttl`.`ttl_user` (`UserName`,`Password`,`Phone`) VALUES(@UserName,@Password,@Phone); ", userEntity);
		}

		public int DeleteUser(string userId)
		{
			return MySqlContext.Execute($"UPDATE `ttl`.`ttl_user` SET `IsDelete` =1 where `Id`={userId}");
		}
	}
}
