﻿using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TekConf.RemoteData.Dtos.v1;
using TekConf.UI.Web.App_Start;

namespace TekConf.UI.Web.Controllers
{
    public class SpeakerController : Controller
    {
        private RemoteDataRepositoryAsync _repository;

        public SpeakerController()
        {
            _repository = new RemoteDataRepositoryAsync();
        }

        [CompressFilter]
        public async Task<ActionResult> Index(string conferenceSlug, string sessionSlug)
        {
            var sessionSpeakersTask = _repository.GetSessionSpeakers(conferenceSlug, sessionSlug);

            await sessionSpeakersTask;

            return View(sessionSpeakersTask.Result);
        }



        [CompressFilter]
        public async Task<ActionResult> Detail(string conferenceSlug, string sessionSlug, string speakerSlug)
        {
            var speakerTask = _repository.GetSpeaker(conferenceSlug, speakerSlug);

            var conferenceTask = _repository.GetFullConference(conferenceSlug);

            await Task.WhenAll(speakerTask, conferenceTask);

            if (speakerTask.Result == null || conferenceTask.Result == null)
            {
                return RedirectToAction("NotFound", "Error");
            }

            var conference = conferenceTask.Result;
            var speaker = speakerTask.Result;

            var conferenceDto = new ConferencesDto()
            {
                description = conference.description,
                end = conference.end,
                imageUrl = conference.imageUrl,
                location = conference.location,
                name = conference.name,
                slug = conference.slug,
                start = conference.start
            };

            var sessions = from s in conference.sessions
                           from sp in s.speakers
                           where sp.slug == speaker.slug
                           select new SessionsDto()
                           {
                               conferenceSlug = conference.slug,
                               description = s.description,
                               difficulty = s.difficulty,
                               end = s.end,
                               links = s.links,
                               prerequisites = s.prerequisites,
                               room = s.room,
                               sessionType = s.sessionType,
                               slug = s.slug,
                               start = s.start,
                               subjects = s.subjects,
                               tags = s.tags,
                               title = s.title,
                               twitterHashTag = s.twitterHashTag,
                           };

            var viewModel = new SpeakerDetailViewModel()
            {
                Conference = conferenceDto,
                Speaker = speaker,
                Sessions = sessions.ToList(),
            };

            return View(viewModel);
        }



    }
}
