﻿using Funq;
using NUnit.Framework;
using ServiceStack.CacheAccess.Providers;
using ServiceStack.ServiceInterface.Testing;
using Should;
using TekConf.RemoteData.Dtos.v1;
using TekConf.UI.Api.Services.Requests.v1;
using TekConf.UI.Api.v1;
using Ploeh.AutoFixture;

namespace TekConf.UI.Api.Tests.Int
{
	[TestFixture]
	public class ScheduleTests
	{
		private IFixture _fixture;
		private string _authenticationMethod = "Facebook";
		//private string _authenticationToken = "BAAF8EAYsaGwBAGmf61QFCcnaohTAUd2dDHlDUdyGKXaNEVPzl3837MvfN9HAZCP8uogLEejZAnPpeSuhUiZCZCCjRCDpFjZCAFEas1T1ZCowZBXw83FZC64XV9sJEdblnsNZBm1cpFLDHhU968WgHoWZBIzWIcZCmDp9nabZCkkDzECdUNKP0bxcZBaPlSpsWDy9RjWFdBg2uadlE0GxFweYdZCZA85tYrHJn7kTcnKPfCCeDrWaQZDZD";
		private string _authenticationToken = "robgibbens@gmail.com";
		private string _conferenceSlug = "codemash-2013";
		private string _sessionSlug = "building-real-time-web-applications";

		[TestFixtureSetUp]
		public void Setup()
		{
			_fixture = new Fixture();
		}

		[Test]
		public void should_create_new_schedule()
		{
			var service = new ScheduleService();

			var request = new AddSessionToSchedule()
																				 {
																					 authenticationMethod = _authenticationMethod,
																					 authenticationToken = _authenticationToken,
																					 conferenceSlug = _conferenceSlug,
																					 sessionSlug = _sessionSlug
																				 };
			var bootstrapper = new Bootstrapper();
			Container container = new Container();
			bootstrapper.BootstrapAutomapper(container);

			var response = service.Post(request);
		}

		[Test]
		public void should_retrieve_existing_schedule()
		{
			var service = new ScheduleService();
			var bootstrapper = new Bootstrapper();
			Container container = new Container();
			bootstrapper.BootstrapAutomapper(container);

			var request = new Schedule()
														 {
															 authenticationMethod = _authenticationMethod,
															 authenticationToken = _authenticationToken,
															 conferenceSlug = _conferenceSlug,
														 };

			var response = service.Get(request);

			response.ShouldNotBeNull();
			response.GetType().ShouldEqual(typeof(ScheduleDto));
			((ScheduleDto)response).sessions.ShouldNotBeEmpty();
		}
	}
}
