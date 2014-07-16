using System;
using MonoTouch.UIKit;
using System.Collections.Generic;
using AutoLink.Models;
using System.Drawing;
using MonoTouch.Foundation;

namespace AutoLink
{
	public class ImageCollectionSource : UICollectionViewSource
	{
		Listing item;
		AppDelegate app = (AppDelegate)UIApplication.SharedApplication.Delegate;

		public ImageCollectionSource(Listing list)
		{
			item = list;
			Rows = item.images;
		}

		public List<string> Rows { get; set; }

		public Single FontSize { get; set; }

		public SizeF ImageViewSize { get; set; }

		public override Int32 NumberOfSections(UICollectionView collectionView)
		{
			return 1;
		}

		public override Int32 GetItemsCount(UICollectionView collectionView, Int32 section)
		{
			return Rows.Count;
		}

		public override Boolean ShouldHighlightItem(UICollectionView collectionView, NSIndexPath indexPath)
		{
			return true;
		}

		public override void ItemHighlighted(UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = (ImageCell) collectionView.CellForItem(indexPath);
			cell.ImageView.Alpha = 0.5f;
		}

		public override void ItemUnhighlighted(UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = (ImageCell) collectionView.CellForItem(indexPath);
			cell.ImageView.Alpha = 1;
		}
			
			

		public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			//var cell = (ImageCell) collectionView.DequeueReusableCell(ImageCell.CellID, indexPath);
			//var headerView = (Header)collectionView.DequeueReusableSupplementaryView (new NSString ("UICollectionElementKindSectionHeader"), new NSString ("Header"), indexPath);
			//headerView.image = cell.ImageView.Image;

			app.ShowLargeImageView (item,indexPath.Row);

		}

		/*	public override UICollectionReusableView GetViewForSupplementaryElement (UICollectionView collectionView, NSString elementKind, NSIndexPath indexPath)
		{
			var headerView = (Header)collectionView.DequeueReusableSupplementaryView (elementKind, new NSString ("Header"), indexPath);
			var cell = (ImageCell) collectionView.DequeueReusableCell(ImageCell.CellID, indexPath);
			headerView.image = cell.ImageView.Image;

			return headerView;
		}
*/

		public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = (ImageCell) collectionView.DequeueReusableCell(ImageCell.CellID, indexPath);
			//ImageCell row = Rows[indexPath.Row];
			cell.UpdateRow(Rows[indexPath.Row], FontSize, ImageViewSize);

			return cell;
		}

		public override bool ShouldShowMenu (UICollectionView collectionView, NSIndexPath indexPath)
		{
			return true;
		}

		public override bool CanPerformAction (UICollectionView collectionView, MonoTouch.ObjCRuntime.Selector action, NSIndexPath indexPath, NSObject sender)
		{
			return true;
		}

		public override void PerformAction (UICollectionView collectionView, MonoTouch.ObjCRuntime.Selector action, NSIndexPath indexPath, NSObject sender)
		{
			var cell = GetCell (collectionView, indexPath) as ImageCell;
			var url = Rows [indexPath.Row];
			if(action.Name == "save"){
				if (cell.ImageView.Image != null) {
					cell.ImageView.Image.SaveToPhotosAlbum ((image, error) => {
						//var o = image as UIImage;
						Console.WriteLine ("error:" + error);
					});
				}
			}
		}
	}



}

