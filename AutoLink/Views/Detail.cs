using System;
using MonoTouch.UIKit;
using AutoLink.Models;
using System.Threading.Tasks;
using System.Net.Http;
using MonoTouch.Foundation;
using System.Drawing;
using Alliance.Carousel;

namespace AutoLink
{
	public class Detail : UIView
	{
		public Listing item { get; set; }
		public UIToolbar tool{ get; set; }
		public UIImageViewClickable ImageView { get; set; }
		public UIViewController controller { get; set; }

		UILabel price, desc, make, mileage, source;

		string searchID { get; set; }

		float offset = 10;
		AppDelegate app = (AppDelegate)UIApplication.SharedApplication.Delegate;

		public Detail (RectangleF frame,UIViewController _controller) : base (frame)
		{
			ImageView = new UIImageViewClickable ();
			desc = new UILabel ();
			price = new UILabel ();
			make = new UILabel ();
			mileage = new UILabel ();
			source = new UILabel ();
			controller = _controller;
			controller.NavigationController.NavigationBarHidden = false;
			controller.NavigationController.ToolbarHidden = false;
		}

		public void setSearchID (string id)
		{
			searchID = id;
		}

		public void setItem (Listing list)
		{
			item = list;

			this.DownloadImageAsync (item.images [0]).ContinueWith ((task) => InvokeOnMainThread (() => {
				//DetailTextLabel.Text = list.description;
				if (!task.IsFaulted) {

				
					if (!task.IsFaulted) {
						ImageView.Frame = new RectangleF (0, 0, Frame.Width, 280);
						ImageView.Image = task.Result;
						Add (ImageView);
					}
			
					desc.TextAlignment = UITextAlignment.Center;
					desc.LineBreakMode = UILineBreakMode.WordWrap;
					desc.Font = UIFont.PreferredCaption1;
					desc.Lines = 0;
					desc.Text = string.Empty;
					desc.Text = list.description;


					price.LineBreakMode = UILineBreakMode.WordWrap;
					price.Font = UIFont.PreferredBody;
					price.Lines = 0;
					price.TextAlignment = UITextAlignment.Right;
					price.Text = string.Empty;
					price.Text = string.Format ("$ {0}", list.price.ToString ());


					make.LineBreakMode = UILineBreakMode.WordWrap;
					make.Font = UIFont.PreferredBody;
					make.Lines = 0;
					make.TextAlignment = UITextAlignment.Left;
					make.Text = string.Empty;
					make.Text = list.title;


					mileage.Font = UIFont.PreferredBody;
					mileage.Lines = 1;
					mileage.TextAlignment = UITextAlignment.Left;
					mileage.Text = string.Empty;
					mileage.Text = string.Format ("Mileage : {0} mi", list.mileage.ToString ());

					source.Font = UIFont.PreferredBody;
					source.Lines = 1;
					source.TextAlignment = UITextAlignment.Left;
					source.Text = string.Empty;
					source.Text = string.Format ("Source : {0}", list.source);

					ImageView.OnClick += () => {
						ImageView.RemoveGestureRecognizer(ImageView.grTap);
						app.ShowImageController (item);
					};
				}


			}));
		}

		public override void WillMoveToWindow (UIWindow window)
		{
			base.WillMoveToWindow (window);
			if (controller.NavigationController != null) {
				controller.NavigationController.NavigationBarHidden = false;
			}

		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			ClipsToBounds = true;

			ImageView.Frame = new RectangleF (0, 0, Frame.Width, 280);

			price.Frame = new RectangleF (offset, ImageView.Frame.Bottom, Bounds.Width - (offset * 2), 30);

			make.Frame = new RectangleF (offset, ImageView.Frame.Bottom, Bounds.Width * 0.70f, 30);

			mileage.Frame = new RectangleF (offset, make.Frame.Bottom + offset, Bounds.Size.Width - offset, 20);
			source.Frame = new RectangleF (offset, mileage.Frame.Bottom, Bounds.Width - offset, 20);

			desc.Frame = new RectangleF (offset, source.Frame.Bottom + 40, Bounds.Width - (offset * 2), 50);

			var carousel = new CarouselView (new RectangleF (0, ImageView.Frame.Bottom, Bounds.Width, 50));
			carousel.DataSource = new LinearDataSource (item);
			carousel.Delegate = new LinearDelegate ();
			carousel.CarouselType = CarouselType.Linear;
			//carousel.ConfigureView();
			//Add(carousel);
			//carousel.Autoscroll = 1.0f;

			Add (make);
			Add (price);
			Add (mileage);
			Add (source);

			using (var line = new LineView (new RectangleF (offset, source.Frame.Bottom + (offset * 2), Bounds.Width - (offset * 2), 1))) {
				line.BackgroundColor = UIColor.LightGray;
				Add (line);
			}


			Add (desc);

			using (var line = new LineView (new RectangleF (offset, desc.Frame.Bottom + (offset * 2), Bounds.Width - (offset * 2), 1))) {
				line.BackgroundColor = UIColor.LightGray;
				Add (line);
			}

			Add (GetToolBar ());

		}

		UIToolbar GetToolBar ()
		{
			UITextAttributes attr = new UITextAttributes ();
			string local = "Location Not Available";
			string datesOn = "Unknown List Date";
			string pricesAbove = "Price Above Edmunds";

			tool = new UIToolbar (new RectangleF (0, Frame.Height - 55, Frame.Width, 55));  
			//tool.Translucent = true;
			tool.Layer.BorderColor = UIColor.White.CGColor;
			tool.BarTintColor = UIColor.White;
			tool.ClipsToBounds = true;

			attr.Font = UIFont.SystemFontOfSize (9);
			attr.TextColor = UIColor.LightGray;

			//add location and change font color
			if (item.address.city != null) {
				local = string.Format ("{0},{1}", item.address.city, item.address.state);
				attr.TextColor = UIColor.Blue;

			} 

			var location = new UIBarButtonItem (local, UIBarButtonItemStyle.Plain, (s, e) => {
				//for event

			});
			location.SetTitleTextAttributes (attr, UIControlState.Normal);

			if (item.created != null || item.updated != null) {

				datesOn = string.Format ("Listed {0} days ago", CalculateDateDiff ());
				attr.TextColor = UIColor.LightGray;

			} else if (item.deleted) {

				datesOn = "Listing Removed";
				attr.TextColor = UIColor.Red;
			}

			var timeSpan = new UIBarButtonItem (datesOn, UIBarButtonItemStyle.Plain, (s, e) => {
				//for event
			});
			timeSpan.SetTitleTextAttributes (attr, UIControlState.Normal);


			var price = new UIBarButtonItem (pricesAbove, UIBarButtonItemStyle.Plain, delegate {
				if (item.pricing != null) {
					app.ShowPriceEdmunds (item.pricing);
				}
			});
			if (item.pricing != null) {
				attr.TextColor = UIColor.Blue;
			}
			price.SetTitleTextAttributes (attr, UIControlState.Normal);
			price.Tag = 22;
			

			var bbs = new UIBarButtonItem[] {
				location,
				new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace),

				timeSpan,
				new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace),

				price

			};


			tool.SetItems (bbs, true);

			return tool;
		}

		int CalculateDateDiff ()
		{
			int result = 0;

			try {

				var res = item.updated - item.created;
				result = res.Value.Days;

			} catch (Exception exp) {

				Console.WriteLine (exp.Message);
			}
			return result;
		}

	

	}
}

