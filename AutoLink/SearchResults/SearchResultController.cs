using System;
using System.Linq;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Autolink;
using AutoLink.Services;
using AutoLink.Models;
using System.Collections.Generic;
using MonoTouch.Dialog;
using BigTed;

namespace AutoLink
{
	public partial class SearchResultController : UIViewController
	{

		AppDelegate app = (AppDelegate)UIApplication.SharedApplication.Delegate;
		SearchService service;
		public Bin bins;
		public FlyoutController binsController;
		public List<SearchResult> results;

		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public SearchResultController ()
		//: base (UserInterfaceIdiomIsPhone ? "SearchResultController_iPhone" : "SearchResultController_iPad", null)
		{
			service = app.searchService;

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

			//BTProgressHUD.Show ("Searching in...");
			results = service.GetResults ();

			binsController = new FlyoutController ();

			binsController.Position = FlyOutNavigationPosition.Left;
			binsController.View.Frame = UIScreen.MainScreen.Bounds;
			//View.AddSubview (CreateToolbarView ());
			View.AddSubview (binsController.View);
			this.AddChildViewController (binsController);


			var name = results.Select (x => x.name).ToArray ();
			var searchIds = results.Select (x => x.id).ToArray ();

			int count = 0;
			binsController.ViewControllers = Array.ConvertAll (name, title =>{
				var nav = new UINavigationController (new Bins (binsController, title,searchIds[count],false));
				nav.NavigationBarHidden = false;
				count++;

				return nav;
			});

			service.GetBins ().ContinueWith((task) => InvokeOnMainThread(() =>
				{
					bins = task.Result.Result;
					if(bins != null){
						app.setUpLocalNotifications(bins.@new.count);
						binsController.NavigationTableView.SectionHeaderHeight = 0;
						binsController.NavigationTableView.TableHeaderView = null;
						var vc = binsController.ViewControllers;

						var vcArr = new UIViewController [] {
							new UINavigationController ( new Bins (binsController,"Starred",bins.starred.id,true)),
							new UINavigationController ( new Bins (binsController,"New",bins.@new.id,true)),
							new UINavigationController ( new Bins (binsController,"Contacted",bins.contacted.id,true )),
							new UINavigationController (new Bins (binsController,"Deleted",bins.deleted.id,true))

							//new UINavigationController ( new Bins (binsController,"Seen",bins.seen.id,true))
						};

						var tmp = vc.Concat(vcArr).ToArray();			
						binsController.ViewControllers = tmp;

						binsController.NavigationRoot = new RootElement ("Live Searches"){
							new RootElement("Live Search"){GetSearchSection ()},
							UpdateBins(bins)
						};

						//rootElement.Add (UpdateBins(bins));
					}
						
				
				}));

			//UIApplication.SharedApplication.Windows[0].RootViewController = binsController;
		}

		public override void LoadView ()
		{
			base.LoadView ();
			BTProgressHUD.Dismiss ();

		}

		private Section GetSearchSection()
		{

			var vals = new Section ("Live Searches");

			vals.AddAll (results.Select 
				(x => {
					var str = new StyledStringElement (
						x.name,
						x.newListingsCount.ToString(),
						UITableViewCellStyle.Value1
					);
	
					str.Accessory = UITableViewCellAccessory.DisclosureIndicator;
					str.Tapped += () => {binsController.Title = x.name;};

					return str;

				})
			);
			return vals;

		}

		public Section UpdateBins(Bin bin)
		{

			var header = new UILabel (new RectangleF (0, 0, this.View.Bounds.Width, 60)) {
				Font = UIFont.SystemFontOfSize(18),
				BackgroundColor = UIColor.LightGray,
				Text = "Bins"
			};

			var stared = new StyledStringElement (
				             "Starred",
				(bin.starred != null) ? bin.starred.count.ToString () : "0",
				UITableViewCellStyle.Value1
			);

			stared.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			using (var fav = new UITabBarItem (UITabBarSystemItem.Favorites, 1)) {
				stared.Image = fav.SelectedImage;
			}

			stared.Tapped += () => {binsController.Title =  "Starred";};

			var allNew = new StyledStringElement (
				"All New",
				(bin.@new != null)?bin.@new.count.ToString():"0",
				UITableViewCellStyle.Value1
			);

			allNew.Tapped += () => {binsController.Title =  "New";};
			allNew.Accessory = UITableViewCellAccessory.DisclosureIndicator;

			using (var fav = new UITabBarItem (UITabBarSystemItem.MostRecent, 1)) {
				allNew.Image = fav.SelectedImage;
			}

			var contacted = new StyledStringElement (
				"Contacted",
				(bin.contacted != null)?bin.contacted.count.ToString():"0",
				UITableViewCellStyle.Value1
			);

			contacted.Tapped += () => {binsController.Title =  "Contacted";};
			contacted.Accessory = UITableViewCellAccessory.DisclosureIndicator;

			using (var fav = new UITabBarItem (UITabBarSystemItem.Recents, 1)) {
				contacted.Image = fav.SelectedImage;
			}

			var deleted = new StyledStringElement (
				"Deleted Listing",
				(bin.deleted != null)?bin.deleted.count.ToString():"0",
				UITableViewCellStyle.Value1
			);

			deleted.Tapped += () => {binsController.Title =  "Deleted Listings";};
			deleted.Accessory = UITableViewCellAccessory.DisclosureIndicator;

			using (var fav = new UITabBarItem (UITabBarSystemItem.History, 1)) {
				deleted.Image = fav.Image;
			}

			return new Section ("Bins", null) {
				stared,allNew,contacted,deleted

			};
				
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

		class Bins : listViewController
		{
			public Bins (FlyoutController navigation, string title,string list, bool bin=false): base (list,bin)
			{
				var fav = new UITabBarItem(UITabBarSystemItem.Contacts,1);
				var img = fav.SelectedImage;
				this.Title = title;	

				navigation.NavigationTableView.TableHeaderView = new UIView (new RectangleF (0, 0, 320, 0)) {
					BackgroundColor = UIColor.Blue
				};
				//navigation.NavigationTableView.TableHeaderView.Add(CreateToolbarView());
						
				NavigationItem.RightBarButtonItem = new UIBarButtonItem (fav.SelectedImage,UIBarButtonItemStyle.Plain, delegate {
					using(var app = (AppDelegate)UIApplication.SharedApplication.Delegate){
						app.ShowSearch();
					}
				});
				NavigationItem.LeftBarButtonItem = new UIBarButtonItem (UIBarButtonSystemItem.Action, delegate {
					navigation.ToggleMenu ();
				});

				//navigation.NavigationController.NavigationBar.TintColor = UIColor.Black;


		
			}


		}
	}
}

