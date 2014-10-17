using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using Gecko;

namespace Sniffer
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			MainForm mainForm = new MainForm();
			Application.ThreadException += mainForm.Application_ThreadException;
			Application.ApplicationExit += (sender, e) =>
			{
				Xpcom.Shutdown();
			};
			Application.Run(mainForm);
		}
	}
}
