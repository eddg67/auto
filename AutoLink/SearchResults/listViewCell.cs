using System;
using System.Linq;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using System.Net.Http;
using System.Threading.Tasks;
using AutoLink.Models;
using System.Collections.Generic;

namespace AutoLink
{
	public class listViewCell : SWTableViewCell
	{
		public static readonly NSString Key = new NSString ("listViewCell");
		private Listing item;
		DialogViewController dialog;
		UILabel price,desc,make,mileage,source;

		public listViewCell (Listing list,UITableView tableView, List<UIButton> rightsBtns, UIView leftView)
			:base(UITableViewCellStyle.Value1, Key,tableView,rightsBtns,leftView)
		{
		
			item = list;

			ContentView.AutosizesSubviews = true;
			UpdateCell (item, tableView, rightsBtns, leftView,null);

		}

		public void UpdateCell (Listing list,UITableView tableView, List<UIButton> rightsBtns, UIView leftView,NSIndexPath indexPath)
		{

			//if(testCell != null){
				ContentView.RemoveAllSubViews ();
				desc = new UILabel();
				price = new UILabel();
				make = new UILabel();
				mileage = new UILabel();
				source = new UILabel();
			//ImageView.Image = null;


				DownloadImageAsync(list.images[0]).ContinueWith((task) => InvokeOnMainThread(() =>
				{
					var cellShown = tableView.IndexPathsForVisibleRows.ToList().Select(x=>x.Equals(indexPath));
					//DetailTextLabel.Text = list.description;
					if(!task.IsFaulted){
						//ImageView.Image = null;
						ImageView.Image = task.Result;
					}

					//desc = new UILabel(new RectangleF(0, ImageView.Frame.Bottom + 60, ContentView.Bounds.Size.Width -10, 100));
				
					desc.TextAlignment = UITextAlignment.Center;
					desc.LineBreakMode = UILineBreakMode.TailTruncation;
					desc.Font = UIFont.PreferredCaption1;
					desc.Lines = 0;
					desc.Text = string.Empty;
					desc.Text = list.descriptionCollapsed;

					//price = new UILabel(new RectangleF(0, ImageView.Frame.Bottom, ContentView.Bounds.Size.Width, 20));

					price.LineBreakMode = UILineBreakMode.WordWrap;
					price.Font = UIFont.PreferredBody;
					price.Lines = 0;
					price.TextAlignment = UITextAlignment.Right;
					price.Text = string.Empty;
					price.Text = string.Format("$ {0}", list.price.ToString());

					//make = new UILabel(new RectangleF( 0 , ImageView.Frame.Bottom, ContentView.Bounds.Size.Width, 20));
				
					make.LineBreakMode = UILineBreakMode.TailTruncation;
					make.Font = UIFont.PreferredBody;
					make.Lines = 0;
					make.TextAlignment = UITextAlignment.Left;
					make.Text = string.Empty;
					make.Text = list.title;

					//mileage = new UILabel(new RectangleF( 0 , make.Frame.Bottom, ContentView.Bounds.Size.Width, 20));
				
					mileage.Font = UIFont.PreferredBody;
					mileage.Lines = 1;
					mileage.TextAlignment = UITextAlignment.Left;
					mileage.Text = string.Empty;
					mileage.Text = string.Format("Mileage : {0} mi",list.mileage.ToString());

					//source = new UILabel(new RectangleF( 0 , mileage.Frame.Bottom, ContentView.Bounds.Size.Width, 20));
				
					source.Font = UIFont.PreferredBody;
					source.Lines = 1;
					source.TextAlignment = UITextAlignment.Left;
					source.Text = string.Empty;
					source.Text = string.Format("Source : {0}",list.source);


				}));
			//}
		}
		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			ClipsToBounds = true;

			ImageView.Frame = new RectangleF(0, 0, ContentView.Bounds.Width, 180);
			price.Frame = new RectangleF(0, ImageView.Frame.Bottom , ContentView.Bounds.Width, 30);
			make.Frame = new RectangleF (0, ImageView.Frame.Bottom, ContentView.Bounds.Width, 20);
			mileage.Frame = new RectangleF( 0 , make.Frame.Bottom, ContentView.Bounds.Size.Width, 20);
			source.Frame = new RectangleF( 0 , mileage.Frame.Bottom, ContentView.Bounds.Width, 20);
			desc.Frame = new RectangleF (0, ImageView.Frame.Bottom + 60, ContentView.Bounds.Width - 10, 100);
			SelectionStyle = UITableViewCellSelectionStyle.None;
			//DetailTextLabel.Frame = new RectangleF(0, 180, Bounds.Size.Width, 180);
			EditingAccessory = UITableViewCellAccessory.DetailButton;

			ContentView.Add(make);
			ContentView.Add(price);
			ContentView.Add(mileage);
			ContentView.Add(source);
			ContentView.Add(desc);
			ContentView.Add(GetToolBar());
		}
			

		public async Task<UIImage> DownloadImageAsync(string imageUrl)
		{
			var httpClient = new HttpClient();

			Task <Byte[]> contentsTask = httpClient.GetByteArrayAsync(imageUrl);

			var contents = await contentsTask;

			return UIImage.LoadFromData(NSData.FromArray(contents));
		}

		UIToolbar GetToolBar()
		{
			var tool = new UIToolbar (new RectangleF (0 , ContentView.Frame.Height - 35 , ContentView.Frame.Width, 35));  
			//tool.TintColor = UIColor.Clear;
			//tool.Layer.BorderWidth = 0;



			var location = new UIBarButtonItem (UIBarButtonSystemItem.Action);


			var timeSpan = new UIBarButtonItem (UIBarButtonSystemItem.Organize);
		

			var price = new UIBarButtonItem (UIBarButtonSystemItem.Stop);

		
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
			


		private void HandleSwipeRight()
		{
			// load another view
			Console.WriteLine("Swipe right.");
		}

		private DialogViewController GetDialog(Listing item)
		{
			var prRow = new StyledStringElement (
				string.Format("Make: {0}",item.make),
				string.Format("${0}",item.price.ToString()),
				UITableViewCellStyle.Value1
			);

			var mileSource = new StyledStringElement (
				string.Format("Mileage : {0} mi",item.mileage.ToString()),
				string.Format("source: {0}",item.source),
				     UITableViewCellStyle.Subtitle
			   );

			var line = new UIViewElement ("", new UIView (new RectangleF (10, 0, ContentView.Bounds.Size.Width-20, 1)){
				BackgroundColor = UIColor.DarkGray
			}, false);

			//var desc = new MultilineElement ("", item.description);


			//svar carImg = new UIViewElement ("",img,false);

			var root = new RootElement ("RootCell") {
				new Section () {
					prRow,mileSource,line,

				}
			};


			var vc = new DialogViewController (root);

			vc.View.Frame = new RectangleF (0, 200, ContentView.Bounds.Size.Width, 200);

			return vc;

		}
			
			
	}
}

