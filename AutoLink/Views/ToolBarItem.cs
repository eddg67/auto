using System;
using MonoTouch.UIKit;

namespace AutoLink
{
	public class ToolBarItem : UIButton
	{
		UITextAttributes attr = new UITextAttributes ();
		UIImage image;
		UIColor color = UIColor.LightGray;
		string text = string.Empty;
		float spacer = 12.0f;

		public ToolBarItem (string _text,UIImage _image,EventHandler handle = null,UIColor _color = null) 
			: base(UIButtonType.Custom)
		{
			color = _color;
			image = _image;
			text = _text;

			TitleLabel.Font = UIFont.FromName("Clan-Medium", 8f);
			attr.TextColor = UIColor.LightGray;
			SetImage (image, UIControlState.Normal);
			SetTitle (text, UIControlState.Normal);
			SetTitleColor (UIColor.Blue, UIControlState.Normal);

			ImageEdgeInsets = new UIEdgeInsets (- (image.Size.Height - spacer), 0,0,-image.Size.Width);
			TitleEdgeInsets = new UIEdgeInsets (- (image.Size.Height - spacer), 0,0,-image.Size.Width);
			//TitleEdgeInsets = new UIEdgeInsets (0, -image.Size.Width,- (image.Size.Height + spacer), 0);


		}
	}
}

