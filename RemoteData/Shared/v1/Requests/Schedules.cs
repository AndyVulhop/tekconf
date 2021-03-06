﻿using System.Collections.Generic;
using ServiceStack.ServiceHost;
using TekConf.RemoteData.Dtos.v1;

namespace TekConf.UI.Api.Services.Requests.v1
{
	[Route("/v1/conferences/schedules", "GET")]
	public class Schedules : IReturn<List<FullConferenceDto>>
	{
		[ApiMember(Name = "userName", Description = "XXXX", ParameterType = "query", DataType = "string", IsRequired = false)]
		public string userName { get; set; }
	}
}
