using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Equipment.Models.Equipment
{
	public class EquipmentQueryModel
	{
		public int PageIndex { get; set; } = 1;
		public int PageSize { get; set; } = 20;
		public int StartIndex => (PageIndex - 1) * PageSize;
	}
}
