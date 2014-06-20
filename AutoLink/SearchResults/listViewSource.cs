using System;
using System.Linq;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using AutoLink.Models;
using System.Collections.Generic;
using AutoLink.Services;


namespace AutoLink 
{
	public class listViewSource : UITableViewSource
	{

		public List<Listing> items;
		public ListResult listResults;
		public string searchID,binID;
		AppDelegate app = (AppDelegate)UIApplication.SharedApplication.Delegate;
		SearchService service; 
		bool updating = false;
		int startCount = 0;
		bool moreResults = true;

		public DetailViewController DetailViewController {
			get;
			set;
		}

		public listViewSource (string id,string binId = "")
		{
			//this.W
			service = app.searchService;
			searchID = id;
			binID = binId;


			if (!string.IsNullOrEmpty (binId)) {

				items = service.GetBinsListings (binId, null);

			} else {

				listResults = service.GetListings (id);
				items = listResults.listings;
			}
		

			if (items != null) {
				startCount = items.Count;
			} 
				

		}

		public override string TitleForHeader (UITableView tableView, int section)
		{
			return "test";
		}

		public override int NumberOfSections (UITableView tableView)
		{
			// TODO: return the actual number of sections
			return 1;
		}

		public override int RowsInSection (UITableView tableview, int section)
		{
			// TODO: return the actual number of items in the section
			return (items != null )?items.Count:0;
		}
			
		public override UIView GetViewForHeader (UITableView tableView, int section)
		{
			return new UIView(new RectangleF(0,0,0,0));
		}
			

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			tableView.SectionHeaderHeight = 0;
			//set the type
			items [indexPath.Row].listType = (!string.IsNullOrEmpty (binID)) ? ListType.Bin : ListType.Listings;
			//add edits
			var leftView = new UILabel () {
				Frame = new RectangleF (0, 0, 75, tableView.RowHeight/2),
				BackgroundColor = UIColor.Red,
				Text = "Delete!",
				TextColor = UIColor.White,
				TextAlignment = UITextAlignment.Left
			};
			UITapGestureRecognizer labelTap = new UITapGestureRecognizer(() => {
				service.DeleteItem(searchID,items [indexPath.Row]);
				CommitEditingStyle(tableView,UITableViewCellEditingStyle.Delete,indexPath);

			});

			leftView.UserInteractionEnabled = true;
			leftView.AddGestureRecognizer(labelTap);


			var buttons = new List<UIButton> ();
			//buttons.AddUtilityButton ("More", UIColor.LightGray);
			buttons.AddUtilityButton ("Star", UIColor.Blue);

			tableView.RowHeight = GetHeightForRow(tableView, indexPath);
			var cell = tableView.DequeueReusableCell (listViewCell.Key) as listViewCell;

			if (cell == null) {
				cell = new listViewCell (items [indexPath.Row],tableView,buttons,leftView );
			} else {
				cell.UpdateCell (items [indexPath.Row], tableView, buttons, leftView,indexPath);
			}

			buttons[0].TouchUpInside += (object sender, EventArgs e) => {
				service.StarListing(searchID,items[indexPath.Row]._id).ContinueWith((task) => InvokeOnMainThread(() =>
					{
						new UIAlertView("Result Starred","Result Starred",null,"OK",null).Show();
						cell.HideSwipedContent(true);
					}));
			};
				
			//reload
			if (indexPath.Section + 1 == NumberOfSections (tableView) 
				&& RowsInSection (tableView, indexPath.Section) == indexPath.Row + 1
				&& !updating
				&& startCount >= 20
				&& items.Count > 0
				&& moreResults) 
			{

				updating = true;
				UpdateItems (tableView);
			}

			cell.Tag = indexPath.Row;
			cell.SizeToFit ();
			cell.IndentationLevel = 0; 



			return cell;
		}


			

		public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			return 395;
		}

		public override float GetHeightForHeader (UITableView tableView, int section)
		{
			return 0.0f;
		}
		public override bool CanEditRow (UITableView tableView, NSIndexPath indexPath)
		{
			return true; // return false if you wish to disable editing for a specific indexPath or for all rows
		}
		public override bool CanMoveRow (UITableView tableView, NSIndexPath indexPath)
		{
			return false; // return false if you don't allow re-ordering
		}

		public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			switch (editingStyle) {
			case UITableViewCellEditingStyle.Delete:
				tableView.BeginUpdates ();
				// remove the item from the underlying data source
				items.RemoveAt(indexPath.Row);
				// delete the row from the tabsle
				tableView.DeleteRows (new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
				tableView.ReloadData ();
				tableView.EndUpdates ();
				break;
			case UITableViewCellEditingStyle.None:
				Console.WriteLine ("CommitEditingStyle:None called");
				break;
			}
		}
	
		public override string TitleForDeleteConfirmation (UITableView tableView, NSIndexPath indexPath)
		{   // Optional - default text is 'Delete'
			return "Trash (" + items[indexPath.Row].title + ")";
		}



		//when click row on ListView
		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			var touch = new UITouch ();
			var cell = GetCell (tableView, indexPath);

			//toolBar frame
			var testFrame = new RectangleF (0, 390 - 35, cell.ContentView.Frame.Width, 35);


			//trying to trap event of toolbar
			if(testFrame.Contains(touch.LocationInView (cell.ContentView))){

			}

			cell.SetEditing(true,true);
			tableView.DeselectRow (indexPath, true); 
			app.ShowDetail (searchID,items [indexPath.Row]);

		}



		void OnScrolling (object sender, ScrollingEventArgs e)
		{

			//uncomment to close any other cells that are open when another cell is swiped
			/*
				if (e.CellState != SWCellState.Center) {
					var paths = this.controller.TableView.IndexPathsForVisibleRows;
					foreach (var path in paths) {
						if(path.Equals(e.IndexPath))
						   continue;
						var cell = (SWTableViewCell)this.controller.TableView.CellAt (path);
						if (cell.State != SWCellState.Center) {
							cell.HideSwipedContent (true);
						}
					}
				}
				*/
		}

		void OnButtonPressed (object sender, CellUtilityButtonClickedEventArgs e)
		{
			if (e.UtilityButtonIndex ==  1) {
				new UIAlertView("Pressed", "You pressed the edit button!", null, null, new[] {"OK"}).Show();
			}
			else if(e.UtilityButtonIndex == 0){
				new UIAlertView("Pressed", "You pressed the more button!", null, null, new[] {"OK"}).Show();
			}
		}



		public void UpdateItems(UITableView tableView)
		{
			var list = items.Select (x => x._id).ToArray ();

			if (string.IsNullOrEmpty (binID)) {

				service.GetListingsAsync (searchID, list).ContinueWith ((task) => InvokeOnMainThread (() => {
					//tableView.BeginUpdates();
					if(!task.IsFaulted){
						listResults = task.Result.Result;
						var newRes = listResults.listings;
						if(newRes.Count > 0){
							items.AddRange (listResults.listings);
						
							updating = false;
							tableView.ReloadData ();

						}else{
							moreResults = false;
						}
					}
					//tableView.EndUpdates();
					

			
				}));

			} else {
		
				service.GetBinsListingAsync (binID, list).ContinueWith ((task) => InvokeOnMainThread (() => {
					//dont change more results flag to retry BANG BANG BITCH
					if(!task.IsFaulted){
						var tmp = task.Result.Result;
						if(tmp.Count > 0){
							items.AddRange (tmp);

							updating = false;
							tableView.ReloadData ();

						}else{
							moreResults = false;
						}
					}


				}));
			}
		}



	}
}

