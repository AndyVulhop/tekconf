﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Cirrious.CrossCore.Core;
using Cirrious.MvvmCross.Plugins.File;
using Cirrious.MvvmCross.Plugins.Network.Reachability;
using TekConf.Core.Models;
using TekConf.RemoteData.Dtos.v1;

namespace TekConf.Core.Services
{

	public class RemoteDataService : IRemoteDataService
	{
		private readonly IMvxFileStore _fileStore;
		private readonly ICacheService _cache;
		private readonly IMvxReachability _reachability;

		public RemoteDataService(IMvxFileStore fileStore,ICacheService cache)
		{
			_fileStore = fileStore;
			_cache = cache;
			//_reachability = reachability;
			_reachability = null;
		}

		public void GetConferences(
			bool isRefreshing = false,
			string userName = null,
			string sortBy = "end",
			bool? showPastConferences = null,
			bool? showOnlyOpenCalls = null,
			bool? showOnlyOnSale = null,
			string search = null,
			string city = null,
			string state = null,
			string country = null,
			double? latitude = null,
			double? longitude = null,
			double? distance = null,
			Action<IEnumerable<FullConferenceDto>> success = null,
			Action<Exception> error = null)
		{
			ConferencesService.GetConferencesAsync(search, _fileStore, isRefreshing, _cache, success, error);
		}

		public void GetConference(string slug, bool isRefreshing, Action<FullConferenceDto> success = null, Action<Exception> error = null)
		{
			ConferenceService.GetConferenceAsync(_fileStore, _reachability, slug, isRefreshing, _cache, success, error);
		}

		public void GetSchedule(string userName, string conferenceSlug, bool isRefreshing, Action<ScheduleDto> success = null, Action<Exception> error = null)
		{
			ScheduleService.GetScheduleAsync(_fileStore, userName, conferenceSlug, isRefreshing, _cache, success, error);
		}

		public void GetSchedules(string userName, bool isRefreshing, Action<IEnumerable<FullConferenceDto>> success = null, Action<Exception> error = null)
		{
			ScheduleService.GetSchedulesAsync(_fileStore, userName, isRefreshing, _cache, success, error);
		}

		public void AddToSchedule(string userName, string conferenceSlug, Action<ScheduleDto> success = null, Action<Exception> error = null)
		{
			ScheduleService.AddToScheduleAsync(_fileStore, userName, conferenceSlug, false, _cache, success, error);
		}

		public void GetSession(string conferenceSlug, string sessionSlug, bool isRefreshing, Action<FullSessionDto> success, Action<Exception> error)
		{
			SessionService.GetSessionAsync(_fileStore, conferenceSlug, sessionSlug, isRefreshing, _cache, success, error);
		}

		public void GetIsOauthUserRegistered(string userId, Action<string> success, Action<Exception> error)
		{
			UserService.GetIsOauthUserRegisteredAsync(userId, success, error);
		}
	}
}