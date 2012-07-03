﻿using ArtekSoftware.Conference.UI.Web.Services.Requests;
using KellermanSoftware.CompareNetObjects;
using NUnit.Framework;

namespace ArtekSoftware.Conference.UI.Web.Tests.Int
{
  [TestFixture]
  public partial class ScheduleTests : RestTestBase
  {
    [Test(Description = "http://localhost:6327/api/conferences/CodeMash-2012/schedule/rob-gibbens")]
    public void given_a_POST_request_to_a_schedule_it_returns_the_schedule_with_sessions()
    {
      //GetConferences().FirstOrDefault().IsTheSameAs(codemashs).ShouldBeTrue();
      var schedule = GetSchedule(new ScheduleRequest() { conferenceSlug = "CodeMash-2012", userSlug = "rob-gibbens" });
      var compareObjects = new CompareObjects();
      var areSame = compareObjects.Compare(schedule, robsSchedule);
      if (!areSame)
      {
        Assert.Fail(compareObjects.DifferencesString);
      }
    }

    [Test(Description = "http://localhost:6327/api/conferences/CodeMash-2012/schedule/rob-gibbens")]
    public void given_a_POST_request_to_a_schedule_it_returns_response_code_CREATED()
    {
      Assert.Fail("Incomplete");
    }

    [Test(Description = "http://localhost:6327/api/conferences/CodeMash-2012/schedule/rob-gibbens")]
    public void given_a_POST_request_to_an_existing_schedule_it_returns_an_error()
    {
      Assert.Fail("Incomplete");
    }
  }
}
