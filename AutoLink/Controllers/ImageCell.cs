using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Drawing;
using AutoLink.Models;
using System.Threading.Tasks;
using System.Net.Http;

namespace AutoLink
{
	public class ImageCell : UICollectionViewCell
	{
		public static NSString CellID = new NSString("UserSource");

		[Export("initWithFrame:")]
		public ImageCell(RectangleF frame)
			: base(frame)
		{
			ImageView = new UIImageView();
			ImageView.Layer.BorderColor = UIColor.DarkGray.CGColor;
			ImageView.Layer.BorderWidth = 1f;
			ImageView.Layer.CornerRadius = 3f;
			ImageView.Layer.MasksToBounds = true;
			ImageView.ContentMode = UIViewContentMode.ScaleToFill;

			ContentView.AddSubview(ImageView);

			LabelView = new UILabel();
			LabelView.BackgroundColor = UIColor.Clear;
			LabelView.TextColor = UIColor.DarkGray;
			LabelView.TextAlignment = UITextAlignment.Center;

			ContentView.AddSubview(LabelView);
		}

		public UIImageView ImageView { get; set; }

		public int index { get; set; }

		public UILabel LabelView { get; set; }
		public NSAction Tapped { get; set; }

		public void UpdateRow(string element, Single fontSize, SizeF imageViewSize)
		{

			DownloadImageAsync (element).ContinueWith ((task) => InvokeOnMainThread (() => {
				//DetailTextLabel.Text = list.description;
				if (!task.IsFaulted) {

					ImageView.Image = task.Result;
					ImageView.Frame = new RectangleF(0, 0, imageViewSize.Width, imageViewSize.Height);
				

				}


			}));
	


		
		}

		public async Task<UIImage> DownloadImageAsync(string imageUrl)
		{
			var httpClient = new HttpClient();

			Task <Byte[]> contentsTask = httpClient.GetByteArrayAsync(imageUrl);

			var contents = await contentsTask;

			return UIImage.LoadFromData(NSData.FromArray(contents));
		}



	}


}

