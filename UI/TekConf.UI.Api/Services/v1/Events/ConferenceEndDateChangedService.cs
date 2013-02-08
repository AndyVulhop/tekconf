﻿using System;
using System.Linq;
using ServiceStack.CacheAccess;
using ServiceStack.ServiceHost;
using TekConf.RemoteData.Shared.v1.Requests;

namespace TekConf.UI.Api.Services.v1
{
	public class ConferenceEndDateChangedService : MongoServiceBase
	{
		private readonly IRepository<ConferenceEndDateChangedMessage> _repository;
		private readonly IConfiguration _configuration;
		public ICacheClient CacheClient { get; set; }

		public ConferenceEndDateChangedService(IRepository<ConferenceEndDateChangedMessage> repository, IConfiguration configuration)
		{
			_repository = repository;
			_configuration = configuration;
		}

		public object Get(ConferenceEndDateChanged request)
		{
			var cacheKey = "GetConferenceEndDateChanged";
			var expireInTimespan = new TimeSpan(0, 0, _configuration.cacheTimeout);

			return base.RequestContext.ToOptimizedResultUsingCache(this.CacheClient, cacheKey, expireInTimespan, () =>
				{
					var events = _repository
						.AsQueryable()
						.ToList();

					return events;
				});
		}
	}
}