using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace AutoLink
{
	public class NoResultsView : UIView
	{
		UILabel label,body;


		public NoResultsView (RectangleF frame,string searchID)
				:base(frame)
		{
			label = new UILabel (new RectangleF (0, Bounds.Height / 2, Bounds.Width, 55f));
			label.Font = UIFont.FromName ("Clan-Medium", 24f);
			label.Text = "Shoot!";
			label.TextAlignment = UITextAlignment.Center;

			body = new UILabel (new RectangleF (0, (Bounds.Height / 2) + 55 , Bounds.Width, 35f));
			body.Font = UIFont.FromName ("Clan-Book", 10f);
			body.Text = "We didn't find any listings matching your search criteria. Try changing below in the search settings!";
			body.TextAlignment = UITextAlignment.Center;
			body.Lines = 2;


			Add (label);
			Add (body);
		}



	}
	
}

