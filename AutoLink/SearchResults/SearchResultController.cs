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

		static float scrollamount = 0.0f;    // amount to scroll 
		static float bottom = 0.0f;           // bottom point
		static float offset = 10.0f;          // extra offset
		static bool moveViewUp { get; set; }


		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public SearchResultController ()
		//: base (UserInterfaceIdiomIsPhone ? "SearchResultController_iPhone" : "SearchResultController_iPad", null)
		{
			service = app.searchService;
			moveViewUp = false;

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

					if(!task.IsFaulted && task.Result != null){
					results = task.Result.Result;

					View.AddSubview (navigation.View);
					var searchIds = results.Select (x => x.id).ToArray ();

					int count = 0;
					navigation.ViewControllers = Array.ConvertAll (
						results.Select (x => x.name).ToArray (), 
						title =>{
						
							var list = new ListView (navigation, title,searchIds[count],false);
								list.TableView.TableHeaderView = null;
								list.TableView.SectionHeaderHeight = 0f;
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
						navigation.NavigationTableView.SectionHeaderHeight = 0;
						navigation.NavigationTableView.TableHeaderView = null;

						var vc = navigation.ViewControllers;

						var vcArr = new UIViewController [] {
							new UINavigationController ( new ListView (navigation,"Starred",bins.starred.id,true)),
							new UINavigationController ( new ListView (navigation,"New",bins.@new.id,true)),
							new UINavigationController ( new ListView (navigation,"Contacted",bins.contacted.id,true )),
							new UINavigationController (new ListView (navigation,"Deleted",bins.deleted.id,true))
						};

						if(bins.custom != null && bins.custom.Count > 0){
							var custs = bins.custom.Select(x=>{
								return new UINavigationController ( new ListView (navigation,x.name,x.id,true));
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

			secSearch = new Section (CreateToolbarView(),null)
			{
				//new UIViewElement("",header,true)
			};

			secSearch.Caption = "Live Search";
			//secSearch.Header = "Live Search";


			secSearch.AddAll (
				results.Select 
				(x => {

					var str = new StyledStringElement (
						x.name,
						x.newListingsCount.ToString(),
						UITableViewCellStyle.Value1
					);
				
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
				Font = UIFont.FromName("Clan-Bold", 16f),
				TintColor = UIColor.LightTextColor,
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

					str.Tapped += () => {navigation.Title =  x.name;};
					deleted.Accessory = UITableViewCellAccessory.DisclosureIndicator;

					return str;
				}));
			}


			return result;
				
		}

		public UIToolbar CreateToolbarView()
		{
			var tool = new UIToolbar (new RectangleF (20, 0, View.Bounds.Width - 10, 66));
			tool.BackgroundColor = UIColor.Black;
			tool.TintColor = UIColor.Black;

			tool.Layer.BackgroundColor = UIColor.Black.CGColor;
			var btn = new UIBarButtonItem (UIBarButtonSystemItem.Add, (sender, args) => {
				// button was clicked
				ShowActionPicker();
			}){TintColor = UIColor.Black};



			tool.SetItems (new UIBarButtonItem[]{ 
				btn,
				new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
				new UIBarButtonItem("Autolink",UIBarButtonItemStyle.Plain,null){
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




		///sub class
		class ListView : listViewController
		{
			UIView activeview{ get; set; }            // Controller that activated the keyboard
			 float scrollamount = 0.0f;    // amount to scroll 
			// float bottom = 0.0f;           // bottom point
			 float offset = 10.0f;          // extra offset
			 bool moveViewUp { get; set; }

			public ListView (FlyoutController navigation, string title,string list, bool bin=false): base (list,bin)
			{
				var fav = new UITabBarItem(UITabBarSystemItem.Contacts,1);
				var img = fav.SelectedImage;
				this.Title = title;	

				var topV = View.Subviews[0];
				topV.Layer.BackgroundColor = UIColor.White.CGColor;

				//navigation.RotatingHeaderView

				navigation.NavigationTableView.SectionHeaderHeight = 0f;
						
				NavigationItem.RightBarButtonItem = new UIBarButtonItem (fav.SelectedImage,UIBarButtonItemStyle.Plain, delegate {
					using(var app = (AppDelegate)UIApplication.SharedApplication.Delegate){
						app.ShowAccount();
					}
				});
				NavigationItem.LeftBarButtonItem = new UIBarButtonItem (UIBarButtonSystemItem.Action, delegate {
					navigation.ToggleMenu ();
				});

				NSNotificationCenter.DefaultCenter.AddObserver (UIKeyboard.WillHideNotification, OnKeyboardNotification);
				NSNotificationCenter.DefaultCenter.AddObserver (UIKeyboard.WillShowNotification, OnKeyboardNotification);
					
			}

			private void OnKeyboardNotification (NSNotification notification)
			{
				if (IsViewLoaded) {

					//Check if the keyboard is becoming visible
					bool visible = notification.Name == UIKeyboard.WillShowNotification;

					//Start an animation, using values from the keyboard
					UIView.BeginAnimations ("AnimateForKeyboard");
					UIView.SetAnimationBeginsFromCurrentState (true);
					UIView.SetAnimationDuration (UIKeyboard.AnimationDurationFromNotification (notification));
					UIView.SetAnimationCurve ((UIViewAnimationCurve)UIKeyboard.AnimationCurveFromNotification (notification));

					//Pass the notification, calculating keyboard height, etc.
					bool landscape = InterfaceOrientation == UIInterfaceOrientation.LandscapeLeft || InterfaceOrientation == UIInterfaceOrientation.LandscapeRight;
					if (visible) {
						var keyboardFrame = UIKeyboard.FrameEndFromNotification (notification);

						OnKeyboardChanged (visible, landscape ? keyboardFrame.Width : keyboardFrame.Height);
					} else {
						var keyboardFrame = UIKeyboard.FrameBeginFromNotification (notification);

						OnKeyboardChanged (visible, landscape ? keyboardFrame.Width : keyboardFrame.Height);
					}

					//Commit the animation
					UIView.CommitAnimations ();	
				}
			}

			/// <summary>
			/// Override this method to apply custom logic when the keyboard is shown/hidden
			/// </summary>
			/// <param name='visible'>
			/// If the keyboard is visible
			/// </param>
			/// <param name='height'>
			/// Calculated height of the keyboard (width not generally needed here)
			/// </param>
			protected virtual void OnKeyboardChanged (bool visible, float height)
			{

				// Find what opened the keyboard
				foreach (UIView view in this.View.Subviews) {
					if (view.IsFirstResponder)
						activeview = view;
				}

				// Bottom of the controller = initial position + height + offset      
				bottom = (View.Frame.Y + View.Frame.Height + offset);

				// Calculate how far we need to scroll
				scrollamount = View.Frame.Height + offset;//(height - (View.Frame.Size.Height - bottom)) ;
				if (visible) {
					// Perform the scrolling
					if (scrollamount > 0) {
						moveViewUp = true;
						ScrollTheView (moveViewUp);
					} else {
						moveViewUp = false;
					}
				}

			}

			public void KeyBoardDownNotification(NSNotification notification)
			{
				if(moveViewUp){ScrollTheView(false);}
			}

			public void ScrollTheView(bool move)
			{

				// scroll the view up or down
				UIView.BeginAnimations (string.Empty, System.IntPtr.Zero);
				UIView.SetAnimationDuration (0.3);

				RectangleF frame = View.Frame;

				if (move) {
					frame.Y -= scrollamount;
				} else {
					frame.Y += scrollamount;
					scrollamount = 0;
				}

				View.Frame = frame;
				UIView.CommitAnimations();
			}


		}
	}
}

