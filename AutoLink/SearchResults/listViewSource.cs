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

		public DetailViewController DetailViewController {
			get;
			set;
		}

		public listViewSource (string id,string binId = "")
		{
	
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
			

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			//add edits
			var leftView = new UILabel () {
				Frame = new RectangleF (0, 0, 50, tableView.RowHeight/2),
				BackgroundColor = UIColor.Red,
				Text = "Delete Listing!",
				TextColor = UIColor.White,
				TextAlignment = UITextAlignment.Center
			};

			var buttons = new List<UIButton> ();
			//buttons.AddUtilityButton ("More", UIColor.LightGray);
			buttons.AddUtilityButton ("Star", UIColor.Blue);



			buttons[0].TouchUpInside += (object sender, EventArgs e) => {

				service.StarListing(searchID,items[indexPath.Row]._id);
			};

			tableView.RowHeight = GetHeightForRow(tableView, indexPath);

			//var cell = new listViewCell (items [indexPath.Row],tableView,buttons,leftView );
			var cell = tableView.DequeueReusableCell (listViewCell.Key) as listViewCell;

			if (cell == null) {
				cell = new listViewCell (items [indexPath.Row],tableView,buttons,leftView );
			} else {
				cell.UpdateCell (items [indexPath.Row],tableView,buttons,leftView );
			}
				
			cell.SizeToFit ();

			if (indexPath.Row + 10 > items.Count && !updating && startCount >= 20) {
				updating = true;
				UpdateItems (tableView);
			
			}

			return cell;
		}
			

		public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
	
			return 400;
		}

		public override float GetHeightForHeader (UITableView tableView, int section)
		{
			return 0.0f;
		}



		//when click row on ListView
		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			new UIAlertView("Row Selected", indexPath.Row.ToString(), null, "OK", null).Show();
			var cell = GetCell (tableView, indexPath);
			cell.SetEditing(true,true);
			tableView.DeselectRow (indexPath, true); // iOS convention is to remove the highlight


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

		void UpdateItems(UITableView tableView)
		{
			var list = items.Select (x => x._id).ToArray ();

			if (string.IsNullOrEmpty (binID)) {

				service.GetListingsAsync (searchID, list).ContinueWith ((task) => InvokeOnMainThread (() => {

					listResults = task.Result.Result;
					items.AddRange (listResults.listings);
				
					updating = false;
					tableView.ReloadData ();

			
				}));

			} else {
		
				service.GetBinsListingAsync (binID, list).ContinueWith ((task) => InvokeOnMainThread (() => {

					var tmp = task.Result.Result;
					items.AddRange (tmp);

					updating = false;
					tableView.ReloadData ();


				}));
			}
		}



	}
}

