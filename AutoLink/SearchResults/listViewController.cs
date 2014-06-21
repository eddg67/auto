using System;
using System.Linq;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using AutoLink.Models;
using System.Collections.Generic;
using MonoTouch.Dialog;

namespace AutoLink
{
	public class listViewController : UITableViewController
	{
		public string searchId  { get; set; }
		public bool useBinId  { get; set; }

		public DetailViewController DetailViewController {
			get;
			set;
		}

		public listViewController (string id,bool bin) : base (UITableViewStyle.Grouped)
		{
	
			searchId = id;
			useBinId = bin;
			//TableView.TableHeaderView = GetHeader ();
			TableView.SeparatorInset = new UIEdgeInsets (0, 0, 0, 0);

			//TableView.ContentInset = new UIEdgeInsets (0, 5, 0, 5); 

		}

	

		public override void ViewWillLayoutSubviews()
		{
			var frame = TableView.Frame;
			TableView.Frame = new RectangleF (0, 0, frame.Width, frame.Height);
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			TableView.RowHeight = 0;

		

			// Register the TableView's data source
			if (useBinId) {
				TableView.Source = new listViewSource ("",searchId);
			} else {
				TableView.Source = new listViewSource (searchId);
			}




		}



		public UIToolbar CreateToolbarView()
		{
			var tool = new UIToolbar (new RectangleF (0, 0, 320, 60));
			tool.BackgroundColor = UIColor.Black;
			var btn = new UIBarButtonItem (UIBarButtonSystemItem.Add, (sender, args) => {
				// button was clicked
			});

			tool.SetItems (new UIBarButtonItem[]{ 
				btn
			},true);

			return tool;
		}

		public UIView GetHeader ()
		{

			var TitleHeader = new UITextView (new RectangleF (
				0, 
				10, 
				UIScreen.MainScreen.Bounds.Width, 
				50 
			)) {
				TextAlignment = UITextAlignment.Center,
				Text = @"New live search",
				Editable = false,
				AutosizesSubviews = true,
				Font = UIFont.FromName ("KannadaSangamMN-Bold", 30f)

			}; 

			TitleHeader.ContentInset = new UIEdgeInsets (10, 0, 0, 0);

			var hvHght = TitleHeader.Frame.Height + 10;
			var Header = new UIView (new RectangleF (0, 0, View.Frame.Width - 20, hvHght));
			Header.Add (TitleHeader);
		
			return Header;

		}
			

		private void HandleSwipeRight()
		{
			// load another view
			Console.WriteLine("Swipe right.");
		}
	}
}

