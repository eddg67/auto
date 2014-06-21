using System;
using Alliance.Carousel;
using MonoTouch.UIKit;
using AutoLink.Models;
using System.Drawing;

namespace AutoLink.Models
{
	public class LinearDataSource : CarouselViewDataSource
	{
		Listing item;
		const float indentX = 50;
		const float indentY = 10;
		const float imageHeight = 100;

		UIViewFullscreen vMain;

		UILabel lBigImage;
		UIImageViewClickable ivcThumbnail1;

		UILabel lSmallImage;
		UIImageViewClickable ivcThumbnail2;

		public LinearDataSource(Listing list)
		{
			this.item = list;
		}

		public override uint NumberOfItems(CarouselView carousel)
		{
			return (uint)item.images.Count;
		}

		public override UIView ViewForItem(CarouselView carousel, uint index, UIView reusingView)
		{

				UILabel label;

				if (reusingView == null)
				{
					// don't do anything specific to the index within
					// this `if (view == null) {...}` statement because the view will be
					// recycled and used with other index values later
					var imgView = new UIImageViewClickable()
					{
						Frame = new RectangleF(0, 0, 200, 200),
						ContentMode = UIViewContentMode.Center
					};

				imgView.DownloadImageAsync (item.images[Convert.ToInt32(index)]).ContinueWith ((task) => InvokeOnMainThread (() => {
					//DetailTextLabel.Text = list.description;
					if (!task.IsFaulted) {
						imgView.Image = task.Result;

						imgView.ContentMode = UIViewContentMode.ScaleAspectFit;
						imgView.AutoresizingMask = UIViewAutoresizing.All;
						imgView.OnClick += () => {

						};

						label = new UILabel(imgView.Bounds)
						{
							BackgroundColor = UIColor.Clear,
							TextAlignment = UITextAlignment.Center,
							Tag = 1
						};
						label.Font = label.Font.WithSize(50);
						imgView.AddSubview(label);
						reusingView = imgView;

					}

				}));


				}
				else
				{
					label = (UILabel)reusingView.ViewWithTag(1);
				}

			return reusingView;
		}
	}
		

}

