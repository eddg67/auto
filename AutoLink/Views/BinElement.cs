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

