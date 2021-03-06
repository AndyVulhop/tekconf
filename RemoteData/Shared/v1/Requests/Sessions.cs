using System;
using System.Collections.Generic;
using ServiceStack.ServiceHost;
using TekConf.RemoteData.Dtos.v1;

namespace TekConf.UI.Api.Services.Requests.v1
{
	[Route("/v1/conferences/{conferenceSlug}/sessions", "GET")]
	public class Sessions : IReturn<List<SessionsDto>>
	{
		[ApiMember(Name = "conferenceSlug", Description = "XXXX", ParameterType = "query", DataType = "string", IsRequired = false)]
		public string conferenceSlug { get; set; }
	}

	[Route("/v1/conferences/{conferenceSlug}/sessions/{sessionSlug}", "GET")]
	public class Session : IReturn<SessionDto>
	{
		[ApiMember(Name = "conferenceSlug", Description = "XXXX", ParameterType = "query", DataType = "string", IsRequired = false)]
		public string conferenceSlug { get; set; }
		[ApiMember(Name = "sessionSlug", Description = "XXXX", ParameterType = "query", DataType = "string", IsRequired = false)]
		public string sessionSlug { get; set; }
	}

	[Route("/v1/conferences/{conferenceSlug}/sessions/{slug}", "POST")]
	[Route("/v1/conferences/{conferenceSlug}/sessions/{slug}", "PUT")]
	public class AddSession : IReturn<SessionDto>
	{
		[ApiMember(Name = "slug", Description = "XXXX", ParameterType = "query", DataType = "string", IsRequired = false)]
		public string slug { get; set; }
		
		[ApiMember(Name = "conferenceSlug", Description = "XXXX", ParameterType = "query", DataType = "string", IsRequired = false)]
		public string conferenceSlug { get; set; }
		
		[ApiMember(Name = "title", Description = "XXXX", ParameterType = "query", DataType = "string", IsRequired = false)]
		public string title { get; set; }
		
		[ApiMember(Name = "start", Description = "XXXX", ParameterType = "query", DataType = "DateTime", IsRequired = false)]
		public DateTime start { get; set; }
		
		[ApiMember(Name = "end", Description = "XXXX", ParameterType = "query", DataType = "DateTime", IsRequired = false)]
		public DateTime end { get; set; }

		[ApiMember(Name = "defaultTrackLength", Description = "XXXX", ParameterType = "query", DataType = "int", IsRequired = false)]
		public int defaultTalkLength { get; set; }

		[ApiMember(Name = "room", Description = "XXXX", ParameterType = "query", DataType = "string", IsRequired = false)]
		public string room { get; set; }
		
		[ApiMember(Name = "difficulty", Description = "XXXX", ParameterType = "query", DataType = "string", IsRequired = false)]
		public string difficulty { get; set; }
		
		[ApiMember(Name = "description", Description = "XXXX", ParameterType = "query", DataType = "string", IsRequired = false)]
		public string description { get; set; }
		
		[ApiMember(Name = "twitterHashTag", Description = "XXXX", ParameterType = "query", DataType = "string", IsRequired = false)]
		public string twitterHashTag { get; set; }
		
		[ApiMember(Name = "sessionType", Description = "XXXX", ParameterType = "query", DataType = "string", IsRequired = false)]
		public string sessionType { get; set; }
		
		[ApiMember(Name = "links", Description = "XXXX", ParameterType = "query", DataType = "string", IsRequired = false)]
		public List<string> links { get; set; }
		
		[ApiMember(Name = "tags", Description = "XXXX", ParameterType = "query", DataType = "string", IsRequired = false)]
		public List<string> tags { get; set; }
		
		[ApiMember(Name = "subjects", Description = "XXXX", ParameterType = "query", DataType = "string", IsRequired = false)]
		public List<string> subjects { get; set; }
		
		[ApiMember(Name = "resources", Description = "XXXX", ParameterType = "query", DataType = "string", IsRequired = false)]
		public List<string> resources { get; set; }
		
		[ApiMember(Name = "prerequisites", Description = "XXXX", ParameterType = "query", DataType = "string", IsRequired = false)]
		public List<string> prerequisites { get; set; }
	}
}
