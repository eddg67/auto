using System;
using System.Linq;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using AutoLink.Models;
using AutoLink.Services;
using System.Threading.Tasks;

namespace AutoLink
{
	public partial class DetailViewController : UIViewController
	{
		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public string searchID { get; set; }
		public bool removeNavigation { get; set; }
			
		Listing items { get; set; }
		UIToolbar toolbar { get; set; }
		AppDelegate app = (AppDelegate)UIApplication.SharedApplication.Delegate;
		SearchService service; 
		UIActionSheet actionSheet { get; set; }
		UIScrollView scrollView { get; set; }
		UIView contentView;


		public DetailViewController (string _searchID,Listing _item)
			: base (UserInterfaceIdiomIsPhone ? "DetailViewController_iPhone" : "DetailViewController_iPad", null)
		{
			service = app.searchService;
			searchID = _searchID;
			items = _item;
			removeNavigation = true;
		}

		public DetailViewController (IntPtr pointer)
			: base (UserInterfaceIdiomIsPhone ? "DetailViewController_iPhone" : "DetailViewController_iPad", null)
		{
			service = app.searchService;

		}

		public void SetItems(Listing item)
		{
			items = item;

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

			BuildDetailToolbar();

		
			// Perform any additional setup after loading the view, typically from a nib.
		}

		public override void LoadView ()
		{
			base.LoadView ();

			NavigationItem.RightBarButtonItem = new UIBarButtonItem (){Title="Mark"};
			NavigationItem.RightBarButtonItem.Clicked += (sender, e)=>{
				actionSheet = new UIActionSheet ("Mark Results", null, "Cancel", null, null);

					int markViewed = 0,markAll=0;
					markViewed = actionSheet.AddButton("Mark as Viewed");
					markAll = actionSheet.AddButton("All? No Call found");

					actionSheet.DestructiveButtonIndex = 0; // red
					actionSheet.CancelButtonIndex = markViewed;  // black

					actionSheet.Clicked += (object s, UIButtonEventArgs btnEv) => {

						if(btnEv.ButtonIndex > 0){
							if(btnEv.ButtonIndex == markViewed){

								var list = new string[]{items._id};

								service.SeenListing(searchID,list).ContinueWith((task) => InvokeOnMainThread(() =>{
								using(var alert = new UIAlertView ("Listing Marked", "This Listing has been marked seen", null, "OK", null)){
										alert.Show();
									}
							}));

							}else if(btnEv.ButtonIndex == markAll){


							}
						}
					};
				actionSheet.ShowFrom(NavigationItem.RightBarButtonItem,true);//.ShowInView(View);

			};
		
			var detailView = new Detail (View.Frame,this);

			detailView.setItem (items);

			View.AddSubview (scrollView = new UIScrollView (View.Bounds));
			scrollView.Add (contentView = detailView);

			//View.Add (detailView);

		}

		public override void ViewDidLayoutSubviews ()
		{
			var bounds = View.Bounds;
			contentView.Frame = bounds;
			scrollView.ContentSize = bounds.Size;
			scrollView.Frame = bounds;
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			removeNavigation = true;
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			if (removeNavigation) {
				NavigationController.NavigationBarHidden = true;
				NavigationController.ToolbarHidden = true;
			}
		
		}

