using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gecko;

namespace Sniffer.Sniffs
{
	public class HoveWebBrowser : GeckoWebBrowser
	{
		public event EventHandler OnNavigate;

		new public void Navigate(string url)
		{
			base.Navigate(url);
			if (OnNavigate != null)
			{
				OnNavigate(this, EventArgs.Empty);
			}
		}
	}
}
