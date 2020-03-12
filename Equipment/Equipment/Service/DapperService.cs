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
		private string _connection = @"Database='ttl';Data Source=sh-cynosdbmysql-grp-iqthd3q4.sql.tencentcdb.com;Port=23948;User ID=root;Password=Qianyuzhe0!0;Connection Timeout=60;Keepalive=120;Character Set=utf8;";

		/// 返回连接实例        
		private IDbConnection dbConnection = null;

		/// <summary>
		/// 创建数据库连接对象并打开链接
		/// </summary>
		/// <returns></returns>
		public IDbConnection OpenCurrentDbConnection()
		{
			if (dbConnection == null)
			{
				dbConnection = new MySqlConnection(_connection);
			}
			//判断连接状态
			if (dbConnection.State == ConnectionState.Closed)
			{
				dbConnection.Open();
			}
			return dbConnection;
		}
	}

	public class MySqlContext : IDisposable
	{
		private readonly IDbConnection _connection;
		public MySqlContext()
		{
			_connection = new DapperService().OpenCurrentDbConnection();
		}


		/// <summary>
		/// 查出一条记录的实体
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="sql"></param>
		/// <returns></returns>
		public T QueryFirstOrDefault<T>(string sql, object param = null)
		{
			return _connection.QueryFirstOrDefault<T>(sql, param);
		}

		public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null)
		{
			return await _connection.QueryFirstOrDefaultAsync<T>(sql, param);
		}
		/// <summary>
		/// 查出多条记录的实体泛型集合
		/// </summary>
		/// <typeparam name="T">泛型T</typeparam>
		/// <returns></returns>
		public IEnumerable<T> Query<T>(string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
		{
			return _connection.Query<T>(sql, param, transaction, buffered, commandTimeout, commandType);
		}

		public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			return await _connection.QueryAsync<T>(sql, param, transaction, commandTimeout, commandType);
		}

		public int Execute(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			return _connection.Execute(sql, param, transaction, commandTimeout, commandType);
		}

		public async Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			return await _connection.ExecuteAsync(sql, param, transaction, commandTimeout, commandType);
		}

		public T ExecuteScalar<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			return _connection.ExecuteScalar<T>(sql, param, transaction, commandTimeout, commandType);
		}

		public async Task<T> ExecuteScalarAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			return await _connection.ExecuteScalarAsync<T>(sql, param, transaction, commandTimeout, commandType);
		}

		/// <summary>
		/// 同时查询多张表数据（高级查询）
		/// "select *from K_City;select *from K_Area";
		/// </summary>
		/// <param name="sql"></param>
		/// <returns></returns>
		public SqlMapper.GridReader QueryMultiple(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			return _connection.QueryMultiple(sql, param, transaction, commandTimeout, commandType);
		}
		public async Task<SqlMapper.GridReader> QueryMultipleAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			return await _connection.QueryMultipleAsync(sql, param, transaction, commandTimeout, commandType);
		}

		public void Dispose()
		{
			_connection.Dispose();
		}
	}
}
