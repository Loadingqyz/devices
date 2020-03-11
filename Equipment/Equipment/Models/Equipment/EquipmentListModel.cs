using Equipment.Entities.Equipment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Equipment.Models.Equipment
{
	public class EquipmentListModel
	{
		public List<EquipmentModel> EquipmentList { get; set; }
		public int TotalCount { get; set; }
		public int PageSize { get; set; }
		public int PageIndex { get; set; }
		public int TotalPage => (TotalCount / PageSize) + 1;
	}
}
