using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;

namespace AutoLink
{
	public class BinElement : StyledStringElement
	{
		public BinElement(string caption, string value, UITableViewCellStyle style)  
			: base(caption, value, style) 
		{
			Font = UIFont.FromName ("Clan-Book", 12f);
			TextColor = UIColor.LightGray;
		}

		public override UITableViewCell GetCell (UITableView tv)
		{
			//var key = GetKey ((int) style);
			//var cell1 = tv.DequeueReusableCell (key);

			var cell = new UITableViewCell (style, "");
			cell.SelectionStyle = UITableViewCellSelectionStyle.Blue;

			PrepareCell (cell);
			if (cell.DetailTextLabel != null) {
				cell.DetailTextLabel.Text = Value;
			}
			return cell;

		}


	}
}

