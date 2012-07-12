﻿using ConferencesIO.RemoteData.Dtos;
using AutoMapper;
using ConferencesIO.RemoteData.Dtos.v1;

namespace ConferencesIO.UI.Api
{
  public class Bootstrapper
  {
    public void BootstrapAutomapper()
    {
      Mapper.CreateMap<ConferenceEntity, ConferencesDto>()
          .ForMember(dest => dest.url, opt => opt.Ignore());

      Mapper.CreateMap<ConferenceEntity, ConferenceDto>()
          .ForMember(dest => dest.url, opt => opt.Ignore())
          .ForMember(dest => dest.sessionsUrl, opt => opt.Ignore())
          .ForMember(dest => dest.speakersUrl, opt => opt.Ignore());

      Mapper.CreateMap<SessionEntity, SessionsDto>()
          .ForMember(dest => dest.url, opt => opt.Ignore());

      Mapper.CreateMap<SessionEntity, SessionDto>()
          .ForMember(dest => dest.url, opt => opt.Ignore())
          .ForMember(dest => dest.speakersUrl, opt => opt.Ignore());

      Mapper.CreateMap<SpeakerEntity, SpeakersDto>()
          .ForMember(dest => dest.url, opt => opt.Ignore());

      Mapper.CreateMap<SpeakerEntity, SpeakerDto>();

      Mapper.CreateMap<ScheduleEntity, ScheduleDto>();
    }

  }
}