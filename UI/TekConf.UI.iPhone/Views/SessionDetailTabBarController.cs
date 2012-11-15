using System;
using MonoTouch.UIKit;
using TekConf.RemoteData.Dtos.v1;

namespace TekConf.UI.iPhone
{
	public class SessionDetailTabBarController : BaseUITabBarController {
		
		UIViewController aboutTab;

		public SessionDetailTabBarController (string slug, string title)
		{
			this.Title = title;

			aboutTab = new SessionDetailAboutViewController(slug);
			aboutTab.Title = "About";

			var tabs = new UIViewController[] {
				aboutTab
			};
			
			ViewControllers = tabs;
			
			SelectedViewController = aboutTab;
		}

	}
}