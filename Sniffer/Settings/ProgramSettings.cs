using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sniffer.Sniffs;

namespace Sniffer.Settings
{
	[Serializable]
	public class ProgramSettings
	{
		public SniffEnum SelectedSniffer { get; set; }
		public decimal RepeatDelay { get; set; }
	}
}
