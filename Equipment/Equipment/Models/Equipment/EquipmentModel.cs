﻿using Equipment.Entities.Equipment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Equipment.Models.Equipment
{
	public class EquipmentModel: EquipmentEntity
	{
		public string LastMaintenanceTimeStr => LastMaintenanceTime.HasValue ? LastMaintenanceTime.Value.ToString("yyyy-MM-dd") : "";
		public string ScrapDateStr => ScrapDate.HasValue ? ScrapDate.Value.ToString("yyyy-MM-dd") : "";
		public string EquipmentNameSpan { get; set; }
		public string LastMaintenanceNamesSpan { get; set; }
		public string FixedAssetIdSpan { get; set; }

		public string ShowEquipmentName => !string.IsNullOrEmpty(EquipmentNameSpan) ? EquipmentNameSpan : EquipmentName;
		public string ShowLastMaintenanceNames => !string.IsNullOrEmpty(LastMaintenanceNamesSpan) ? LastMaintenanceNamesSpan : LastMaintenanceNames;
		public string ShowFixedAssetId => !string.IsNullOrEmpty(FixedAssetIdSpan) ? FixedAssetIdSpan : FixedAssetId;
	}
}
