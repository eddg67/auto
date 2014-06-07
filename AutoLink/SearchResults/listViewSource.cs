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
		public string searchID;
		AppDelegate app = (AppDelegate)UIApplication.SharedApplication.Delegate;
		SearchService service; 
		bool updating = false;


		public listViewSource (string id)
		{
	
			service = app.searchService;
			searchID = id;
		
			listResults = service.GetListings (id); 
			items = listResults.listings;

			if(items == null){
				using(var alert = new UIAlertView("Listing Fetch Failure","Listing Service not Responding, Please try again.",null,"OK",null))
				{
					alert.Show ();
				}
				listResults = service.GetListings (id); 
				items = listResults.listings;
			}


			//ount = listResults.Count;
			//Head
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
				Frame = new RectangleF (0, 0, SWTableViewCell.UtilityButtonsWidthMax, tableView.RowHeight),
				BackgroundColor = UIColor.Red,
				Text = "Peekaboo!",
				TextColor = UIColor.White,
				TextAlignment = UITextAlignment.Center
			};

			var buttons = new List<UIButton> ();
			buttons.AddUtilityButton ("More", UIColor.LightGray);
			buttons.AddUtilityButton ("Edit", UIColor.Blue);

			tableView.RowHeight = GetHeightForRow(tableView, indexPath);

			var cell = new listViewCell (items [indexPath.Row],tableView,buttons,leftView );
			//tableView.DequeueReusableCell (listViewCell.Key) as listViewCell;

			/*if (cell == null) {
				cell = new listViewCell (items [indexPath.Row],tableView,buttons,leftView );
			} else {
				cell.UpdateCell (items [indexPath.Row],tableView,buttons,leftView );
			}*/

			cell.SizeToFit ();

			if (indexPath.Row + 10 > items.Count && !updating) {
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
			service.GetListingsAsync (searchID, list).ContinueWith((task) => InvokeOnMainThread(() =>
				{

					listResults = task.Result.Result;
					items.AddRange(listResults.listings);
				
					var c = tableView.NumberOfRowsInSection(1);
					updating = false;
					tableView.ReloadData ();

			
				}));
		}



	}
}

