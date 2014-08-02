using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Foundation;

namespace AutoLink
{
	public class NoResultsView : UIView
	{
		UILabel label,body,link;
		UIImage cogImg = UIImage.FromBundle ("cog.png");
		float offSet = 100.0f,labelH = 55f,bodyH=35f,linkH=35f;

		public NoResultsView (RectangleF frame,string searchID)
				:base(frame)
		{
			label = new UILabel (new RectangleF (0, (Bounds.Height / 2) - offSet, Bounds.Width, labelH));
			label.Font = UIFont.FromName ("Clan-Medium", 24f);
			label.Text = "Shoot!";
			label.TextAlignment = UITextAlignment.Center;

			body = new UILabel (new RectangleF (0, ((Bounds.Height / 2) +labelH) - offSet , Bounds.Width, bodyH));
			body.Font = UIFont.FromName ("Clan-Book", 10f);
			body.Text = "We didn't find any listings matching your search criteria.\nTry changing below in the search settings!";
			body.TextAlignment = UITextAlignment.Center;
			body.Lines = 2;

			link = new UILabel (new RectangleF (0, ((Bounds.Height / 2) + (bodyH +labelH) ) - offSet , Bounds.Width, linkH));
			link.Font = UIFont.FromName ("Clan-Book", 10f);
			link.TextColor = UIColor.Blue;
			//link.BackgroundColor = UIColor.Blue;
			link.Text = "Live Search Settings!";
			link.TextAlignment = UITextAlignment.Center;
			link.Lines = 1;

			var textAttach = new NSTextAttachment ();
			textAttach.Image = cogImg;

			NSAttributedString cog = NSAttributedString.CreateFrom (textAttach);
			var text = new NSAttributedString (
				"Live Search Settings!",
				font: UIFont.FromName ("Clan-Book", 12.0f),
				foregroundColor: UIColor.Blue,
				backgroundColor:UIColor.White,
				strokeColor:UIColor.Blue,
				strokeWidth:4f
			);

				
			var attributeString = new NSMutableAttributedString ();
			attributeString.Append (cog);
			attributeString.Append (text);

			link.AttributedText = attributeString;
			link.UserInteractionEnabled = true;

			link.AddGestureRecognizer (
				new UITapGestureRecognizer(() => {
					using (var app = (AppDelegate)UIApplication.SharedApplication.Delegate){
						app.ShowSearch(searchID);
					}
						
						
				})
			);

			Add (label);
			Add (body);
			Add (link);
		}



	}
	
}

