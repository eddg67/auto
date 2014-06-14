using System;
using MonoTouch.UIKit;
using AutoLink.Models;
using System.Threading.Tasks;
using System.Net.Http;
using MonoTouch.Foundation;
using System.Drawing;

namespace AutoLink
{
	public class Detail : UIView
	{
		public Listing item { get; set; }
		public UIImageView ImageView { get; set; }
		UILabel price,desc,make,mileage,source;
		string searchID { get; set; }
		AppDelegate app = (AppDelegate)UIApplication.SharedApplication.Delegate;

		public Detail (RectangleF frame) : base (frame)
		{

	

		}

		public void setSearchID(string id){
			searchID = id;
		}

		public void setItem(Listing list){
			item = list;

			this.DownloadImageAsync (item.images [0]).ContinueWith ((task) => InvokeOnMainThread (() => {
				//DetailTextLabel.Text = list.description;
				if (!task.IsFaulted) {

					var img = task.Result;
					var imgClick = new UIImageViewClickable();
					imgClick.Image = img;
					ImageView = imgClick;
						//ImageView.Image = img;

					imgClick.OnClick += () => {
					
						app.ShowImageController(item);
					
					};


					ImageView.Frame = new RectangleF(0, 0 , Bounds.Width, 300f);

					desc = new UILabel(new RectangleF(0, ImageView.Frame.Bottom + 60, Bounds.Size.Width -10, 100));
					desc.TextAlignment = UITextAlignment.Center;
					desc.LineBreakMode = UILineBreakMode.WordWrap;
					desc.Font = UIFont.PreferredCaption1;
					desc.Lines = 0;
					desc.Text = string.Empty;
					desc.Text = list.description;

					price = new UILabel(new RectangleF(0, ImageView.Frame.Bottom, Bounds.Size.Width, 20));
					price.LineBreakMode = UILineBreakMode.WordWrap;
					price.Font = UIFont.PreferredBody;
					price.Lines = 0;
					price.TextAlignment = UITextAlignment.Right;
					price.Text = string.Empty;
					price.Text = string.Format("$ {0}", list.price.ToString());

					make = new UILabel(new RectangleF( 0 , ImageView.Frame.Bottom, Bounds.Size.Width, 20));
					make.LineBreakMode = UILineBreakMode.WordWrap;
					make.Font = UIFont.PreferredBody;
					make.Lines = 0;
					make.TextAlignment = UITextAlignment.Left;
					make.Text = string.Empty;
					make.Text = list.title;

					mileage = new UILabel(new RectangleF( 0 , make.Frame.Bottom, Bounds.Size.Width, 20));
					mileage.Font = UIFont.PreferredBody;
					mileage.Lines = 1;
					mileage.TextAlignment = UITextAlignment.Left;
					mileage.Text = string.Empty;
					mileage.Text = string.Format("Mileage : {0} mi",list.mileage.ToString());

					source = new UILabel(new RectangleF( 0 , mileage.Frame.Bottom, Bounds.Size.Width, 20));
					source.Font = UIFont.PreferredBody;
					source.Lines = 1;
					source.TextAlignment = UITextAlignment.Left;
					source.Text = string.Empty;
					source.Text = string.Format("Source : {0}",list.source);

					//dialog = GetDialog(list);
					Add(ImageView);
					Add(make);
					Add(price);
					Add(mileage);
					Add(source);
					Add(desc);

				}


			}));
		}
	
		/*public async Task<UIImage> DownloadImageAsync(string imageUrl)
		{
			var httpClient = new HttpClient();

			Task <Byte[]> contentsTask = httpClient.GetByteArrayAsync(imageUrl);

			var contents = await contentsTask;

			return UIImage.LoadFromData(NSData.FromArray(contents));
		}*/
	}
}

