using System;
using MonoTouch.UIKit;
using System.Drawing;
using AutoLink.Models;

namespace AutoLink
{
	public class CellView : UIView
	{
		UILabel price,desc,make,mileage,source;
		Listing listing;
		public CellView (RectangleF frame) : base(frame)
		{
		}

		public void SetData(Listing list)
		{
			listing = list;
		}

		public UIView GetView(Listing list)
		{

			return new UIView ();

		}


	}
}

