using Equipment.Entities.Equipment;
using Equipment.Models.Equipment;
using Equipment.Service.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Equipment.Service.Equipment
{
	public class EquipmentService: IBaseService
	{
		private MySqlContext _dbContext;
		public EquipmentService(MySqlContext dbContext)
		{
			_dbContext = dbContext;
		}
		public int AddEquipment(EquipmentEntity equipmentEntity)
		{
			string sql = @"INSERT INTO `ttl`.`ttl_engineering_equipment` (
									  `EquipmentName`,
									  `LastMaintenanceNames`,
									  `LastMaintenanceIds`,
									  `LastMaintenanceTime`,
									  `FixedAssetId`,
									  `Remark`,
									  `ScrapDate`,
									  `CreateUserId`
									)
									VALUES
									(
										@EquipmentName,
										@LastMaintenanceNames,@LastMaintenanceIds,
										@LastMaintenanceTime,
										@FixedAssetId,
										@Remark,
										@ScrapDate,
										@CreateUserId
									  );SELECT LAST_INSERT_ID();";
			int insertId = _dbContext.ExecuteScalar<int>(sql, equipmentEntity);
			if (insertId > 0) 
			{
				_dbContext.Execute(@"INSERT INTO `ttl`.`ttl_engineering_equipment_log` (
									  `EquipmentId`,
									  `EquipmentName`,
									  `LastMaintenanceNames`, `LastMaintenanceIds`,
									  `LastMaintenanceTime`,
									  `FixedAssetId`,
									  `Remark`,
									  `ScrapDate`,
									  `CreateUserId`,`LogContent`,`IsDelete`
									)
									VALUES
									(" + insertId +
										@",@EquipmentName,
										@LastMaintenanceNames,@LastMaintenanceIds,
										@LastMaintenanceTime,
										@FixedAssetId,
										@Remark,
										@ScrapDate,
										@CreateUserId,'添加设备',0
									  );", equipmentEntity);
			}

			return 1;
		}

		public EquipmentListModel GetEquipmentByPagination(EquipmentQueryModel queryModel)
		{
			StringBuilder where = new StringBuilder();
			if (!string.IsNullOrEmpty(queryModel.QueryArgs))
			{
				where.Append($" and (`EquipmentName` like '%{queryModel.QueryArgs}%'");
				where.Append($" or `LastMaintenanceNames` like '%{queryModel.QueryArgs}%'");
				where.Append($" or `FixedAssetId` like '%{queryModel.QueryArgs}%' ) ");
			}


			StringBuilder sqlBuilder1 = new StringBuilder();
			sqlBuilder1.Append($"SELECT * FROM `ttl`.`ttl_engineering_equipment` WHERE isdelete=0 {where.ToString()} LIMIT {queryModel.StartIndex},{queryModel.PageSize}; ");

			StringBuilder sqlBuilder2 = new StringBuilder();
			sqlBuilder2.Append($"SELECT COUNT(1) FROM `ttl`.`ttl_engineering_equipment` WHERE isdelete=0 {where.ToString()};");

			EquipmentListModel resultModel = new EquipmentListModel();
			using (var gridReader = _dbContext.QueryMultiple($"{sqlBuilder1.ToString()}{sqlBuilder2.ToString()}"))
			{
				resultModel.EquipmentList = gridReader.Read<EquipmentModel>().ToList();
				resultModel.TotalCount = gridReader.Read<int>().Single();
			}
			resultModel.PageIndex = queryModel.PageIndex;
			resultModel.PageSize = queryModel.PageSize;
			resultModel.QueryArgs = queryModel.QueryArgs;
			return resultModel;
		}

		public EquipmentEntity GetEquipmentById(long id)
		{
			return _dbContext.QueryFirstOrDefault<EquipmentEntity>($"select * from `ttl`.`ttl_engineering_equipment` WHERE isdelete=0 and id={id}");
		}

		public int UpdateEquipment(EquipmentEntity equipmentEntity,string logContent= "修改设备")
		{
			int code = 0;
			StringBuilder sqlBuilder = new StringBuilder();
			if (!string.IsNullOrEmpty(equipmentEntity.EquipmentName))
				sqlBuilder.Append(",`EquipmentName` = @EquipmentName");
			if (!string.IsNullOrEmpty(equipmentEntity.LastMaintenanceIds))
				sqlBuilder.Append(",`LastMaintenanceIds` = @LastMaintenanceIds");
			if (!string.IsNullOrEmpty(equipmentEntity.LastMaintenanceNames))
				sqlBuilder.Append(",`LastMaintenanceNames` = @LastMaintenanceNames");
			if (equipmentEntity.LastMaintenanceTime.HasValue)
				sqlBuilder.Append(",`LastMaintenanceTime` = @LastMaintenanceTime");
			if (!string.IsNullOrEmpty(equipmentEntity.FixedAssetId))
				sqlBuilder.Append(",`FixedAssetId` = @FixedAssetId");
			if (equipmentEntity.IsDelete.HasValue)
				sqlBuilder.Append(",`IsDelete` = @IsDelete");
			if (!string.IsNullOrEmpty(equipmentEntity.Remark))
				sqlBuilder.Append(",`Remark` = @Remark");
			if (equipmentEntity.ScrapDate.HasValue)
				sqlBuilder.Append(",`ScrapDate` = @ScrapDate");
			if (equipmentEntity.UpdateUserId.HasValue)
				sqlBuilder.Append(",`UpdateUserId` = @UpdateUserId");

			if (sqlBuilder.Length > 0)
				code = _dbContext.Execute($"UPDATE `ttl`.`ttl_engineering_equipment`  SET {sqlBuilder.ToString().Substring(1)} WHERE `Id` =@Id ;", equipmentEntity);

			if (code > 0)
			{
				_dbContext.Execute(@"INSERT INTO `ttl`.`ttl_engineering_equipment_log` (
									  `EquipmentId`,
									  `EquipmentName`,
									  `LastMaintenanceNames`, `LastMaintenanceIds`,
									  `LastMaintenanceTime`,
									  `FixedAssetId`,
									  `Remark`,
									  `ScrapDate`,
									  `CreateUserId`,`LogContent`,`IsDelete`,`UpdateUserId`
									)
									VALUES
									(" + equipmentEntity.Id +
										@",@EquipmentName,
										@LastMaintenanceNames,@LastMaintenanceIds,
										@LastMaintenanceTime,
										@FixedAssetId,
										@Remark,
										@ScrapDate,
										@CreateUserId,
										'" + logContent + @"',@IsDelete,@UpdateUserId
									  );", equipmentEntity);
			}

			return 1;
		}
	}
}
