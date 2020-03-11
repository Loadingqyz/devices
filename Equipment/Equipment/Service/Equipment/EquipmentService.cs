using Equipment.Entities.Equipment;
using Equipment.Models.Equipment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Equipment.Service.Equipment
{
	public class EquipmentService
	{
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
			int insertId = MySqlContext.ExecuteScalar<int>(sql, equipmentEntity);
			if (insertId > 0) 
			{
				MySqlContext.Execute(@"INSERT INTO `ttl`.`ttl_engineering_equipment_log` (
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
			string sql = $"SELECT * FROM `ttl`.`ttl_engineering_equipment` WHERE isdelete=0 LIMIT {queryModel.StartIndex},{queryModel.PageSize}; SELECT COUNT(1) FROM `ttl`.`ttl_engineering_equipment` WHERE isdelete=0;";
			EquipmentListModel resultModel = new EquipmentListModel();
			using (var gridReader = MySqlContext.QueryMultiple(sql))
			{
				resultModel.EquipmentList = gridReader.Read<EquipmentModel>().ToList();
				resultModel.TotalCount = gridReader.Read<int>().Single();
			}
			resultModel.PageIndex = queryModel.PageIndex;
			resultModel.PageSize = queryModel.PageSize;
			return resultModel;
		}

		public EquipmentEntity GetEquipmentById(long id)
		{
			return MySqlContext.QueryFirstOrDefault<EquipmentEntity>($"select * from `ttl`.`ttl_engineering_equipment` WHERE isdelete=0 and id={id}");
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
				code = MySqlContext.Execute($"UPDATE `ttl`.`ttl_engineering_equipment`  SET {sqlBuilder.ToString().Substring(1)} WHERE `Id` =@Id ;", equipmentEntity);

			if (code > 0)
			{
				MySqlContext.Execute(@"INSERT INTO `ttl`.`ttl_engineering_equipment_log` (
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
