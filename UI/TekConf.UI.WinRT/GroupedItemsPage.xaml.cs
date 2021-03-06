﻿using Microsoft.WindowsAzure.MobileServices;
using TekConf.RemoteData.Dtos.v1;
using TekConf.UI.WinRT.Data;

using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Grouped Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234231

namespace TekConf.UI.WinRT
{
	public class SomeShit
	{
		public string GroupMonthName { get; set; }
		public List<FullConferenceDto> Conferences { get; set; }
	}

	/// <summary>
	/// A page that displays a grouped collection of items.
	/// </summary>
	public sealed partial class GroupedItemsPage : TekConf.UI.WinRT.Common.LayoutAwarePage
	{
		public GroupedItemsPage()
		{
			this.InitializeComponent();
		}

		//protected override async void OnNavigatedTo(NavigationEventArgs e)
		//{
		//    await Authenticate();
		//}

		private MobileServiceUser user;
		private async void Authenticate()
		{
			if (!App.IsAuthenticated)
			{
				while (user == null)
				{
					string message;
					try
					{
						user = await App.MobileService
														.LoginAsync(MobileServiceAuthenticationProvider.Twitter);
						App.IsAuthenticated = true;
						//message =
						//string.Format("You are now logged in - {0}", user.UserId);
					}
					catch (InvalidOperationException)
					{
						//message = "You must log in. Login Required";
						//var dialog = new MessageDialog(message);
						//dialog.Commands.Add(new UICommand("OK"));
						//await dialog.ShowAsync();
					}
				}
			}
		}

		/// <summary>
		/// Populates the page with content passed during navigation.  Any saved state is also
		/// provided when recreating a page from a prior session.
		/// </summary>
		/// <param name="navigationParameter">The parameter value passed to
		/// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
		/// </param>
		/// <param name="pageState">A dictionary of state preserved by this page during an earlier
		/// session.  This will be null the first time a page is visited.</param>
		protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
		{
			Authenticate();

			// TODO: Create an appropriate data model for your problem domain to replace the sample data
			var sampleDataGroups = SampleDataSource.GetGroups((String)navigationParameter);
			//this.DefaultViewModel["Groups"] = sampleDataGroups;
			var groups = App.ViewModel.GroupedItems;

			this.DefaultViewModel["Groups"] = groups;

		}

		/// <summary>
		/// Invoked when a group header is clicked.
		/// </summary>
		/// <param name="sender">The Button used as a group header for the selected group.</param>
		/// <param name="e">Event data that describes how the click was initiated.</param>
		void Header_Click(object sender, RoutedEventArgs e)
		{
			// Determine what group the Button instance represents
			var group = (sender as FrameworkElement).DataContext;

			// Navigate to the appropriate destination page, configuring the new page
			// by passing required information as a navigation parameter
			//this.Frame.Navigate(typeof(GroupDetailPage), ((SampleDataGroup)group).UniqueId);
			this.Frame.Navigate(typeof(GroupDetailPage), ((FullConferenceDto)group).slug);

		}

		/// <summary>
		/// Invoked when an item within a group is clicked.
		/// </summary>
		/// <param name="sender">The GridView (or ListView when the application is snapped)
		/// displaying the item clicked.</param>
		/// <param name="e">Event data that describes the item clicked.</param>
		void ItemView_ItemClick(object sender, ItemClickEventArgs e)
		{
			// Navigate to the appropriate destination page, configuring the new page
			// by passing required information as a navigation parameter
			var itemId = ((FullConferenceDto)e.ClickedItem).slug;
			//this.Frame.Navigate(typeof(ItemDetailPage), itemId);
			this.Frame.Navigate(typeof(GroupDetailPage), itemId);

		}
	}
}
