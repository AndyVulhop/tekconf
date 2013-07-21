using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.MvvmCross.Plugins.Sqlite;
using Cirrious.MvvmCross.ViewModels;
using TekConf.Core.Interfaces;
using TekConf.Core.Messages;
using TekConf.Core.Models;
using TekConf.Core.Repositories;
using TekConf.Core.Services;
using TekConf.RemoteData.Dtos.v1;

namespace TekConf.Core.ViewModels
{
	using TekConf.Core.Entities;

	public class ConferenceSessionsViewModel : MvxViewModel
	{

		private readonly IRemoteDataService _remoteDataService;
		private readonly IAnalytics _analytics;
		private readonly IMvxMessenger _messenger;
		private readonly IAuthentication _authentication;
		private readonly ILocalConferencesRepository _localConferencesRepository;

		private readonly ISQLiteConnection _connection;
		private MvxSubscriptionToken _favoritesUpdatedMessageToken;

		public ConferenceSessionsViewModel(IRemoteDataService remoteDataService, IAnalytics analytics, IMvxMessenger messenger,
																			IAuthentication authentication, 
																			ILocalConferencesRepository localConferencesRepository,
																			ISQLiteConnection connection)
		{
			_remoteDataService = remoteDataService;
			_analytics = analytics;
			_messenger = messenger;
			_authentication = authentication;
			_localConferencesRepository = localConferencesRepository;
			_connection = connection;

			_favoritesUpdatedMessageToken = _messenger.Subscribe<FavoriteSessionAddedMessage>(OnFavoritesUpdatedMessage);
		}

		public void Init(string slug)
		{
			HasSessions = true;
			StartGetConference(slug);
			if (_authentication.IsAuthenticated)
			{
				var userName = _authentication.UserName;
				StartGetSchedule(userName, slug, false);
			}
		}

		private string _pageTitle;
		public string PageTitle
		{
			get
			{
				return _pageTitle;
			}
			set
			{
				_pageTitle = value.ToUpper();
				RaisePropertyChanged(() => PageTitle);
			}
		}

		private bool _isAuthenticated;
		public bool IsAuthenticated
		{
			get
			{
				return _isAuthenticated;
			}
			set
			{
				_isAuthenticated = value;
				RaisePropertyChanged(() => IsAuthenticated);
			}
		}

		public void Refresh(string slug)
		{
			StartGetConference(slug, true);
			if (_authentication.IsAuthenticated)
			{
				var userName = _authentication.UserName;
				StartGetSchedule(userName, slug, true);
			}
		}

		public bool ShouldAddFavorites
		{
			get
			{
				bool shouldAddFavorites = false;
				if (_authentication.IsAuthenticated)
				{
					shouldAddFavorites = HasSessions && (Schedule == null || Schedule.sessions == null || !Schedule.sessions.Any());
				}

				return shouldAddFavorites;
			}
		}

		private void OnFavoritesUpdatedMessage(FavoriteSessionAddedMessage message)
		{
			this.DisplayFavoriteSessions(message.Schedule);
		}

		private void DisplayFavoriteSessions(ScheduleDto schedule)
		{
			Schedule = schedule;
		}

		private void StartGetConference(string slug, bool isRefreshing = false)
		{
			if (IsLoadingConference)
				return;

			IsLoadingConference = true;
			_analytics.SendView("ConferenceSessions-" + slug);
			
			if (!isRefreshing)
			{
				var conference = _localConferencesRepository.Get(slug);
				if (conference != null)
				{
					IEnumerable<SessionEntity> sessions = null;
					sessions = conference.Sessions(_connection);
					if (sessions != null)
					{
						var conferenceSessionsListViewDto = new ConferenceSessionsListViewDto(sessions)
						{
							name = conference.Name,
							slug = conference.Slug
						};
						GetConferenceSuccess(conferenceSessionsListViewDto);
					}
					else
					{
						_remoteDataService.GetConferenceSessionsList(slug, isRefreshing, GetConferenceSuccess, GetConferenceError);						
					}
				}
				else
				{
					_remoteDataService.GetConferenceSessionsList(slug, isRefreshing, GetConferenceSuccess, GetConferenceError);
				}
			}
			else
			{
				_remoteDataService.GetConferenceSessionsList(slug, isRefreshing, GetConferenceSuccess, GetConferenceError);				
			}
		}

