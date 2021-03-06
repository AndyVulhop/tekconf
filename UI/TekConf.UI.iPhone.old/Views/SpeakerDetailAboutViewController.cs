using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using TekConf.RemoteData.Dtos.v1;
using MonoTouch.Dialog.Utilities;

namespace TekConf.UI.iPhone
{
	public partial class SpeakerDetailAboutViewController : BaseUIViewController, IImageUpdated
	{
		private FullSpeakerDto _speaker;

		public SpeakerDetailAboutViewController (FullSpeakerDto speaker) : this()
		{
			_speaker = speaker;
		}

		public SpeakerDetailAboutViewController () : base ("SpeakerDetailAboutViewController", null)
		{
		}
		
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			this.contentDetailsScrollView.ContentSize = new SizeF (width: this.View.Frame.Width, height: 600);
			this.contentDetailsScrollView.ScrollEnabled = true;
			this.contentDetailsScrollView.ClipsToBounds = true;
			this.contentDetailsScrollView.ContentInset = new UIEdgeInsets (top: 0, left: 0, bottom: 900, right: 0);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			SetProfileImage ();
			
			SetFullName ();
		
			SetSeparator ();

			SetDescription ();

			SetMoreInformation ();


			if (_speaker != null)
			{
				TrackAnalyticsEvent("SpeakerDetailAboutViewController-" + _speaker.slug);
			}
		}

		void SetProfileImage ()
		{
			if (!string.IsNullOrWhiteSpace (_speaker.profileImageUrl)) {
				var profileImage = ImageLoader.DefaultRequestImage (new Uri ("http://www.tekconf.com" + _speaker.profileImageUrl), this);
				if (profileImage == null) {
					//logoImage.Image = DefaultImage;
				} else {
					this.profileImageView.Image = profileImage;
				}
			}
		}

		void SetFullName ()
		{
			var font = UIFont.FromName ("OpenSans-Light", 22f);
			this.fullNameLabel.Font = font;
			this.fullNameLabel.Text = _speaker.fullName.Trim ();
			this.fullNameLabel.Lines = 0;
			this.fullNameLabel.LineBreakMode = UILineBreakMode.WordWrap;
			this.fullNameLabel.SizeToFit ();
		}

		void SetSeparator ()
		{
			var frame = this.separatorBelowName.Frame;
			frame.Y = this.fullNameLabel.Frame.Y + this.fullNameLabel.Frame.Height + 10;
			this.separatorBelowName.Frame = frame;
		}

		void SetDescription ()
		{
			this.descriptionLabel.Text = _speaker.description.Trim ();
			this.descriptionLabel.Lines = 0;
			this.descriptionLabel.SizeToFit ();

			var frame = this.descriptionLabel.Frame;
			frame.Y = this.fullNameLabel.Frame.Y + this.fullNameLabel.Frame.Height + 10;
			this.descriptionLabel.Frame = frame;
		}

		void SetMoreInformation ()
		{
			if (string.IsNullOrWhiteSpace (_speaker.emailAddress)
				&& string.IsNullOrWhiteSpace (_speaker.facebookUrl)
				&& string.IsNullOrWhiteSpace (_speaker.googlePlusUrl)
				&& string.IsNullOrWhiteSpace (_speaker.twitterName)) {

				this.moreInformationView.Hidden = true;
			} else {
				this.moreInformationView.Hidden = false;

				this.emailAddressLabel.Text = _speaker.emailAddress;
				this.facebookLabel.Text = _speaker.facebookUrl;
				this.googlePlusLabel.Text = _speaker.googlePlusUrl;
				this.twitterNameLabel.Text = _speaker.twitterName;
				
				this.moreInformationView.BackgroundColor = UIColor.FromRGBA (red: 0.933f, green: 0.933f, blue: 0.933f, alpha: 1f);
				
				var frame = this.moreInformationView.Frame;
				frame.Y = this.descriptionLabel.Frame.Y + this.descriptionLabel.Frame.Height + 10;
				this.moreInformationView.Frame = frame;		
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
	
		public void UpdatedImage (Uri uri)
		{
			this.profileImageView.Image = ImageLoader.DefaultRequestImage (uri, this);
		}	
	}
}

