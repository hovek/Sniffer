using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sniffer.Settings;
using Sniffer.Common;

namespace Sniffer.Sniffs.Njuskalo
{
	[Serializable]
	public class NjuskaloUserSettings : UserSettings
	{
		public DateTime ValidFrom { get; set; }
		public DateTime ValidTo { get; set; }
        public string Url { get; set; }
        public string TitleNotText { get; set; }
        public string DescriptionNotText { get; set; }

		public bool IsActive
		{
			get
			{
				return Active && ValidFrom <= DateTime.Now && ValidTo >= DateTime.Now;
			}
		}

        public NjuskaloUserSettings()
		{
			ValidFrom = new DateTime(1753, 01, 01, 0, 0, 0);
			ValidTo = new DateTime(9998, 12, 31, 0, 0, 0);
		}
	}
}