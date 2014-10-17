using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sniffer.Sniffs;

namespace Sniffer.Settings
{
	[Serializable]
	public class Settings
	{
		public ProgramSettings ProgramSettings { get; set; }
		public Dictionary<SniffEnum, SniffSettings> SnifferSettings { get; set; }

		public Settings()
		{
			ProgramSettings = new global::Sniffer.Settings.ProgramSettings();
			SnifferSettings = new Dictionary<SniffEnum, SniffSettings>();
		}
	}
}
