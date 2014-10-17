using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sniffer.Settings;
using Sniffer.Common;

namespace Sniffer.Sniffs.Dvoznak
{
	[Serializable]
	public class DvoznakUserSettings : UserSettings
	{
		public bool SendCSVFile { get; set; }
		public bool CumulateCSVFile { get; set; }
		public DateTime CumulateCSVFileFrom { get; set; }
		public DateTime CumulateCSVFileTo { get; set; }
		public DateTime ValidFrom { get; set; }
		public DateTime ValidTo { get; set; }
		public bool SendNew { get; set; }
		public bool SendCompleted { get; set; }
		public bool IsBadOrGoodFilterOn { get; set; }
		public string BadOrGoodFilterSQL { get; set; }
		public string BadOrGoodFilterParameters { get; set; }
		public string BadOrGoodFilterSPName { get; set; }

		public bool IsActive
		{
			get
			{
				return Active && ValidFrom <= DateTime.Now && ValidTo >= DateTime.Now;
			}
		}

		public DvoznakUserSettings()
		{
			BadOrGoodFilterSQL = "";
			BadOrGoodFilterParameters = "";
			BadOrGoodFilterSPName = "";
			CumulateCSVFileFrom = new DateTime(1753, 1, 1, 0, 0, 0);
			CumulateCSVFileTo = new DateTime(1753, 1, 1, 0, 0, 0);
			ValidFrom = new DateTime(1753, 01, 01, 0, 0, 0);
			ValidTo = new DateTime(9998, 12, 31, 0, 0, 0);
		}
	}
}