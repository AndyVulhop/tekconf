﻿using ArtekSoftware.Conference.UI.Web.Services.Requests;
using KellermanSoftware.CompareNetObjects;
using NUnit.Framework;

namespace ArtekSoftware.Conference.UI.Web.Tests.Int
{
  [TestFixture]
  public partial class ScheduleTests : RestTestBase
  {
    [Test(Description = "http://localhost:6327/api/conferences/CodeMash-2012/schedule/rob-gibbens")]
    public void given_a_POST_request_for_a_resource_that_already_exists_it_should_update_the_resource()
    {
      Assert.Fail("Not Implemented");
    }

    [Test(Description = "http://localhost:6327/api/conferences/CodeMash-2012/schedule/rob-gibbens")]
    public void given_a_POST_request_for_a_resource_that_does_not_already_exist_it_should_create_the_resource()
    {
      Assert.Fail("Not Implemented");
    }
  }
}