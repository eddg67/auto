using System;
using System.Linq;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using AutoLink.Models;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;
using MonoTouch.ObjCRuntime;

namespace AutoLink
{
	public partial class ImageViewerViewController : UICollectionViewController
	{

			private ImageCollectionSource userSource;
			List<string> images { get; set; }

		public ImageViewerViewController(UICollectionViewLayout layout,List<string>  list) : base (layout)
			{
			images = list;
			}
		public ImageViewerViewController ()
			
		{


		}

		public override void ViewDidLoad()
		{
				base.ViewDidLoad();

				Title = "Collection";

			userSource = new ImageCollectionSource(images);
				userSource.FontSize = 11f;
			userSource.ImageViewSize = new SizeF(100, 125);

			CollectionView.RegisterClassForCell (typeof(ImageCell), ImageCell.CellID);
			CollectionView.Source = userSource;
			//CollectionView.RegisterClassForSupplementaryView(typeof(Header), UICollectionElementKindSection.Header,Header. );

				// add a custom menu item
				UIMenuController.SharedMenuController.MenuItems = new UIMenuItem[] { 
					new UIMenuItem ("Custom", new Selector ("custom:")) 
				};

			NavigationController.NavigationItem.LeftBarButtonItem = new UIBarButtonItem (UIBarButtonSystemItem.Done, delegate {
				NavigationController.PopViewControllerAnimated(true);
			});
		
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


			DownloadImageAsync (images [0]).ContinueWith ((task) => InvokeOnMainThread (() => {
				//DetailTextLabel.Text = list.description;
				if (!task.IsFaulted) {


					//LoadThumbNails ();

				}


			}));

		}

		void LoadThumbNails()
		{
			foreach (var img in images) {

				DownloadImageAsync (img).ContinueWith ((task) => InvokeOnMainThread (() => {
					//DetailTextLabel.Text = list.description;
					if (!task.IsFaulted) {



					}
				}));

			}
		}



	}

	public class Header : UICollectionReusableView
	{
		UILabel label;

		public string Text {
			get {
				return label.Text;
			}
			set {
				label.Text = value;
				SetNeedsDisplay ();
			}
		}

		[Export ("initWithFrame:")]
		public Header (System.Drawing.RectangleF frame) : base (frame)
		{
			label = new UILabel (){Frame = new System.Drawing.RectangleF (0,0,300,50), BackgroundColor = UIColor.Yellow};
			AddSubview (label);
		}
	}

}
