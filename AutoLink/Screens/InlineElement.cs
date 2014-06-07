using System;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using System.Drawing;
using MonoTouch.Foundation;

namespace AutoLink
{
	public class InlineElement : UIView {

		UITextView leftRow,rightRow;

		public InlineElement (string left,string right)
		{
			leftRow = new UITextView (new RectangleF (0, 0, Bounds.Width  / 2 - 5, 50)) { Text = left };
			rightRow = new UITextView (new RectangleF (0, 50, Bounds.Width / 2 - 5, 50)) { Text = right };
			AddSubview (leftRow);
			AddSubview (rightRow);
			Frame = new RectangleF (0, 50, Bounds.Width / 2 - 5, 50);

		}

	}
}

