using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace AutoLink
{
	public class CircleView :  UIView
	{
		public CircleView ()
		{
			float x = 0;
			float y = 190;
			float width = 6;
			float height = width;
			// corner radius needs to be one half of the size of the view
			float cornerRadius = width / 2;
			RectangleF frame = new RectangleF(x, y, width, height);
			// initialize button
			UIButton circularView = new UIButton(frame);
			// set corner radius
			circularView.Layer.CornerRadius = cornerRadius;
			// set background color, border color and width to see the circular view
			circularView.BackgroundColor = UIColor.Blue;
			circularView.Layer.CornerRadius = cornerRadius;
			circularView.Layer.BorderColor = UIColor.Clear.CGColor;
			circularView.Layer.BorderWidth = 1;

			Add (circularView);


		}
			

		public override void Draw (RectangleF rect)
		{
			float x = 25;
			float y = 25;
			float width = 100;
			float height = width;
			// corner radius needs to be one half of the size of the view
			float cornerRadius = width / 2;
			RectangleF frame = new RectangleF(x, y, width, height);
			// initialize button
			UIButton circularView = new UIButton(frame);
			// set corner radius
			circularView.Layer.CornerRadius = cornerRadius;
			// set background color, border color and width to see the circular view
			circularView.BackgroundColor = UIColor.Blue;
			circularView.Layer.CornerRadius = cornerRadius;
			circularView.Layer.BorderColor = UIColor.Black.CGColor;
			circularView.Layer.BorderWidth = 5;

		}

	}
}

