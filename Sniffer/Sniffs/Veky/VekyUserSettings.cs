using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sniffer.Settings;
using Sniffer.Common;

namespace Sniffer.Sniffs.Veky
{
	[Serializable]
	public class VekyUserSettings : UserSettings
	{
		public DateTime ValidFrom { get; set; }
		public DateTime ValidTo { get; set; }

		public bool IsActive
		{
			get
			{
				return Active && ValidFrom <= DateTime.Now && ValidTo >= DateTime.Now;
			}
		}

        public VekyUserSettings()
		{
			ValidFrom = new DateTime(1753, 01, 01, 0, 0, 0);
			ValidTo = new DateTime(9998, 12, 31, 0, 0, 0);
		}
	}
}