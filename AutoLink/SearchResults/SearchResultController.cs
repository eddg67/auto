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
using PerpetualEngine.Storage;

namespace AutoLink
{
	public partial class SearchResultController : UIViewController
	{

		AppDelegate app = (AppDelegate)UIApplication.SharedApplication.Delegate;
		SearchService service;
		LoadingOverlay over;
		SimpleStorage storage;
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
			storage = SimpleStorage.EditGroup ("SearchResult");
			UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.LightContent;
			RestorationIdentifier = "SearchResult";


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

			AddChildViewController (navigation);

			//get results from cache
			results = storage.Get<List<SearchResult>> ("getResults");

			if (results != null) {
				UpdateResults (results);
				//over.Hide ();
			} else {
				LoadResults ();
			}
		}


		public void LoadResults()
		{
			service.GetResultsAsync ().ContinueWith (
				(task) => InvokeOnMainThread (() => {

					if(!task.IsFaulted && task.Result != null){

						results = task.Result.Result;
						storage.Put<List<SearchResult>>("getResults",results);
						UpdateResults(results);

					}else{

						over.Hide ();
					}

				}
				));
		}

		public void UpdateResults(List<SearchResult> newResult)
		{

			View.AddSubview (navigation.View);
			var searchIds = newResult.Select (x => x.id).ToArray ();

			int count = 0;
			navigation.ViewControllers = Array.ConvertAll (
				newResult.Select (x => x.name).ToArray (), 
				title =>{

					var list = new ListViewController (navigation, title, searchIds[count],false);
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

		void UpdateBinResults()
		{
		}




		public void LoadBin()
		{
			//bins = storage.Get<Bin> ("Bins");

			service.GetBins ().ContinueWith((task) => InvokeOnMainThread(() =>
				{
					bins = task.Result.Result;
					if(bins != null){
						storage.Put<Bin>("Bins",bins);
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

		public override UIStatusBarStyle PreferredStatusBarStyle ()
		{
			return UIStatusBarStyle.LightContent;
		}
			

		private Section GetSearchSection()
		{

			secSearch = new Section (CreateToolbarView (), null);

			var header = new UILabel (new RectangleF (0, 0, this.View.Bounds.Width, 80)) {
				Font = UIFont.FromName("Clan-Book", 16f),
				TintColor = UIColor.LightTextColor,
				BackgroundColor = UIColor.LightGray,
				Alpha=0.5f,
				Text = "    Live Search",
				ClipsToBounds = false
			};

			var cogImg = UIImage.FromBundle ("cog.png");

			var assBtn = new UIButton (new RectangleF (0, 0, cogImg.Size.Width, cogImg.Size.Height));
			assBtn.SetBackgroundImage (cogImg, UIControlState.Normal);
			header.Tag = 0;

			secSearch.Add (new UIViewElement ("", header, true));

			secSearch.AddAll (
				results.Select 
				(x => {

					var str = new StyledStringElement (
						x.name,
						x.newListingsCount.ToString(),
						UITableViewCellStyle.Value1
					){Font = UIFont.FromName("Clan-Book", 12f)};

					str.TextColor = UIColor.LightGray;

					str.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
					str.Tapped += () => {
						navigation.Title = string.Format("{0}({1})",x.name,x.newListingsCount);
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

			var header = new UILabel (new RectangleF (0, 0, this.View.Bounds.Width, 80)) {
				Font = UIFont.FromName("Clan-Book", 16f),
				TintColor = UIColor.LightTextColor,
				BackgroundColor = UIColor.LightGray,
				Alpha=0.5f,
				Text = "    Bins",
				ClipsToBounds = false
			};	

			var stared = new BinElement (
				 "Starred",
				(bin.starred != null) ? bin.starred.count.ToString () : "0",
				UITableViewCellStyle.Value1
			);
			stared.Font = UIFont.FromName("Clan-Book", 12f);
			stared.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			stared.Tapped += () => {navigation.Title =  "Starred";};
			stared.Image = UIImage.FromBundle ("binicon_star.png");
		
			var allNew = new BinElement (
				"All New",
				(bin.@new != null)?bin.@new.count.ToString():"0",
				UITableViewCellStyle.Value1
			);

			allNew.Font = UIFont.FromName("Clan-Book", 12f);
			allNew.Tapped += () => {navigation.Title =  "New";};
			allNew.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			allNew.Image = UIImage.FromBundle ("binicon_allnew.png");

			var contacted = new BinElement (
				"Contacted",
				(bin.contacted != null)?bin.contacted.count.ToString():"0",
				UITableViewCellStyle.Value1
			);

			contacted.Font = UIFont.FromName("Clan-Book", 12f);
			contacted.Tapped += () => {navigation.Title =  "Contacted";};
			contacted.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			contacted.Image = UIImage.FromBundle ("binicon_contacted.png");

			var deleted = new BinElement (
				"Deleted Listing",
				(bin.deleted != null)?bin.deleted.count.ToString():"0",
				UITableViewCellStyle.Value1
			);

			deleted.Font = UIFont.FromName("Clan-Book", 12f);
			deleted.Tapped += () => {navigation.Title =  "Deleted Listings";};
			deleted.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			deleted.Image = UIImage.FromBundle ("binicon_deleted.png");

			result = new Section (header, null) {
				stared, allNew, contacted, deleted

			};

			if (bin.custom != null || bin.custom.Count > 0) 
			{
				result.AddAll( bin.custom.Select (x => {
					var str = new BinElement (
						x.name,
						x.count.ToString(),
						UITableViewCellStyle.Value1
					);

					str.Font = UIFont.FromName("Clan-Book", 12f);
					str.Tapped += () => {navigation.Title =  string.Format("{0}({1})",x.name,x.count.ToString());};
					str.Image = UIImage.FromBundle ("binicon_usercreatedbin.png");
					str.Accessory = UITableViewCellAccessory.DisclosureIndicator;

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
				ShowOptionMenu();
			}){TintColor = UIColor.White};
			btn.ImageInsets = new UIEdgeInsets(15f,10,0,0);

			UITextAttributes attr = new UITextAttributes ();
			attr.TextColor = UIColor.White;
			attr.Font =  UIFont.FromName("Clan-Book", 14f);
			btn.SetTitleTextAttributes (attr,UIControlState.Normal);

			var autoHeader = new UIBarButtonItem ("autolink", UIBarButtonItemStyle.Plain, null);
			attr.Font =  UIFont.FromName("Clan-Book", 18f);
			autoHeader.SetTitleTextAttributes(attr,UIControlState.Normal);

			tool.SetItems (new UIBarButtonItem[]{ 
				btn,
				new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
				autoHeader,
				new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
			},true);

			return tool;
		}

		void ShowOptionMenu()
		{
			var action = new UIActionSheet ();
			var searchIndex = action.AddButton ("New Live Search");
			var binIndex = action.AddButton ("Add New Bin");
			var cancelIndex = action.AddButton ("Cancel");

			//styling
			//action.DestructiveButtonIndex = searchIndex;
			action.CancelButtonIndex = cancelIndex;
			//action.Opaque = true;
			action.Style = UIActionSheetStyle.BlackTranslucent;

			var searchBtn = (UIButton)action.Subviews[searchIndex];
			searchBtn.SetImage (UIImage.FromBundle ("binicon_usercreatedbin.png"),UIControlState.Normal);
			searchBtn.ImageEdgeInsets = new UIEdgeInsets (0, 0, 0, 0);
			searchBtn.TitleLabel.Font = UIFont.FromName("Clan-Book", 10f);
			searchBtn.SetTitleColor (UIColor.Black, UIControlState.Normal);
			searchBtn.BackgroundColor = UIColor.Clear;
			searchBtn.ContentEdgeInsets = new UIEdgeInsets (0, 0, 0, 0);
			searchBtn.Font = UIFont.FromName("Clan-Book", 10f);
			searchBtn.ImageView.Frame = new RectangleF (0, 0, 0, 0);


			var binBtn = (UIButton)action.Subviews[binIndex];
			binBtn.SetImage (UIImage.FromBundle ("binicon_usercreatedbin.png"),UIControlState.Normal);
			binBtn.ImageEdgeInsets = new UIEdgeInsets (0, 0, 0, 0);
			binBtn.TitleLabel.Font = UIFont.FromName("Clan-Book", 10f);
			binBtn.SetTitleColor (UIColor.Black, UIControlState.Normal);

			binBtn.BackgroundColor = UIColor.Clear;
			binBtn.ContentEdgeInsets = new UIEdgeInsets (0, 0, 0, 0);
			binBtn.Font = UIFont.FromName("Clan-Book", 10f);

			action.Clicked += (s, e) => { 
				Console.WriteLine ("Clicked on item {0}", e.ButtonIndex); 

				if(e.ButtonIndex == searchIndex){
					app.ShowSearch ();
				}else if(e.ButtonIndex == binIndex){
					AddNewBin();
				}
			
			};

			action.ShowInView (View);

		}

		void AddNewBin(){
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
			


	}
}

