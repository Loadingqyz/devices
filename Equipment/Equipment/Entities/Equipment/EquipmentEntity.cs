using Equipment.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Equipment.Entities.Equipment
{
	public class EquipmentEntity : BaseEntity
	{
		public string EquipmentName { get; set; }
		public string LastMaintenanceNames { get; set; }
		public string LastMaintenanceIds { get; set; }
		public DateTime? LastMaintenanceTime { get; set; }
		public string FixedAssetId { get; set; }
		public string Remark { get; set; }
		public DateTime? ScrapDate { get; set; }
		public long? UpdateUserId { get; set; }
		public int? IsDelete { get; set; }
	}
}
