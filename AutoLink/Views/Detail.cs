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
		UIViewController controller;
		UILabel price,desc,make,mileage,source;
		string searchID { get; set; }
		float offset = 10;
		AppDelegate app = (AppDelegate)UIApplication.SharedApplication.Delegate;

		public Detail (RectangleF frame,UIViewController _controller) : base ()
		{
			ImageView = new UIImageViewClickable();
			desc = new UILabel();
			price = new UILabel ();
			make = new UILabel ();
			mileage = new UILabel ();
			source = new UILabel();
			controller = _controller;
			ContentMode = UIViewContentMode.ScaleAspectFit;
		
			//AutosizesSubviews = true;

		}

		public void setSearchID(string id){
			searchID = id;
		}

		public void setItem(Listing list){
			item = list;

			this.DownloadImageAsync (item.images [0]).ContinueWith ((task) => InvokeOnMainThread (() => {
				//DetailTextLabel.Text = list.description;
				if (!task.IsFaulted) {

				
					if(!task.IsFaulted){

						ImageView.Image = task.Result;
						ImageView.ContentMode = UIViewContentMode.ScaleAspectFill;
						Add(ImageView);
					}
		
					desc.TextAlignment = UITextAlignment.Center;
					desc.LineBreakMode = UILineBreakMode.WordWrap;
					desc.Font =  UIFont.FromName("Clan-Book", 10f);
					desc.Lines = 0;
					desc.Text = string.Empty;
					desc.Text = list.descriptionCollapsed;

					//price = new UILabel(new RectangleF(0, ImageView.Frame.Bottom, ContentView.Bounds.Size.Width, 20));

					price.LineBreakMode = UILineBreakMode.WordWrap;
					price.Font = UIFont.FromName("Clan-Medium", 12f);
					price.Lines = 0;
					price.TextAlignment = UITextAlignment.Right;
					price.Text = string.Empty;
					if(!string.IsNullOrEmpty(list.price)){
						price.Text = string.Format("$ {0}", list.price);
					}else{
						price.Text = "No Price Available";
					}			

					//make = new UILabel(new RectangleF( 0 , ImageView.Frame.Bottom, ContentView.Bounds.Size.Width, 20));

					make.LineBreakMode = UILineBreakMode.TailTruncation;
					make.Font =  UIFont.FromName("Clan-Medium", 12f);
					make.Lines = 1;
					make.TextAlignment = UITextAlignment.Left;
					make.Text = string.Empty;
					make.Text = list.title;

					//mileage = new UILabel(new RectangleF( 0 , make.Frame.Bottom, ContentView.Bounds.Size.Width, 20));

					mileage.Font = UIFont.FromName("Clan-Book", 12f);
					mileage.Lines = 1;
					mileage.TextAlignment = UITextAlignment.Left;
					mileage.Text = string.Empty;
					if(string.IsNullOrEmpty(list.mileage)){
						mileage.Text = "No Mileage Available";
					}else{
						mileage.Text = string.Format("Mileage : {0} mi",list.mileage);
					}

					source.Font = UIFont.FromName("Clan-Book", 12f);
					source.Lines = 1;
					source.TextAlignment = UITextAlignment.Left;
					source.Text = string.Empty;
					source.Text = string.Format("Source : {0}",list.source);

					ImageView.OnClick += () => {
						((DetailViewController)controller).removeNavigation = false;
						ImageView.RemoveGestureRecognizer(ImageView.grTap);
						app.ShowImageController(item);

					};

					//LayoutSubviews ();

						
				}


			}));
		}

	 

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			ClipsToBounds = true;

			ImageView.Frame = new RectangleF(0, 0 , Frame.Width, 300);

			price.Frame = new RectangleF(offset, ImageView.Frame.Bottom , Bounds.Width - (offset*2), 30);

			make.Frame = new RectangleF (offset, ImageView.Frame.Bottom, Bounds.Width * 0.70f, 30);

			mileage.Frame = new RectangleF( offset , make.Frame.Bottom + offset , Bounds.Size.Width - offset, 20);
			source.Frame = new RectangleF( offset , mileage.Frame.Bottom, Bounds.Width  -offset, 20);

			AddSubview(make);
			AddSubview(price);
			AddSubview(mileage);
			AddSubview(source);
		
			using (var line = new LineView (new RectangleF (offset, source.Frame.Bottom + offset, Bounds.Width - (offset * 2), 1))) {
				line.BackgroundColor = UIColor.LightGray;
				//desc.InsertSubviewAbove (line,);
				AddSubview (line);
				if (desc.Text != null) {
					var s = desc.StringSize (desc.Text, UIFont.FromName ("Clan-Book", 12f),Frame.Size, UILineBreakMode.WordWrap);
					desc.Frame = new RectangleF (offset, line.Frame.Bottom + offset, Bounds.Width - (offset * 2), s.Height);
				}
			
			}


			AddSubview(desc);


			var h = ImageView.Frame.Height + price.Frame.Height + mileage.Frame.Height + source.Frame.Height + desc.Frame.Height + 50;
			this.Frame = new RectangleF(0,0,Frame.Width,(float)Math.Ceiling(h));
			//Frame = f;

			if (desc.Frame.Height > 0) {

				using (var line = new LineView (new RectangleF (offset, desc.Frame.Bottom + offset, Bounds.Width - (offset * 2), 1))) {
					line.BackgroundColor = UIColor.LightGray;
					AddSubview (line);
				}
				GetToolBar ();

				AddSubview (tool);
			}
		}


		public override SizeF SizeThatFits (SizeF size)
		{
			return base.SizeThatFits (size);
		}

		UIToolbar GetToolBar()
		{
			UITextAttributes attr = new UITextAttributes ();
			string local = "Location Not Available";
			string datesOn = "Unknown List Date";
			string pricesAbove = "Price Above Edmunds";

			tool = new UIToolbar (new RectangleF (0 , Frame.Height - 35 , Frame.Width, 35));  
			//tool.Translucent = true;
			tool.Layer.BorderColor = UIColor.White.CGColor;
			tool.BarTintColor = UIColor.White;
			tool.ClipsToBounds = true;

			attr.Font =  UIFont.FromName("Clan-Medium", 9f);
			attr.TextColor = UIColor.LightGray;

			//add location and change font color
			if (item.address.city != null) {
				local = string.Format ("{0},{1}", item.address.city, item.address.state);
				attr.TextColor = UIColor.Blue;

			} 

			var location = new UIBarButtonItem (local, UIBarButtonItemStyle.Plain, (s,e)=>{
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

			var timeSpan = new UIBarButtonItem (datesOn, UIBarButtonItemStyle.Plain, (s,e)=>{
				//for event
			});
			timeSpan.SetTitleTextAttributes (attr, UIControlState.Normal);


			var price = new UIBarButtonItem (pricesAbove, UIBarButtonItemStyle.Plain, delegate {
				if(item.pricing != null){
					app.ShowPriceEdmunds(item.pricing);
				}
			});
			if (item.pricing != null) {
				attr.TextColor = UIColor.Blue;
			}
				price.SetTitleTextAttributes (attr, UIControlState.Normal);
				price.Tag = 22;
			

			var bbs = new UIBarButtonItem[] {
				location,
				new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),

				timeSpan,
				new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),

				price

			};


			tool.SetItems (bbs, true);

			return tool;
		}

		int CalculateDateDiff()
		{
			int result = 0;

			try{

				var res =  item.updated - item.created;
				result = res.Value.Days;

			}catch(Exception exp){

				Console.WriteLine (exp.Message);
			}
			return result;
		}

	

	}
}

