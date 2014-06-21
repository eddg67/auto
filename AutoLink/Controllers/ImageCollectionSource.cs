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
		public ImageCollectionSource(List<string> images)
		{
			Rows = images;
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

			//ImageCell row = Rows[indexPath.Row];
			//row.Tapped.Invoke();
		}

		public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = (ImageCell) collectionView.DequeueReusableCell(ImageCell.CellID, indexPath);

			//ImageCell row = Rows[indexPath.Row];

			cell.UpdateRow(Rows[indexPath.Row], FontSize, ImageViewSize);

			return cell;
		}
	}



}

