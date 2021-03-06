using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using AutoMapper;
using ServiceStack.CacheAccess;
using TekConf.RemoteData.Dtos.v1;
using TekConf.UI.Api.Services;
using TekConf.UI.Api.Services.Requests.v1;

namespace TekConf.UI.Api.v1
{
	using TekConf.Common.Entities;

	public class SchedulesService : MongoServiceBase
	{
		private readonly IRepository<ScheduleEntity> _scheduleRepository;
		private readonly IRepository<ConferenceEntity> _conferenceRepository;

		public ICacheClient CacheClient { get; set; }

		public SchedulesService(IRepository<ScheduleEntity> scheduleRepository, IRepository<ConferenceEntity> conferenceRepository)
		{
			_scheduleRepository = scheduleRepository;
			this._conferenceRepository = conferenceRepository;
		}

		public object Get(Schedules request)
		{
			List<ScheduleEntity> schedules = null;
			List<FullConferenceDto> conferences = new List<FullConferenceDto>();

			schedules = _scheduleRepository
																			.AsQueryable()
																			.Where(s => s.UserName.ToLower() == request.userName.ToLower())
																			.ToList();

			foreach (var schedule in schedules)
			{
				var conference = _conferenceRepository
																								 .AsQueryable()
																								 .FirstOrDefault(c => c.slug == schedule.ConferenceSlug);
				var conferenceDto = Mapper.Map<ConferenceEntity, FullConferenceDto>(conference);

				conferenceDto.sessions.RemoveAll(x => !schedule.SessionSlugs.Contains(x.slug));
				foreach (var session in conferenceDto.sessions)
				{
						session.isAddedToSchedule = true;
				}

				conferences.Add(conferenceDto);
			}

			conferences = conferences.OrderBy(c => c.start).ToList();
			return conferences;
		}
	}
}