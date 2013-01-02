using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using AutoMapper;
using ServiceStack.ServiceInterface;
using TekConf.RemoteData.Dtos.v1;
using TekConf.UI.Api.Services;
using TekConf.UI.Api.Services.Requests.v1;
using TekConf.UI.Api.UrlResolvers.v1;
using FluentMongo.Linq;
using ServiceStack.CacheAccess;
using ServiceStack.Common.Web;
using ServiceStack.ServiceHost;

namespace TekConf.UI.Api.v1
{
	public class ScheduleService : MongoServiceBase
	{
		public ICacheClient CacheClient { get; set; }

		public object Get(Schedule request)
		{
			var session = this.GetSession();
			var x = session.UserName;
			if (request.conferenceSlug == default(string))
			{
				return new HttpError() { StatusCode = HttpStatusCode.BadRequest };
			}

			return GetSchedule(request);
		}

		public object Post(AddSessionToSchedule request)
		{
			var session = this.GetSession();

			var scheduleCollection = this.RemoteDatabase.GetCollection<ScheduleEntity>("schedules");
			ScheduleEntity schedule = scheduleCollection.AsQueryable()
					.Where(s => s.ConferenceSlug.ToLower() == request.conferenceSlug.ToLower())
					.SingleOrDefault(s => s.UserName.ToLower() == session.UserName.ToLower());

			if (schedule == null)
			{
				schedule = new ScheduleEntity()
											 {
												 _id = Guid.NewGuid(),
												 ConferenceSlug = request.conferenceSlug,
												 UserName = session.UserName,
												 SessionSlugs = new List<string>(),
											 };
			}
			var conferenceCollection = this.RemoteDatabase.GetCollection<ConferenceEntity>("conferences");

			var conference =
					conferenceCollection.AsQueryable()
					.SingleOrDefault(c => c.slug == request.conferenceSlug);

			if (conference != null)
			{
				if (!string.IsNullOrWhiteSpace(request.sessionSlug) && !schedule.SessionSlugs.Any(s => s == request.sessionSlug))
				{
					schedule.SessionSlugs.Add(request.sessionSlug);
				}
			}

			scheduleCollection.Save(schedule);

			var scheduleDto = Mapper.Map<ScheduleDto>(schedule);

			return scheduleDto;
		}


		private ScheduleDto GetSchedule(Schedule request)
		{
			var session = this.GetSession();
			var x = session.UserName;
			var schedule = this.RemoteDatabase.GetCollection<ScheduleEntity>("schedules")
												 .AsQueryable()
												 .Where(s => s.ConferenceSlug.ToLower() == request.conferenceSlug.ToLower())
												 .SingleOrDefault(s => s.UserName.ToLower() == session.UserName.ToLower());

			var scheduleDto = Mapper.Map<ScheduleEntity, ScheduleDto>(schedule);

			return scheduleDto;
		}
	}
}