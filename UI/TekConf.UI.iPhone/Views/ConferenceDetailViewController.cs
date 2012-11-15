using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;
using TekConf.RemoteData.Dtos.v1;
using System.Linq;

namespace TekConf.UI.iPhone
{
	public partial class ConferenceDetailViewController : BaseUIViewController
	{
		public ConferenceDetailViewController () : base ("ConferenceDetailViewController", null)
		{
		}
		
		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
		}

		private void LoadConference()
		{
		
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			LoadConference();
			var loading = new UIAlertView (" Downloading Sessions", "Please wait...", null, null, null);
			
			loading.Show ();
			
			var indicator = new UIActivityIndicatorView (UIActivityIndicatorViewStyle.WhiteLarge); 
			indicator.Center = new System.Drawing.PointF (loading.Bounds.Width / 2, loading.Bounds.Size.Height - 40); 
			indicator.StartAnimating (); 
			loading.AddSubview (indicator);

			sessionsTableView.Source = new SessionsTableViewSource (this, NavigationItems.Conference.sessions); 
			sessionsTableView.ReloadData (); 
			loading.DismissWithClickedButtonIndex (0, true); 
		

			if (!UserInterfaceIdiomIsPhone) {
				this.sessionsTableView.SelectRow (
					NSIndexPath.FromRowSection (0, 0),
					false,
					UITableViewScrollPosition.Middle
				);
			}

			if (NavigationItems.Conference != null)
			{
				TrackAnalyticsEvent("ConferenceDetailViewController-" + NavigationItems.Conference.slug);
			}
		}

		[Obsolete]
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			
			// Clear any references to subviews of the main view in order to
			// allow the Garbage Collector to collect them sooner.
			//
			// e.g. myOutlet.Dispose (); myOutlet = null;
			
			ReleaseDesignerOutlets ();
		}

		[Obsolete]
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
		}

		private class SessionsTableViewSource : UITableViewSource
		{ 
			private readonly IList<FullSessionDto> _sessions;
			private const string SessionCell = "SessionCell";
			private ConferenceDetailViewController _rootViewController;
			private SessionDetailTabBarController _sessionDetailTabBarViewController;
			private List<DateTime> _sessionStartTimes;


			public SessionsTableViewSource (ConferenceDetailViewController controller, IList<FullSessionDto> sessions)
			{ 
				_rootViewController = controller;
				_sessions = sessions; 
			
				_sessionStartTimes = _sessions.Select (x => x.start).Distinct ().ToList ();
			}


//			public override string[] SectionIndexTitles (UITableView tableView)
//			{
//				return _sessionStartTimes.Select (x => x.ToString ("dddd h:mm tt")).ToArray ();
//			}

//			public override UIView GetViewForHeader (UITableView tableView, int section)
//			{
//				var label = new UILabel();
//				label.Text = "Here";
//
//				return label;
//			}

			public override int NumberOfSections (UITableView tableView)
			{
				return _sessionStartTimes.Count ();
			}

			public override string TitleForHeader (UITableView tableView, int section)
			{
				return _sessionStartTimes [section].ToString ("dddd h:mm tt");
			}

			public override int SectionFor (UITableView tableView, string title, int atIndex)
			{
				return atIndex;
			}

			public override int RowsInSection (UITableView tableView, int section)
			{ 
				return _sessions.ToList ().FindAll (x => x.start == _sessionStartTimes [section]).Count;
			}
			
			public override float GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
			{ 
				var startTimeSection = _sessionStartTimes [indexPath.Section];
				var filteredSessions = _sessions.Where (s => s.start == startTimeSection).OrderBy (o => o.title).ToArray ();
				
				var session = filteredSessions [indexPath.Row]; 


				var titleSize = tableView.StringSize(session.title, BaseUIViewController.TitleFont, new SizeF(237.0f, 1000.0f), UILineBreakMode.WordWrap);
				SizeF descriptionSize = new SizeF(0,0);
				if (!string.IsNullOrWhiteSpace(session.room))
				{
					descriptionSize = tableView.StringSize(session.room, BaseUIViewController.DescriptionFont, new SizeF(237.0f, 1000.0f), UILineBreakMode.WordWrap);
				}
				var sizeTotal = new SizeF(237.0f, titleSize.Height + descriptionSize.Height + 20);
				var cellSize = sizeTotal;
				
				//if (cellSize.Height < 108)
				//	cellSize = new SizeF(cellSize.Width, 108);
			
				return cellSize.Height + 10; 
			}
			
			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{ 
				var cell = tableView.DequeueReusableCell (SessionCell) ?? new UITableViewCell (UITableViewCellStyle.Subtitle, SessionCell); 
				
				var startTimeSection = _sessionStartTimes [indexPath.Section];
				var filteredSessions = _sessions.Where (s => s.start == startTimeSection).OrderBy (o => o.title).ToArray ();

				var session = filteredSessions [indexPath.Row]; 


				cell.TextLabel.Font = BaseUIViewController.TitleFont;
				cell.TextLabel.Text = session.title;
				cell.TextLabel.LineBreakMode = UILineBreakMode.WordWrap;
				cell.TextLabel.Lines = 0;
				cell.TextLabel.SizeToFit();
				cell.SizeToFit();
				cell.DetailTextLabel.Font = BaseUIViewController.DescriptionFont;

				if (string.IsNullOrEmpty(session.room))
				{
					cell.DetailTextLabel.Text = "No room set";
				}
				else
				{
					cell.DetailTextLabel.Text = session.room;
				}

				return cell; 
			}


			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{ 
				var startTimeSection = _sessionStartTimes [indexPath.Section];
				var filteredSessions = _sessions.Where (s => s.start == startTimeSection).OrderBy (o => o.title).ToArray ();
				
				//var cell = tableView.DequeueReusableCell (SessionCell) ?? new UITableViewCell (UITableViewCellStyle.Subtitle, SessionCell); 
				var selectedSession = filteredSessions [indexPath.Row]; 

				//var selectedSession = _sessions [indexPath.Row]; 
				
				if (UserInterfaceIdiomIsPhone) {
					_sessionDetailTabBarViewController = new SessionDetailTabBarController (selectedSession.slug, selectedSession.title);
					
					_rootViewController.NavigationController.PushViewController (
						_sessionDetailTabBarViewController,
						true
					);
				} else {
					// Navigation logic may go here -- for example, create and push another view controller.
				}
			} 
		}

	}
}

