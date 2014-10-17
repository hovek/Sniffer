using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Sniffer.Common
{
	public class Mouse
	{
		[DllImport("User32.dll")]
		private static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

		private const int MOUSEEVENTF_ABSOLUTE = 0x8000;
		private const int MOUSEEVENTF_MOVE = 0x1;
		private const int MOUSEEVENTF_WHEEL = 0x80;
		private const int WHEEL_DELTA = 120;
		private const int XBUTTON1 = 0x1;
		private const int XBUTTON2 = 0x2;
		private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
		private const int MOUSEEVENTF_LEFTUP = 0x0004;
		private const int MOUSEEVENTF_MIDDLEDOWN = 0x20;
		private const int MOUSEEVENTF_MIDDLEUP = 0x40;
		private const int MOUSEEVENTF_RIGHTDOWN = 0x8;
		private const int MOUSEEVENTF_RIGHTUP = 0x10;
		private const int MOUSEEVENTF_XDOWN = 0x100;
		private const int MOUSEEVENTF_XUP = 0x200;

		private const int MaxX = 65535;
		private const int MaxY = 65535;

		private static decimal XCoordinateMultiplier = (decimal)MaxX / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
		private static decimal YCoordinateMultiplier = (decimal)MaxY / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;

		public enum ButtonEvent
		{
			LeftDown = MOUSEEVENTF_LEFTDOWN,
			LeftUp = MOUSEEVENTF_LEFTUP,
			RightDown = MOUSEEVENTF_RIGHTDOWN,
			RightUp = MOUSEEVENTF_RIGHTUP,
			MiddleDown = MOUSEEVENTF_MIDDLEDOWN,
			MiddleUp = MOUSEEVENTF_MIDDLEUP
		}

		public static void Move(int x, int y)
		{
			mouse_event(MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE, (int)(x * XCoordinateMultiplier), (int)(y * YCoordinateMultiplier), 0, 0);
		}

		public static void Button(ButtonEvent button)
		{
			mouse_event((int)button, 0, 0, 0, 0);
		}
	}
}