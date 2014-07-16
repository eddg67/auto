using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System.Drawing;

namespace AutoLink
{
	public class StyledEntryElement : EntryElement
	{
		//Fields
		private string fontName;
		private string placeholder;

		public StyledEntryElement (string _caption, string _placeholder, string value) : base(string.Empty,string.Empty, value)
		{
			Caption = _caption;
			placeholder = _placeholder;
			fontName = "Clan-Book";
		}

		public override UITableViewCell GetCell(UITableView tableView) 
		{
			var cell = base.GetCell(tableView);
			cell.TextLabel.Font = UIFont.FromName (fontName, 14);
			return cell;
		}

		protected override UITextField CreateTextField(RectangleF frame)
		{
			var textField = base.CreateTextField (frame);
			textField.Font = UIFont.FromName (fontName, 14);
			return textField;
		}

	}
}

