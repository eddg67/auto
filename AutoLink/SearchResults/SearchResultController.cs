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

		Section secSearch;

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
			navigation.View.Frame = View.Bounds;//UIScreen.MainScreen.Bounds;
			navigation.NavigationTableView.Frame = new RectangleF (View.Bounds.X, 66f, View.Bounds.Width, View.Bounds.Height);
			//navigation.NavigationTableView.BackgroundColor = UIColor.Black;

			AddChildViewController (navigation);

			service.GetResultsAsync ().ContinueWith (
				(task) => InvokeOnMainThread (() => {

					if(!task.IsFaulted && task.Result != null){
					results = task.Result.Result;

					View.AddSubview (navigation.View);
					var searchIds = results.Select (x => x.id).ToArray ();

					int count = 0;
					navigation.ViewControllers = Array.ConvertAll (
						results.Select (x => x.name).ToArray (), 
						title =>{
						
							var list = new ListViewController (navigation, title,searchIds[count],false);
							var nav = new UINavigationController (list);
							nav.NavigationBarHidden = false;
							
							count++;

							return nav;
							}
					);
						
					LoadBin();
					count = 0;
					}
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
						app.storage.Put<Bin>("Bins",bins);
						app.setUpLocalNotifications(bins.@new.count);

						var vc = navigation.ViewControllers;

						var vcArr = new UIViewController [] {
							new UINavigationController ( new ListViewController (navigation,"Starred",bins.starred.id,true)),
							new UINavigationController ( new ListViewController (navigation,"New",bins.@new.id,true)),
							new UINavigationController ( new ListViewController (navigation,"Contacted",bins.contacted.id,true )),
							new UINavigationController (new ListViewController (navigation,"Deleted",bins.deleted.id,true))
						};

						if(bins.custom != null && bins.custom.Count > 0){
							var custs = bins.custom.Select(x=>{
								return new UINavigationController ( new ListViewController (navigation,x.name,x.id,true));
							});
								
							vcArr = vcArr.Concat(custs.ToArray()).ToArray(); 
						}



						var tmp = vc.Concat(vcArr).ToArray();  
						navigation.ViewControllers = tmp;

						navigation.NavigationRoot = new RootElement ("Live Searches"){
							new RootElement ("Live Searches"){
							GetSearchSection ()
							},
							UpdateBins(bins)

						};
						//navigation.NavigationTableView.TableHeaderView = CreateToolbarView();
						//navigation.NavigationRoot.Add(GetSearchSection());
						//navigation.NavigationRoot.Add(UpdateBins(bins));

					}
				}));

		}

		public override void LoadView ()
		{
			base.LoadView ();

			over = new LoadingOverlay (View.Bounds,"Loading Results...");
			View.Add (over);

		}
			

		private Section GetSearchSection()
		{

			secSearch = new Section (CreateToolbarView (), null);

			secSearch.Caption = "Live Search";

			secSearch.AddAll (
				results.Select 
				(x => {

					var str = new StyledStringElement (
						x.name,
						x.newListingsCount.ToString(),
						UITableViewCellStyle.Value1
					);
					str.Font = UIFont.FromName("Clan-Book", 12f);
					str.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
					str.Tapped += () => {
						navigation.Title = x.name;
					};

					str.AccessoryTapped += () => {
						app.ShowSearch(x);
					};

					return str;

				})
			);

		return secSearch;
		}

		public Section UpdateBins(Bin bin)
		{
			Section result;
			//StyledStringElement[] customBins;

			var header = new UILabel (new RectangleF (0, 0, this.View.Bounds.Width, 60)) {
				Font = UIFont.FromName("Clan-Book", 16f),
				TintColor = UIColor.LightTextColor,
				BackgroundColor = UIColor.LightGray,
				Alpha=0.5f,
				Text = "Bins",
				ClipsToBounds = false
			};
					

			var stared = new StyledStringElement (
				             "Starred",
				(bin.starred != null) ? bin.starred.count.ToString () : "0",
				UITableViewCellStyle.Value1
			);
			stared.Font = UIFont.FromName("Clan-Book", 12f);
			stared.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			stared.Tapped += () => {navigation.Title =  "Starred";};
			//stared.Image = UIImage.FromBundle ("binicon_star.png");
		
			var allNew = new StyledStringElement (
				"All New",
				(bin.@new != null)?bin.@new.count.ToString():"0",
				UITableViewCellStyle.Value1
			);

			allNew.Font = UIFont.FromName("Clan-Book", 12f);
			allNew.Tapped += () => {navigation.Title =  "New";};
			allNew.Accessory = UITableViewCellAccessory.DisclosureIndicator;

			var contacted = new StyledStringElement (
				"Contacted",
				(bin.contacted != null)?bin.contacted.count.ToString():"0",
				UITableViewCellStyle.Value1
			);

			contacted.Font = UIFont.FromName("Clan-Book", 12f);
			contacted.Tapped += () => {navigation.Title =  "Contacted";};
			contacted.Accessory = UITableViewCellAccessory.DisclosureIndicator;

			var deleted = new StyledStringElement (
				"Deleted Listing",
				(bin.deleted != null)?bin.deleted.count.ToString():"0",
				UITableViewCellStyle.Value1
			);

			deleted.Font = UIFont.FromName("Clan-Book", 12f);
			deleted.Tapped += () => {navigation.Title =  "Deleted Listings";};
			deleted.Accessory = UITableViewCellAccessory.DisclosureIndicator;

			result = new Section (header, null) {
				stared, allNew, contacted, deleted

			};

			if (bin.custom != null || bin.custom.Count > 0) 
			{
				result.AddAll( bin.custom.Select (x => {
					var str = new StyledStringElement (
						x.name,
						x.count.ToString(),
						UITableViewCellStyle.Value1
					);

					str.Font = UIFont.FromName("Clan-Book", 12f);
					str.Tapped += () => {navigation.Title =  x.name;};
					deleted.Accessory = UITableViewCellAccessory.DisclosureIndicator;

					return str;
				}));
			}


			return result;
				
		}

		public UIToolbar CreateToolbarView()
		{
			var tool = new UIToolbar (new RectangleF (0, 0, View.Bounds.Width - 10, 66));
			tool.BarTintColor = UIColor.Black;
			tool.TintColor = UIColor.White;

			//tool.Layer.BackgroundColor = UIColor.Black.CGColor;
			var btn = new UIBarButtonItem (UIBarButtonSystemItem.Add, (sender, args) => {
				// button was clicked
				ShowActionPicker();
			}){TintColor = UIColor.White};
			btn.ImageInsets = new UIEdgeInsets(16.5f,20,0,0);

			UITextAttributes attr = new UITextAttributes ();
			attr.TextColor = UIColor.White;
			attr.Font =  UIFont.FromName("Clan-Book", 14f);
			btn.SetTitleTextAttributes (attr,UIControlState.Normal);

			tool.SetItems (new UIBarButtonItem[]{ 
				btn,
				new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
				new UIBarButtonItem("autolink",UIBarButtonItemStyle.Plain,null){
				},
				new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
			},true);

			return tool;
		}

		public void ShowActionPicker()
		{
			var action = new ActionSheetPicker (View) {
				Title = "Update"
			};
			var model = new ListPicker<string> (
				            new List<string> { 
					"New Live Search", "Add New Bin"
				});
			action.Show (model);

			action.doneButton.TouchUpInside += (sender, e) => {
				if (model.SelectedItem.Contains ("New Live Search")) {
					app.ShowSearch ();
				} else {
					var newBin = new EntryElement (string.Empty, "Enter Bin Name", string.Empty);
					navigation.NavigationRoot.Last<Section> ().Add (newBin);

					newBin.BecomeFirstResponder(true);
					newBin.EntryEnded += (s1, e1) => {
						var name = newBin.Value;
						if (!string.IsNullOrEmpty (name)) {

							service.AddBin (name)
									.ContinueWith ((task) => InvokeOnMainThread (() => {
									if (!task.IsFaulted) {
										LoadBin ();
									}
								}
							));
						}
					};
				}
			};
		}


	}
}

