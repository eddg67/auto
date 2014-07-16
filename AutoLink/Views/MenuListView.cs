using System;
using Autolink;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Drawing;

namespace AutoLink
{
	public class MenuListView: ListViewController
	{
		UIView activeview{ get; set; }            // Controller that activated the keyboard
		float scrollamount = 0.0f;    // amount to scroll 
		float bottom = 0.0f;           // bottom point
		float offset = 10.0f;          // extra offset
		bool moveViewUp { get; set; }

		public MenuListView (FlyoutController navigation, string title,string list, bool bin=false): base (navigation, title,list,bin)
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

