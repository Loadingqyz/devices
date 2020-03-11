using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Equipment.Models.Equipment
{
	public class EquipmentUpdateViewModel
	{
		public long EquipmentId { get; set; }
		public string EquipmentName { get; set; }
		public string LastMaintenanceNames { get; set; }
		public string LastMaintenanceIds { get; set; }
		public HashSet<string> LastMaintenanceIdSet => string.IsNullOrEmpty(LastMaintenanceIds) ? new HashSet<string>(): LastMaintenanceIds.Split(',').ToHashSet();
		public DateTime? LastMaintenanceTime { get; set; }
		public string FixedAssetId { get; set; }
		public string Remark { get; set; }
		public DateTime? ScrapDate { get; set; }
		public string LastMaintenanceTimeStr => LastMaintenanceTime.HasValue ? LastMaintenanceTime.Value.ToString("yyyy-MM-dd") : "";
		public string ScrapDateStr => ScrapDate.HasValue ? ScrapDate.Value.ToString("yyyy-MM-dd") : "";
	}
}
