using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;

namespace AutoLink
{
	public class DualSection : Section
	{
		public DualSection(string header) : base(header){}

		public DualSection(string header, string footer) : base(header, footer) {}

	}
}


