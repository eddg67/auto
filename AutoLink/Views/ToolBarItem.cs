using System;
using MonoTouch.UIKit;

namespace AutoLink
{
	public class ToolBarItem : UIBarButtonItem
	{
		UITextAttributes attr = new UITextAttributes ();
		UIImage image;
		UIColor color = UIColor.LightGray;
		string text = string.Empty;

		public ToolBarItem (string _text,UIImage _image,EventHandler handle = null,UIColor _color = null) 
			: base(_text, UIBarButtonItemStyle.Plain,handle)
		{
			color = _color;
			image = _image;
			text = _text;

			attr.Font = UIFont.FromName("Clan-Medium", 8f);
			attr.TextColor = UIColor.LightGray;

			Image = _image;
			ImageInsets = new UIEdgeInsets (0, 10f, 0, 0);


		}
	}
}

