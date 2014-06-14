using System;
using System.Linq;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using AutoLink.Models;
using System.Threading.Tasks;
using System.Net.Http;

namespace AutoLink
{
	public partial class ImageViewerViewController : UIViewController
	{
		const float indentX = 50;
		const float indentY = 10;
		const float imageHeight = 100;

		UIViewFullscreen vMain;

		UILabel lBigImage;
		UIImageViewClickable ivcThumbnail1;

		UILabel lSmallImage;
		UIImageViewClickable ivcThumbnail2;

		Listing item;

		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public ImageViewerViewController (Listing list)
			: base (UserInterfaceIdiomIsPhone ? "ImageViewerViewController_iPhone" : "ImageViewerViewController_iPad", null)
		{
			item = list;
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

		
			LoadImages ();


		}
			

		public async Task<UIImage> DownloadImageAsync(string imageUrl)
		{
			var httpClient = new HttpClient();

			Task <Byte[]> contentsTask = httpClient.GetByteArrayAsync(imageUrl);

			var contents = await contentsTask;

			return UIImage.LoadFromData(NSData.FromArray(contents));
		}

		void LoadImages()
		{


			DownloadImageAsync (item.images [0]).ContinueWith ((task) => InvokeOnMainThread (() => {
				//DetailTextLabel.Text = list.description;
				if (!task.IsFaulted) {
					lBigImage = new UILabel ();
					lBigImage.Text = "Big image";
					lBigImage.BackgroundColor = UIColor.Clear;
					lBigImage.SizeToFit ();
					lBigImage.Frame = new RectangleF (new PointF (View.Frame.Width / 2 - lBigImage.Frame.Width / 2, indentY), 
						lBigImage.Frame.Size);
					View.AddSubview (lBigImage);

					ivcThumbnail1 = new UIImageViewClickable ();
					ivcThumbnail1.Image = task.Result;
					ivcThumbnail1.ContentMode = UIViewContentMode.ScaleAspectFit;
					ivcThumbnail1.AutoresizingMask = UIViewAutoresizing.All;
					ivcThumbnail1.OnClick += () => {
						if (vMain == null) {
							vMain = new UIViewFullscreen();
						}
						vMain.SetImage(ivcThumbnail1.Image);
						vMain.Show();
					};

					lSmallImage = new UILabel ();
					lSmallImage.Text = "Small image";
					lSmallImage.BackgroundColor = UIColor.Clear;
					lSmallImage.SizeToFit ();
					lSmallImage.Frame = new RectangleF (new PointF (View.Frame.Width / 2 - lSmallImage.Frame.Width / 2, 
						ivcThumbnail1.Frame.Bottom + indentY), 
						lSmallImage.Frame.Size);
					View.AddSubview (lSmallImage);

					ivcThumbnail1.Frame = new RectangleF (indentX, lBigImage.Frame.Bottom + indentY, 
						View.Frame.Width - indentX * 2, imageHeight);
					//vMain.BackgroundColor = UIColor.Red; // Frame debug
					View.AddSubview (ivcThumbnail1);

					//LoadThumbNails ();

				}


			}));
				
		}

		public override void ViewDidLayoutSubviews ()
		{
			var bounds = View.Bounds;
			NavigationController.NavigationBarHidden = false;
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			NavigationController.NavigationBarHidden = false;
			NavigationController.ToolbarHidden = false;

		}

		void LoadThumbNails()
		{
			foreach (var img in item.images) {

				DownloadImageAsync (img).ContinueWith ((task) => InvokeOnMainThread (() => {
					//DetailTextLabel.Text = list.description;
					if (!task.IsFaulted) {

						ivcThumbnail2 = new UIImageViewClickable ();
						ivcThumbnail2.Image = task.Result;
						ivcThumbnail2.ContentMode = UIViewContentMode.ScaleAspectFit;
						ivcThumbnail2.AutoresizingMask = UIViewAutoresizing.All;
						ivcThumbnail2.OnClick += () => {
							if (vMain == null) {
								vMain = new UIViewFullscreen ();
							}
							vMain.SetImage (ivcThumbnail2.Image);
							vMain.Show ();
						};

						ivcThumbnail2.Frame = new RectangleF (indentX, lSmallImage.Frame.Bottom + indentY, 
							View.Frame.Width - indentX * 2, imageHeight);
						View.AddSubview (ivcThumbnail2);


					}
				}));

			}
		}
			
}
}
