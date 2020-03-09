using Equipment.Models.User;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Equipment.Service.User
{
	public class UserService
	{
		public string GenerateAuthInfo(UserEntity userEntity)
		{
			if (userEntity.Id == 0 || string.IsNullOrEmpty(userEntity.Password))
				throw new Exception("Auth异常");
			return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userEntity.Id}:{userEntity.Password}"));
		}
		public UserEntity CheckUserLogin(UserLoginModel userLoginModel)
		{
			UserEntity user = MySqlContext.QueryFirstOrDefault<UserEntity>($"select * from `ttl`.`ttl_user` where `Phone` =@phone and `Password`=@password ;", userLoginModel);
			return user;
		}

		public UserEntity GetUserById(string userId)
		{
			UserEntity user = MySqlContext.QueryFirstOrDefault<UserEntity>($"select * from `ttl`.`ttl_user` where `Id` ={userId};");
			return user;
		}

		public List<UserEntity> GetUserList()
		{
			return MySqlContext.Query<UserEntity>($"select * from `ttl`.`ttl_user`;").ToList();
		}

		public int AddUser(UserEntity userEntity)
		{
			return MySqlContext.Execute("INSERT INTO `ttl`.`ttl_user` (`UserName`,`Password`,`Phone`) VALUES(@UserName,@Password,@Phone); ", userEntity);
		}

		public int DeleteUser(string userId)
		{
			UserEntity user = GetUserById(userId);
			if (user == null)
				return -1;
			MySqlContext.Execute($"INSERT IGNORE INTO `ttl`.`ttl_user_delete` (  `Tdate`,`UserName`,`Password`,`Phone`,`IsSuperAdmin`) SELECT `Tdate`,`UserName`,`Password`,`Phone`,`IsSuperAdmin` FROM `ttl_user` WHERE id={userId}");
			MySqlContext.Execute($"DELETE From `ttl`.`ttl_user` where `Id`={userId}");
			return 1;
		}

		public int UpdateUser(UserUpdateModel userUpdateModel)
		{
			StringBuilder set = new StringBuilder();

			if (!string.IsNullOrEmpty(userUpdateModel.Phone))
			{
				List<int> countList = MySqlContext.Query<int>($"SELECT COUNT(1) FROM  `ttl`.`ttl_user` WHERE `Id`!=@UserId and `Phone`=@Phone", userUpdateModel).ToList();
				if (countList[0] > 0)
					return -1;
				set.Append($",`Phone` = @Phone");
			}

			if(!string.IsNullOrEmpty(userUpdateModel.UserName))
				set.Append($",`UserName` = @UserName");

			if (!string.IsNullOrEmpty(userUpdateModel.Password))
				set.Append($",`Password` = @Password");

			if (set.Length>0)
				return MySqlContext.Execute($"UPDATE `ttl`.`ttl_user` SET {set.ToString().Substring(1)} where `Id`=@UserId", userUpdateModel);

			return 0;
		}
	}
}
