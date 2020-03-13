using Equipment.Models.User;
using Equipment.Service.Db;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Equipment.Service.User
{
	public class UserService: IBaseService
	{
		private MySqlContext _dbContext;
		public UserService(MySqlContext dbContext)
		{
			_dbContext = dbContext;
		}
		public string GenerateAuthInfo(UserEntity userEntity)
		{
			if (userEntity.Id == 0 || string.IsNullOrEmpty(userEntity.Password))
				throw new Exception("Auth异常");
			return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userEntity.Id}:{userEntity.Password}"));
		}
		public UserEntity CheckUserLogin(UserLoginModel userLoginModel)
		{
			UserEntity user = _dbContext.QueryFirstOrDefault<UserEntity>($"select * from `ttl`.`ttl_user` where `Phone` =@phone and `Password`=@password ;", userLoginModel);
			return user;
		}

		public UserEntity GetUserById(string userId)
		{
			UserEntity user = _dbContext.QueryFirstOrDefault<UserEntity>($"select * from `ttl`.`ttl_user` where `Id` ={userId};");
			return user;
		}

		public List<UserEntity> GetUserList()
		{
			return _dbContext.Query<UserEntity>($"select * from `ttl`.`ttl_user`;").ToList();
		}

		public int AddUser(UserEntity userEntity)
		{
			return _dbContext.Execute("INSERT INTO `ttl`.`ttl_user` (`UserName`,`Password`,`Phone`,`CreateUserId`,`IsSuperAdmin`) VALUES(@UserName,@Password,@Phone,@CreateUserId,@IsSuperAdmin); ", userEntity);
		}

		public int DeleteUser(string userId)
		{
			UserEntity user = GetUserById(userId);
			if (user == null)
				return -1;
			_dbContext.Execute($"INSERT IGNORE INTO `ttl`.`ttl_user_delete` (  `Tdate`,`UserName`,`Password`,`Phone`,`IsSuperAdmin`,`CreateUserId`) SELECT `Tdate`,`UserName`,`Password`,`Phone`,`IsSuperAdmin`,`CreateUserId` FROM `ttl`.`ttl_user` WHERE id={userId}");
			_dbContext.Execute($"DELETE From `ttl`.`ttl_user` where `Id`={userId}");
			return 1;
		}

		public int UpdateUser(UserUpdateModel userUpdateModel)
		{
			StringBuilder set = new StringBuilder();

			if (!string.IsNullOrEmpty(userUpdateModel.Phone))
			{
				List<int> countList = _dbContext.Query<int>($"SELECT COUNT(1) FROM  `ttl`.`ttl_user` WHERE `Id`!=@UserId and `Phone`=@Phone", userUpdateModel).ToList();
				if (countList[0] > 0)
					return -1;
				set.Append($",`Phone` = @Phone");
			}

			if(!string.IsNullOrEmpty(userUpdateModel.UserName))
				set.Append($",`UserName` = @UserName");

			if (!string.IsNullOrEmpty(userUpdateModel.Password))
				set.Append($",`Password` = @Password");

			if(userUpdateModel.IsSuperAdmin.HasValue)
				set.Append($",`IsSuperAdmin` = @IsSuperAdmin");

			if (set.Length>0)
				return _dbContext.Execute($"UPDATE `ttl`.`ttl_user` SET {set.ToString().Substring(1)} where `Id`=@UserId", userUpdateModel);

			return 0;
		}
	}
}
