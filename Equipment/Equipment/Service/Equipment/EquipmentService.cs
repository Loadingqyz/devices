using Equipment.Entities.Equipment;
using System;
using System.Collections.Generic;
using System.Linq;
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
									  `LastMaintenanceTime`,
									  `FixedAssetId`,
									  `Remark`,
									  `ScrapDate`,
									  `CreateUserId`
									)
									VALUES
									(
										@EquipmentName,
										@LastMaintenanceNames,
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
									  `LastMaintenanceNames`,
									  `LastMaintenanceTime`,
									  `FixedAssetId`,
									  `Remark`,
									  `ScrapDate`,
									  `CreateUserId`,`LogContent`,`IsDelete`
									)
									VALUES
									(" + insertId +
										@",@EquipmentName,
										@LastMaintenanceNames,
										@LastMaintenanceTime,
										@FixedAssetId,
										@Remark,
										@ScrapDate,
										@CreateUserId,'添加设备',0
									  );", equipmentEntity);
			}

			return 1;
		}

		
	}
}
