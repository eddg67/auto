using System;
using System.Linq;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using AutoLink.Models;
using System.Collections.Generic;
using MonoTouch.Dialog;
using Autolink;

namespace AutoLink
{
	public class ListViewController : UITableViewController
	{
		public string searchId  { get; set; }
		public bool useBinId  { get; set; }

		public DetailViewController DetailViewController {
			get;
			set;
		}

		UIView activeview{ get; set; }            // Controller that activated the keyboard
		float scrollamount = 0.0f;    // amount to scroll 
		float offset = 10.0f;          // extra offset
		bool moveViewUp { get; set; }

		public ListViewController (FlyoutController navigation, string title,string id,bool bin) : base (UITableViewStyle.Grouped)
		{
	
			searchId = id;
			useBinId = bin;
			//TableView.TableHeaderView = GetHeader ();
			TableView.SeparatorInset = new UIEdgeInsets (0, 0, 0, 0);
			TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;

			TableView.ContentInset = new UIEdgeInsets (-25, 0, 0, 0); 


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

	

		public override void ViewWillLayoutSubviews()
		{
			var frame = TableView.Frame;
			TableView.Frame = new RectangleF (0, 0, frame.Width, frame.Height);
			TableView.ClipsToBounds = false;
			//View.Frame = new RectangleF (0, 20, frame.Width, frame.Height); 
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

			var attr = new UITextAttributes ();
			attr.Font =  UIFont.FromName("Clan-Book", 14f);
			btn.SetTitleTextAttributes (attr,UIControlState.Application);
			//btn.SetTitleTextAttributes(

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
				Font = UIFont.FromName ("Clan-Bold", 30f)

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

