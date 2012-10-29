using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using TekConf.RemoteData.v1;
using System.Collections.Generic;
using TekConf.RemoteData.Dtos.v1;

namespace TekConf.UI.iPhone
{

	public partial class ConferencesListViewController : BaseUIViewController
	{
		public ConferencesListViewController () : base ("ConferencesListViewController", null)
		{
			Title = "Conferences";
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

			var loading = new UIAlertView (" Downloading Conferences", "Please wait...", null, null, null);

			loading.Show ();
			
			var indicator = new UIActivityIndicatorView (UIActivityIndicatorViewStyle.WhiteLarge); 
			indicator.Center = new System.Drawing.PointF (loading.Bounds.Width / 2, loading.Bounds.Size.Height - 40); 
			indicator.StartAnimating (); 
			loading.AddSubview (indicator);
			
			Repository.GetConferences (sortBy: "", showPastConferences: false, search: "", callback:conferences => 
			{ 
				InvokeOnMainThread (() => 
			    { 
					conferencesTableView.Source = new ConferencesTableViewSource (this, conferences); 
					conferencesTableView.ReloadData (); 
					loading.DismissWithClickedButtonIndex (0, true); 
				});
			});
			
			if (!UserInterfaceIdiomIsPhone)
			{
				this.conferencesTableView.SelectRow (
					NSIndexPath.FromRowSection (0, 0),
					false,
					UITableViewScrollPosition.Middle
					);
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
	
	
		private class ConferencesTableViewSource : UITableViewSource
		{ 
			private readonly IList<FullConferenceDto> _conferences;
			private const string ConferenceCell = "ConferenceCell";
			private ConferencesListViewController _rootViewController;
			
			public ConferencesTableViewSource (ConferencesListViewController controller, IList<FullConferenceDto> conferences)
			{ 
				_rootViewController = controller;
				_conferences = conferences; 
			}
			
			public override int RowsInSection (UITableView tableView, int section)
			{ 
				return _conferences.Count; 
			}
			
			public override float GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
			{ 
				return 60; 
			}
			
			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{ 
				var cell = tableView.DequeueReusableCell (ConferenceCell) ?? new UITableViewCell (UITableViewCellStyle.Subtitle, ConferenceCell); 
				var conference = _conferences [indexPath.Row]; 

				var font = UIFont.FromName("OpenSans", 12f);
				cell.TextLabel.Font = font;
				cell.DetailTextLabel.Font = font;

				cell.TextLabel.Text = conference.name; 
				cell.DetailTextLabel.Text = conference.start.ToLocalTime ().ToString (); 
				return cell; 
			}
			
			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{ 
				FullConferenceDto selectedConference = _conferences [indexPath.Row]; 
				//new UIAlertView ("View Conference", selectedConference.name, null, "Ok", null).Show (); 
				
				if (UserInterfaceIdiomIsPhone) {
					//_conferenceDetailViewController = new ConferenceDetailViewController (selectedConference.slug);
					//_rootViewController.SelectedConferenceSlug = selectedConference.slug;
					//_rootViewController.PerformSegue (MoveToMapSegueName, _rootViewController);
					// Pass the selected object to the new view controller.
					//_rootViewController.NavigationController.PushViewController (
					//						_conferenceDetailViewController,
					//						true
					//					);
				} else {
					// Navigation logic may go here -- for example, create and push another view controller.
				}
			} 
		}
		
		private class DataSource : UITableViewSource
		{
			//RootViewController controller;
			
			public DataSource ()
			{
				//this.controller = controller;
			}
			
			// Customize the number of sections in the table view.
			public override int NumberOfSections (UITableView tableView)
			{
				return 1;
			}
			
			public override int RowsInSection (UITableView tableview, int section)
			{
				return 1;
			}
			
			// Customize the appearance of table view cells.
			public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				string cellIdentifier = "Cell";
				var cell = tableView.DequeueReusableCell (cellIdentifier);
				if (cell == null) {
					cell = new UITableViewCell (UITableViewCellStyle.Default, cellIdentifier);
					if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone) {
						cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
					}
				}
				var font = UIFont.FromName("OpenSans", 12f);
				cell.TextLabel.Font = font;
				cell.DetailTextLabel.Font = font;
				// Configure the cell.
				//cell.TextLabel.Text = NSBundle.MainBundle.LocalizedString (
				//	"Detail",
				//	"Detail"
				//);
				return cell;
			}
			
			/*
			// Override to support conditional editing of the table view.
			public override bool CanEditRow (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				// Return false if you do not want the specified item to be editable.
				return true;
			}
			*/
			
			/*
			// Override to support editing the table view.
			public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
			{
				if (editingStyle == UITableViewCellEditingStyle.Delete) {
					// Delete the row from the data source.
					controller.conferencesTableView.DeleteRows (new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
				} else if (editingStyle == UITableViewCellEditingStyle.Insert) {
					// Create a new instance of the appropriate class, insert it into the array, and add a new row to the table view.
				}
			}
			*/
			
			/*
			// Override to support rearranging the table view.
			public override void MoveRow (UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
			{
			}
			*/
			
			/*
			// Override to support conditional rearranging of the table view.
			public override bool CanMoveRow (UITableView tableView, NSIndexPath indexPath)
			{
				// Return false if you do not want the item to be re-orderable.
				return true;
			}
			*/
			
			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				if (UserInterfaceIdiomIsPhone) {
//					var DetailViewController = new ConferenceDetailViewController ();
//					// Pass the selected object to the new view controller.
//					controller.NavigationController.PushViewController (
//						DetailViewController,
//						true
//						);
				} else {
					// Navigation logic may go here -- for example, create and push another view controller.
				}
			}
		}

	
	}
}
