using ServiceStack.ServiceHost;
using TekConf.RemoteData.Dtos.v1;

namespace TekConf.UI.Api.Services.Requests.v1
{
	[Route("/v1/registrations/{userName}", "GET")]
	public class Registration : IReturn<string>
	{
		[ApiMember(Name = "userId", Description = "XXXX", ParameterType = "query", DataType = "string", IsRequired = true)]
		public string userId { get; set; }
	}
}