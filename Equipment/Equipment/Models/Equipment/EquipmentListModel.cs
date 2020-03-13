using Equipment.Entities.Equipment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
		public string QueryArgs { get; set; }

		public void MatchQueryArgs()
		{
			if (string.IsNullOrEmpty(QueryArgs))
				return;
			if (EquipmentList == null || EquipmentList.Count == 0)
				return;

			foreach (var equipment in EquipmentList)
			{
				if (!string.IsNullOrEmpty(equipment.LastMaintenanceNames) && equipment.LastMaintenanceNames.Contains(QueryArgs, StringComparison.OrdinalIgnoreCase))
				{
					equipment.LastMaintenanceNamesSpan = HighlightText(equipment.LastMaintenanceNames, QueryArgs);
				}
				if (!string.IsNullOrEmpty(equipment.EquipmentName) && equipment.EquipmentName.Contains(QueryArgs, StringComparison.OrdinalIgnoreCase))
				{
					equipment.EquipmentNameSpan = HighlightText(equipment.EquipmentName, QueryArgs);
				}
				if (!string.IsNullOrEmpty(equipment.FixedAssetId) && equipment.FixedAssetId.Contains(QueryArgs, StringComparison.OrdinalIgnoreCase))
				{
					equipment.FixedAssetIdSpan = HighlightText(equipment.FixedAssetId, QueryArgs);
				}
			}
		}

		public string HighlightText(string inputText, string searchWord)
		{
			Regex expression = new Regex(searchWord.Replace(" ", "|"), RegexOptions.IgnoreCase);
			return expression.Replace(inputText, new MatchEvaluator(ReplaceKeywords));
		}

		private string ReplaceKeywords(Match m)
		{
			return "<span style=\"color: red\">" + m.Value + "</span>";
		}

	}
}
