using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace AutoLink
{
	public class LineView : UIView
	{
		public LineView (RectangleF rec):base(rec)
		{
			using (var line = new UIView (rec)) {
		
				Add (line);
			}

		}
			

	}
}

