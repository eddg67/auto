using System;
using MonoTouch.UIKit;
using System.Collections.Generic;
using MonoTouch.Foundation;


/*
 * Standard Model for PickerView
 * Takes List to generate
 * Returns Click in PickerChanged Event
 *
 */

namespace AutoLink.Models
{
	public class ListPicker<TItem> : UIPickerViewModel
	{

		public TItem SelectedItem { get; private set; }
		public event EventHandler PickerChanged;

		IList<TItem> _items;

		public IList<TItem> Items
		{
			get { return _items; }
			set { _items = value; Selected(null, 0, 0); }
		}
			
		public ListPicker(IList<TItem> items)
		{
			Items = items;
		}
			

		public override int GetRowsInComponent(UIPickerView picker, int component)
		{
			if (NoItem())
				return 1;
			return Items.Count;
		}

		public override string GetTitle(UIPickerView picker, int row, int component)
		{
			if (NoItem(row))
				return "";
			var item = Items[row];
			return GetTitleForItem(item);
		}
		//when click
		public override void Selected(UIPickerView picker, int row, int component)
		{
			if (NoItem (row)) {
				SelectedItem = default(TItem);
			} else {
				SelectedItem = Items [row];
			}

			if (this.PickerChanged != null)
			{

				this.PickerChanged(this, new PickerChangedEventArgs{SelectedItem=SelectedItem,Picker=picker});
				picker.EndEditing (true);
				picker.ShowSelectionIndicator = true;
				//picker.Hidden = true;
				//picker.RemoveFromSuperview ();
			}
				

		}

		public override int GetComponentCount(UIPickerView picker)
		{
			return 1;
		}

		public virtual string GetTitleForItem(TItem item)
		{
			return item.ToString();
		}
		public override float GetComponentWidth (UIPickerView picker, int component)
		{
			if (component == 0)
				return picker.Bounds.Width;
			else
				return 40f;
		}

		public override float GetRowHeight (UIPickerView picker, int component)
		{
			return 40f;
		}



		bool NoItem(int row = 0)
		{
			return Items == null || row >= Items.Count;
		}


			
	}
	public class PickerChangedEventArgs : EventArgs{
		public object SelectedItem {get;set;}
		public object Picker{ get; set; }
		public object Id { get; set; }
	}
}

