﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using AutoMapper;
using TekConf.RemoteData.Dtos.v1;
using TekConf.UI.Api.Services.Requests.v1;
using TekConf.UI.Api.UrlResolvers.v1;
using FluentMongo.Linq;
using ServiceStack.CacheAccess;
using ServiceStack.Common.Web;
using ServiceStack.ServiceHost;

namespace TekConf.UI.Api.Services.v1
{
    public class SpeakersService : MongoServiceBase
    {
        public ICacheClient CacheClient { get; set; }
        static HttpError ConferenceNotFound = HttpError.NotFound("Conference not found") as HttpError;
        static HashSet<string> NonExistingConferences = new HashSet<string>();

        static HttpError SpeakerNotFound = HttpError.NotFound("Speaker not found") as HttpError;
        static HashSet<string> NonExistingSpeakers = new HashSet<string>();

        public object Get(Speakers request)
        {
            if (request.conferenceSlug == default(string))
            {
                throw new HttpError() { StatusCode = HttpStatusCode.BadRequest };
            }

            return GetAllSpeakers(request);
        }

        private object GetAllSpeakers(Speakers request)
        {
            lock (NonExistingConferences)
            {
                if (NonExistingConferences.Contains(request.conferenceSlug))
                {
                    throw ConferenceNotFound;
                }
            }

            var cacheKey = "GetAllSpeakers-" + request.conferenceSlug;
            var expireInTimespan = new TimeSpan(0, 0, 20);
            return base.RequestContext.ToOptimizedResultUsingCache(this.CacheClient, cacheKey, expireInTimespan, () =>
            {
                var conference = this.RemoteDatabase.GetCollection<ConferenceEntity>("conferences")
                .AsQueryable()
                .Where(c => c.isLive)
                .SingleOrDefault(c => c.slug == request.conferenceSlug);

                if (conference == null)
                {
                    lock (NonExistingConferences)
                    {
                        NonExistingConferences.Add(request.conferenceSlug);
                    }
                    throw ConferenceNotFound;
                }

                var speakersList = new List<SpeakerEntity>();

                //TODO : Linq this
                foreach (var session in conference.sessions)
                {
                    if (session.speakers != null)
                    {
                        foreach (var speakerEntity in session.speakers)
                        {
                            if (!speakersList.Any(s => s.slug == speakerEntity.slug))
                            {
                                speakersList.Add(speakerEntity);
                            }
                        }
                    }
                }
                var speakers = speakersList.ToList();

                List<SpeakersDto> speakersDtos = Mapper.Map<List<SpeakerEntity>, List<SpeakersDto>>(speakers);
                foreach (var speakersDto in speakersDtos)
                {
                    var resolver = new ConferencesSpeakersResolver(request.conferenceSlug, speakersDto.slug);
                    speakersDto.url = resolver.ResolveUrl();
                }
                return speakersDtos.ToList();
            });


        }

    }
}