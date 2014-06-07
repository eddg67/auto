using System;
using MonoTouch.UIKit;
using MonoTouch.ObjCRuntime;
using MonoTouch.MediaPlayer;
using MonoTouch.Foundation;

namespace Autolink
{
	public class OpenMenuGestureRecognizer: UIPanGestureRecognizer
	{
		public OpenMenuGestureRecognizer (Action<UIPanGestureRecognizer> callback, Func<UIGestureRecognizer, UITouch,bool>  shouldReceiveTouch) : base (callback)
		{
			this.ShouldReceiveTouch += (sender,touch)=> {
				//Ugly hack to ignore touches that are on a cell that is moving...
				bool isMovingCell = touch.View.ToString().IndexOf("UITableViewCellReorderControl",StringComparison.InvariantCultureIgnoreCase) > -1;
				if(touch.View is UISlider || touch.View is MPVolumeView || isMovingCell)
					return false;
				return shouldReceiveTouch(sender,touch);
			};
		}
	}
}