		void BuildDetailToolbar()
		{
			var fav = new UITabBarItem(UITabBarSystemItem.Favorites,1);
			var img = fav.SelectedImage;

			var starBtn = new UIBarButtonItem (img, UIBarButtonItemStyle.Plain, null);
			starBtn.Clicked += (object sender, EventArgs e) => {

				service.StarListing(searchID,items._id).ContinueWith((task) => InvokeOnMainThread(() =>{
					using(var alert = new UIAlertView ("Listing Marked", "This Listing has been Starred.", null, "OK", null)){
						alert.Show();
					}
				}));
			};

			var contact = new UIBarButtonItem (UIBarButtonSystemItem.Compose);
			//TODO actionsheet
			contact.Clicked += (object sender, EventArgs e) => {
				int emailIndex=0,phoneIndex=0;

				CreateActionSheet("Contact Owner");

				if(items.sellerContact.email != null){
					emailIndex = actionSheet.AddButton ("Email");
				}

				if(items.sellerContact.phone != null){
					phoneIndex = actionSheet.AddButton("Phone "+ items.sellerContact.phone);
				}

				actionSheet.Clicked += (object s, UIButtonEventArgs btnEv) => {

					if(btnEv.ButtonIndex > 0){
						if(btnEv.ButtonIndex == emailIndex){

							app.SendEmail(items.sellerContact.email, items.title);

						}else if(btnEv.ButtonIndex == phoneIndex){

							app.SendPhone(items.sellerContact.phone);
						}
					}
				};
				actionSheet.ShowFromToolbar(NavigationController.Toolbar);

			};

			var delete = new UIBarButtonItem (UIBarButtonSystemItem.Trash);
			delete.Clicked += (object sender, EventArgs e) => {
				int deleteIndex=0;
				actionSheet = CreateActionSheet("Delete Listing, Are you Sure?");

				deleteIndex = actionSheet.AddButton ("Delete");

				actionSheet.DestructiveButtonIndex = 1; // red
				actionSheet.CancelButtonIndex = 0;  // black

				actionSheet.Clicked += (object s, UIButtonEventArgs btnEv) => {
					if(btnEv.ButtonIndex == deleteIndex){
						//BigTed.BTProgressHUD.Show("Deleting Item...");
						service.DeleteItem(searchID,items);
						app.searchResult.navigation.NavigationTableView.ReloadData();
						NavigationController.PopViewControllerAnimated(true);

					}

				};

				actionSheet.ShowFromToolbar(NavigationController.Toolbar);
			};

			var action = new UIBarButtonItem (UIBarButtonSystemItem.Action);
			action.Clicked += (object sender, EventArgs e) => {
		
				actionSheet = CreateActionSheet("Menu");

				int addTo = actionSheet.AddButton ("Add listing to Bin");
				int shareTo = actionSheet.AddButton ("Share listing");
				int goTo = actionSheet.AddButton ("Go to listing");
				actionSheet.BackgroundColor = UIColor.Clear;

				actionSheet.Clicked += (object s, UIButtonEventArgs btnEv) => {

					if(btnEv.ButtonIndex == shareTo){

						app.SendEmail("","Found Great listing On Autolink");

					}else if(btnEv.ButtonIndex == goTo){

						app.OpenUrl(items.url);

					}else if (btnEv.ButtonIndex == addTo){

						var picker = new ActionSheetPicker(View);
						Bin bins = app.storage.Get<Bin>("Bins");
						if(bins != null){
							var model = new ListPicker<string>(bins.custom.Select(x=>x.name).ToList());
							//TODO set value AND Send in Dimissed Call
							//returns selected value 
							model.PickerChanged += (object ss, EventArgs picke) => {
								var eve = (PickerChangedEventArgs)picke;
								Console.WriteLine(eve.SelectedItem);
								var res = bins.custom.Find(x=>x.name == eve.SelectedItem);
								service.AddListingToBin(res._id,items._id).ContinueWith(
									(task) => InvokeOnMainThread(() => {
										app.ShowResultList();
										picker.ActionSheet.EndEditing(true);
									}
									));
							};
							picker.Show(model);

							picker.ActionSheet.Dismissed += (object ss, UIButtonEventArgs test) => {
						
								var t = (UIActionSheet)ss;
							};

						}
						

					}

				};

				actionSheet.ShowFromToolbar(NavigationController.Toolbar);

			};



			var bbs = new UIBarButtonItem[] {
				new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
				starBtn,
				new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),

				contact,
				new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),

				delete,
				new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),

				action,
				new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace)
			};


			//this.NavigationController.Toolbar.Items = bbs;
		
			this.ToolbarItems = bbs;

			//this.NavigationController.SetToolbarItems (bbs, false);

		}


		UIActionSheet CreateActionSheet(string title){
			actionSheet = new UIActionSheet (title, null, "Cancel",null, null);
			return actionSheet;

		}


		/*
		[Export ("splitViewController:willHideViewController:withBarButtonItem:forPopoverController:")]
		public void WillHideViewController (UISplitViewController splitController, UIViewController viewController, UIBarButtonItem barButtonItem, UIPopoverController popoverController)
		{
			barButtonItem.Title = NSBundle.MainBundle.LocalizedString ("Master", "Master");
			NavigationItem.SetLeftBarButtonItem (barButtonItem, true);

		}

		[Export ("splitViewController:willShowViewController:invalidatingBarButtonItem:")]
		public void WillShowViewController (UISplitViewController svc, UIViewController vc, UIBarButtonItem button)
		{
			// Called when the view is shown again in the split view, invalidating the button and popover controller.
			NavigationItem.SetLeftBarButtonItem (null, true);

		}*/
	}
}

