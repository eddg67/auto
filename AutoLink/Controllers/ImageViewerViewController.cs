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
	
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);

			NavigationController.NavigationBarHidden = false;
			NavigationController.ToolbarHidden = false;

		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			Title = "Gallery";

			userSource = new ImageCollectionSource(images);
			userSource.FontSize = 11f;
			userSource.ImageViewSize = new SizeF(100, 125);

			CollectionView.BackgroundColor = UIColor.White;
			CollectionView.RegisterClassForCell (typeof(ImageCell), ImageCell.CellID);
			CollectionView.Source = userSource;
			NSString head = new NSString ("Header");
			CollectionView.RegisterClassForSupplementaryView(typeof(Header), UICollectionElementKindSection.Header,head );

				// add a custom menu item
				UIMenuController.SharedMenuController.MenuItems = new UIMenuItem[] { 
					new UIMenuItem ("Custom", new Selector ("custom:")) 
				};
		}

		public override void LoadView ()
		{
			base.LoadView ();
		
		}



		public async Task<UIImage> DownloadImageAsync(string imageUrl)
		{
			var httpClient = new HttpClient();

			Task <Byte[]> contentsTask = httpClient.GetByteArrayAsync(imageUrl);

			var contents = await contentsTask;

			return UIImage.LoadFromData(NSData.FromArray(contents));
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
			label = new UILabel (){Frame = new System.Drawing.RectangleF (0,0,300,100), BackgroundColor = UIColor.Yellow};
			label.BackgroundColor = UIColor.Yellow;
			Text = "Test";
			AddSubview (label);
		}
	}

}
