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
		LoadingOverlay over;
		public Bin bins;
		public FlyoutController navigation;
		public List<SearchResult> results;

		Section searchSec;


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

			navigation = new FlyoutController ();
			navigation.Position = FlyOutNavigationPosition.Left;
			navigation.View.Frame = UIScreen.MainScreen.Bounds;
			AddChildViewController (navigation);

			service.GetResultsAsync ().ContinueWith (
				(task) => InvokeOnMainThread (() => {

					results = task.Result.Result;

					View.AddSubview (navigation.View);
					var searchIds = results.Select (x => x.id).ToArray ();

					int count = 0;
					navigation.ViewControllers = Array.ConvertAll (
						results.Select (x => x.name).ToArray (), 
						title =>{
						
							var list = new ListView (navigation, title,searchIds[count],false);
							var nav = new UINavigationController (list);
							nav.NavigationBarHidden = false;
							count++;

							return nav;
							}
					);
						
					LoadBin();

					count = 0;
					over.Hide ();


				}
			));


		
		}


		public void LoadBin()
		{
			service.GetBins ().ContinueWith((task) => InvokeOnMainThread(() =>
				{
					bins = task.Result.Result;
					if(bins != null){

						app.setUpLocalNotifications(bins.@new.count);
						navigation.NavigationTableView.SectionHeaderHeight = 0;
						navigation.NavigationTableView.TableHeaderView = null;

						var vc = navigation.ViewControllers;

						var vcArr = new UIViewController [] {
							new UINavigationController ( new ListView (navigation,"Starred",bins.starred.id,true)),
							new UINavigationController ( new ListView (navigation,"New",bins.@new.id,true)),
							new UINavigationController ( new ListView (navigation,"Contacted",bins.contacted.id,true )),
							new UINavigationController (new ListView (navigation,"Deleted",bins.deleted.id,true))

							//new UINavigationController ( new Bins (navigation,"Seen",bins.seen.id,true))
						};

						var tmp = vc.Concat(vcArr).ToArray();            
						navigation.ViewControllers = tmp;

						navigation.NavigationRoot = new RootElement ("Live Searches"){
							GetSearchSection (),
							UpdateBins(bins)

						};


						//navigation.NavigationRoot.Add(GetSearchSection());
						//navigation.NavigationRoot.Add(UpdateBins(bins));

					}


				}));

		}

		public override void LoadView ()
		{
			base.LoadView ();

			over = new LoadingOverlay (View.Bounds,"Searching Results...");
			View.Add (over);

		}

		private Section GetSearchSection()
		{

			var header = new UILabel (new RectangleF (0, 0, this.View.Bounds.Width, 40)) 
			{
				Font = UIFont.SystemFontOfSize(18),
				BackgroundColor = UIColor.LightTextColor,
				Text = "Live Searches"
			};

			var secSearch = new Section (CreateToolbarView(),null)
			{
				//new UIViewElement("",header,true)
			};
			secSearch.AddAll (
				results.Select 
				(x => {

					var str = new StyledStringElement (
						x.name,
						x.newListingsCount.ToString(),
						UITableViewCellStyle.Value1
					);
				
					str.Accessory = UITableViewCellAccessory.DisclosureIndicator;
					str.Tapped += () => {
						navigation.Title = x.name;
					};

					return str;

				})
			);

		return secSearch;
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
			stared.Tapped += () => {navigation.Title =  "Starred";};

			var allNew = new StyledStringElement (
				"All New",
				(bin.@new != null)?bin.@new.count.ToString():"0",
				UITableViewCellStyle.Value1
			);

			allNew.Tapped += () => {navigation.Title =  "New";};
			allNew.Accessory = UITableViewCellAccessory.DisclosureIndicator;

			var contacted = new StyledStringElement (
				"Contacted",
				(bin.contacted != null)?bin.contacted.count.ToString():"0",
				UITableViewCellStyle.Value1
			);

			contacted.Tapped += () => {navigation.Title =  "Contacted";};
			contacted.Accessory = UITableViewCellAccessory.DisclosureIndicator;

			var deleted = new StyledStringElement (
				"Deleted Listing",
				(bin.deleted != null)?bin.deleted.count.ToString():"0",
				UITableViewCellStyle.Value1
			);

			deleted.Tapped += () => {navigation.Title =  "Deleted Listings";};
			deleted.Accessory = UITableViewCellAccessory.DisclosureIndicator;

			return new Section (header, null) {
				stared,allNew,contacted,deleted

			};
				
		}

		public UIToolbar CreateToolbarView()
		{
			var tool = new UIToolbar (new RectangleF (0, 5, 320, 60));
			tool.BackgroundColor = UIColor.Black;
			tool.TintColor = UIColor.Black;
			var btn = new UIBarButtonItem (UIBarButtonSystemItem.Add, (sender, args) => {
				// button was clicked
				app.ShowSearch();
			});
				

			tool.SetItems (new UIBarButtonItem[]{ 
				btn,
				new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
				new UIBarButtonItem("Autolink",UIBarButtonItemStyle.Plain,null),
				new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
			},true);

			return tool;
		}

		class ListView : listViewController
		{
			public ListView (FlyoutController navigation, string title,string list, bool bin=false): base (list,bin)
			{
				var fav = new UITabBarItem(UITabBarSystemItem.Contacts,1);
				var img = fav.SelectedImage;
				this.Title = title;	

				navigation.NavigationTableView.SectionHeaderHeight = 0f;
						
				NavigationItem.RightBarButtonItem = new UIBarButtonItem (fav.SelectedImage,UIBarButtonItemStyle.Plain, delegate {
					using(var app = (AppDelegate)UIApplication.SharedApplication.Delegate){
						app.ShowSearch();
					}
				});
				NavigationItem.LeftBarButtonItem = new UIBarButtonItem (UIBarButtonSystemItem.Action, delegate {
					navigation.ToggleMenu ();
				});
					
			}


		}
	}
}