		private void GetConferenceError(Exception exception)
		{
			// for now we just hide the error...
			_messenger.Publish(new ConferenceSessionsExceptionMessage(this, exception));
			IsLoadingConference = false;
		}

		private void GetConferenceSuccess(ConferenceSessionsListViewDto conference)
		{
			InvokeOnMainThread(() => DisplayConference(conference));
		}

		private void DisplayConference(ConferenceSessionsListViewDto conference)
		{
			IsLoadingConference = false;
			Conference = conference;
		}

		private bool _isLoadingConference;
		public bool IsLoadingConference
		{
			get { return _isLoadingConference; }
			set
			{
				_isLoadingConference = value;
				IsAuthenticated = _authentication.IsAuthenticated;
				RaisePropertyChanged(() => IsLoadingConference);
			}
		}

		private ConferenceSessionsListViewDto _conference;
		public ConferenceSessionsListViewDto Conference
		{
			get
			{
				return _conference;
			}
			set
			{
				_conference = value;
				PageTitle = _conference.name;
				RaisePropertyChanged(() => Conference);
				RaisePropertyChanged(() => HasSessions);

				IsLoadingConference = false;

			}
		}

		private async void StartGetSchedule(string userName, string conferenceSlug, bool isRefreshing)
		{
			if (IsLoadingSchedule)
				return;

			IsLoadingSchedule = true;
			if (!isRefreshing)
			{
				var favorites = await _localConferencesRepository.ListFavoriteSessionsAsync(conferenceSlug);
				if (favorites != null && favorites.Any())
				{
					var dtos = favorites.Select(s => new FullSessionDto(s)).ToList();
					var schedule = new ScheduleDto() { conferenceSlug = conferenceSlug, sessions = dtos, url = "", userSlug = _authentication.UserName };

					GetScheduleSuccess(schedule);
				}
				else
				{
					var schedule = await _remoteDataService.GetScheduleAsync(userName, conferenceSlug, isRefreshing: false, connection: _connection);
					GetScheduleSuccess(schedule);
				}
			}
			else
			{
				var schedule = await _remoteDataService.GetScheduleAsync(userName, conferenceSlug, isRefreshing: true, connection: _connection);
				GetScheduleSuccess(schedule);
			}
		}

		private void GetScheduleError(Exception exception)
		{
			// for now we just hide the error...
			_messenger.Publish(new ConferenceSessionsExceptionMessage(this, exception));

			IsLoadingSchedule = false;
		}

		private void GetScheduleSuccess(ScheduleDto conference)
		{
			InvokeOnMainThread(() => DisplaySchedule(conference));
		}

		private void DisplaySchedule(ScheduleDto conference)
		{
			IsLoadingSchedule = false;
			Schedule = conference;
		}

		private bool _isLoadingSchedule;
		public bool IsLoadingSchedule
		{
			get { return _isLoadingSchedule; }
			set { _isLoadingSchedule = value; RaisePropertyChanged(() => IsLoadingSchedule); }
		}

		public bool HasSessions
		{
			get
			{
				if (Conference.IsNull())
					return false;

				return Conference.Sessions.Any();
			}
			set
			{
				RaisePropertyChanged(() => HasSessions);
			}
		}

		public List<FullSessionDto> Sessions
		{
			get
			{
				if (Schedule != null && Schedule.sessions != null)
					return Schedule.sessions;
				else
					return new List<FullSessionDto>();
			}
		}

		private ScheduleDto _schedule;
		public ScheduleDto Schedule
		{
			get
			{
				return _schedule;
			}
			set
			{
				_schedule = value;
				RaisePropertyChanged(() => Schedule);
				RaisePropertyChanged(() => Sessions);
				RaisePropertyChanged(() => ShouldAddFavorites);
				IsLoadingSchedule = false;
			}
		}

		public ICommand ShowSettingsCommand
		{
			get
			{
				return new MvxCommand(() => ShowViewModel<SettingsViewModel>());
			}
		}

		public ICommand ShowSearchCommand
		{
			get
			{
				return new MvxCommand(() => ShowViewModel<ConferenceSearchViewModel>());
			}
		}

		public ICommand ShowSessionDetailCommand
		{
			get
			{
				return new MvxCommand<SessionDetailViewModel.Navigation>(navigation =>
					ShowViewModel<SessionDetailViewModel>(navigation)
					);
			}
		}

	}
}