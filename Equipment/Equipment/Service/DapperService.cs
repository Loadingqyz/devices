using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Equipment.Service
{
	public class DapperService
	{
		/// 数据库连接名
		private static string _connection = string.Empty;

		/// 获取连接名        
		private static string Connection
		{
			get { return _connection; }
		}

		/// 返回连接实例        
		private static IDbConnection dbConnection = null;

		/// 静态变量保存类的实例        
		private static DapperService uniqueInstance;

		/// 定义一个标识确保线程同步        
		private static readonly object locker = new object();
		/// <summary>
		/// 私有构造方法，使外界不能创建该类的实例，以便实现单例模式
		/// </summary>
		private DapperService()
		{
			// 这里为了方便演示直接写的字符串，实例项目中可以将连接字符串放在配置文件中，再进行读取。
			_connection = @"Database='ttl';Data Source=sh-cynosdbmysql-grp-iqthd3q4.sql.tencentcdb.com;Port=23948;User ID=root;Password=Qianyuzhe0!0;Connection Timeout=60;Keepalive=120;";
		}

		/// <summary>
		/// 获取实例，这里为单例模式，保证只存在一个实例
		/// </summary>
		/// <returns></returns>
		public static DapperService GetInstance()
		{
			// 双重锁定实现单例模式，在外层加个判空条件主要是为了减少加锁、释放锁的不必要的损耗
			if (uniqueInstance == null)
			{
				lock (locker)
				{
					if (uniqueInstance == null)
					{
						uniqueInstance = new DapperService();
					}
				}
			}
			return uniqueInstance;
		}


		/// <summary>
		/// 创建数据库连接对象并打开链接
		/// </summary>
		/// <returns></returns>
		public static IDbConnection OpenCurrentDbConnection()
		{
			if (dbConnection == null)
			{
				dbConnection = new MySqlConnection(Connection);
			}
			//判断连接状态
			if (dbConnection.State == ConnectionState.Closed)
			{
				dbConnection.Open();
			}
			return dbConnection;
		}
	}

	public static class MySqlContext
	{
		// 获取开启数据库的连接
		private static IDbConnection Db
		{
			get
			{
				//创建单一实例
				DapperService.GetInstance();
				return DapperService.OpenCurrentDbConnection();
			}
		}

		/// <summary>
		/// 查出一条记录的实体
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="sql"></param>
		/// <returns></returns>
		public static T QueryFirstOrDefault<T>(string sql, object param = null)
		{
			return Db.QueryFirstOrDefault<T>(sql, param);
		}

		public static Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null)
		{
			return Db.QueryFirstOrDefaultAsync<T>(sql, param);
		}
		/// <summary>
		/// 查出多条记录的实体泛型集合
		/// </summary>
		/// <typeparam name="T">泛型T</typeparam>
		/// <returns></returns>
		public static IEnumerable<T> Query<T>(string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
		{
			return Db.Query<T>(sql, param, transaction, buffered, commandTimeout, commandType);
		}

		public static Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			return Db.QueryAsync<T>(sql, param, transaction, commandTimeout, commandType);
		}

		public static int Execute(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			return Db.Execute(sql, param, transaction, commandTimeout, commandType);
		}

		public static Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			return Db.ExecuteAsync(sql, param, transaction, commandTimeout, commandType);
		}

		public static T ExecuteScalar<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			return Db.ExecuteScalar<T>(sql, param, transaction, commandTimeout, commandType);
		}

		public static Task<T> ExecuteScalarAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			return Db.ExecuteScalarAsync<T>(sql, param, transaction, commandTimeout, commandType);
		}

		/// <summary>
		/// 同时查询多张表数据（高级查询）
		/// "select *from K_City;select *from K_Area";
		/// </summary>
		/// <param name="sql"></param>
		/// <returns></returns>
		public static SqlMapper.GridReader QueryMultiple(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			return Db.QueryMultiple(sql, param, transaction, commandTimeout, commandType);
		}
		public static Task<SqlMapper.GridReader> QueryMultipleAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			return Db.QueryMultipleAsync(sql, param, transaction, commandTimeout, commandType);
		}
	}
}
