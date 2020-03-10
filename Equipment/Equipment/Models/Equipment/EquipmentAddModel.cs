using Equipment.Entities.Equipment;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Equipment.Models.Equipment
{
	public class EquipmentAddModel
	{
		[Required]
		public string EquipmentName { get; set; }
		public string LastMaintenanceNames { get; set; }
		public DateTime? LastMaintenanceTime { get; set; }
		[Required]
		public string FixedAssetId { get; set; }
		public string Remark { get; set; }
		public DateTime? ScrapDate { get; set; }
		public EquipmentEntity ConvertToEntity()
		{
			return new EquipmentEntity()
			{
				EquipmentName= this.EquipmentName,
				LastMaintenanceNames = this.LastMaintenanceNames,
				LastMaintenanceTime = this.LastMaintenanceTime,
				FixedAssetId = this.FixedAssetId,
				Remark = this.Remark,
				ScrapDate = this.ScrapDate
			};
		}
	}
}
