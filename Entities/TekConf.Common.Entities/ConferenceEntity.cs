using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using TekConf.Common.Entities;
using TinyMessenger;

namespace TekConf.UI.Api
{
	public class ConferenceEntity : ISupportInitialize
	{
		private readonly ITinyMessengerHub _hub;
		private readonly IRepository<ConferenceEntity> _repository;
		private bool _isInitializingFromBson;
		private List<ConferenceCreatedMessage> _conferenceCreatedMessages;
		private List<ConferenceStartDateChangedMessage> _conferenceStartDateChangedMessages;
		private List<ConferenceEndDateChangedMessage> _conferenceEndDateChangedMessages;
		private List<ConferenceLocationChangedMessage> _conferenceLocationChangedMessages;
		private List<ConferencePublishedMessage> _conferencePublishedMessages;
		private List<ConferenceUpdatedMessage> _conferenceUpdatedMessages;
		private List<SessionAddedMessage> _sessionAddedMessages;
		private List<SessionRemovedMessage> _sessionRemovedMessages;
		private List<SpeakerAddedMessage> _speakerAddedMessages;
		private List<SpeakerRemovedMessage> _speakerRemovedMessages;
		private List<SessionRoomChangedMessage> _sessionRoomChangedMessages; 

		public ConferenceEntity(ITinyMessengerHub hub, IRepository<ConferenceEntity> repository)
		{
			_hub = hub;
			_repository = repository;

			_conferenceStartDateChangedMessages = new List<ConferenceStartDateChangedMessage>();
			_conferenceCreatedMessages = new List<ConferenceCreatedMessage>();
			_conferenceEndDateChangedMessages = new List<ConferenceEndDateChangedMessage>();
			_conferenceLocationChangedMessages = new List<ConferenceLocationChangedMessage>();
			_conferencePublishedMessages = new List<ConferencePublishedMessage>();
			_conferenceUpdatedMessages = new List<ConferenceUpdatedMessage>();
			_sessionAddedMessages = new List<SessionAddedMessage>();
			_sessionRemovedMessages = new List<SessionRemovedMessage>();
			_sessionRoomChangedMessages = new List<SessionRoomChangedMessage>();
			_speakerAddedMessages = new List<SpeakerAddedMessage>();
			_speakerRemovedMessages = new List<SpeakerRemovedMessage>();
		}


		public void Save()
		{
			bool isNew = false;
			if (!this.isSaved)
			{
				if (_id == default(Guid))
				{
					_id = Guid.NewGuid();
				}
				slug = name.GenerateSlug();
				dateAdded = DateTime.Now;
				isSaved = true;
				isNew = true;
			}
			_repository.Save(this);

			if (isNew)
			{
				_conferenceCreatedMessages.Add(new ConferenceCreatedMessage() { ConferenceSlug = this.slug, ConferenceName = this.name });
			}
			else
			{
				_conferenceUpdatedMessages.Add(new ConferenceUpdatedMessage() { ConferenceSlug = this.slug });
			}

			PublishEvents();
		}

		private void PublishEvents()
		{
			foreach (var message in _conferenceStartDateChangedMessages)
			{
				_hub.Publish(message);
			}
			_conferenceStartDateChangedMessages.Clear();

			foreach (var message in _conferenceCreatedMessages)
			{
				_hub.Publish(message);
			}
			_conferenceCreatedMessages.Clear();

			foreach (var message in _conferenceEndDateChangedMessages)
			{
				_hub.Publish(message);
			}
			_conferenceEndDateChangedMessages.Clear();

			foreach (var message in _conferenceLocationChangedMessages)
			{
				_hub.Publish(message);
			}
			_conferenceLocationChangedMessages.Clear();

			foreach (var message in _conferencePublishedMessages)
			{
				_hub.Publish(message);
			}
			_conferencePublishedMessages.Clear();

			foreach (var message in _conferenceUpdatedMessages)
			{
				_hub.Publish(message);
			}
			_conferenceUpdatedMessages.Clear();

			foreach (var message in _sessionAddedMessages)
			{
				_hub.Publish(message);
			}
			_sessionAddedMessages.Clear();

			foreach (var message in _sessionRemovedMessages)
			{
				_hub.Publish(message);
			}
			_sessionRemovedMessages.Clear();

			foreach (var message in _sessionRoomChangedMessages)
			{
				_hub.Publish(message);
			}
			_sessionRoomChangedMessages.Clear();

			foreach (var message in _speakerAddedMessages)
			{
				_hub.Publish(message);
			}
			_speakerAddedMessages.Clear();

			foreach (var message in _speakerRemovedMessages)
			{
				_hub.Publish(message);
			}
			_speakerRemovedMessages.Clear();
		}

		public void Publish()
		{
			this.datePublished = DateTime.Now;
			this.isLive = true;

			_conferencePublishedMessages.Add(new ConferencePublishedMessage() { ConferenceName = this.name, ConferenceSlug = this.slug });
		}

		public void AddSession(SessionEntity session)
		{
			if (_sessions == null)
			{
				_sessions = new List<SessionEntity>();
			}

			session.RoomChanged += SessionRoomChangedHandler;

			_sessions.Add(session);

			if (!_isInitializingFromBson && isSaved)
				_sessionAddedMessages.Add(new SessionAddedMessage() { SessionSlug = session.slug, SessionTitle = session.title });
		}

		public void RemoveSession(SessionEntity session)
		{
			if (_sessions == null)
			{
				_sessions = new List<SessionEntity>();
			}
			_sessions.Remove(session);
			if (!_isInitializingFromBson && isSaved)
				_sessionRemovedMessages.Add(new SessionRemovedMessage() { ConferenceName = this.name, SessionSlug = session.slug, SessionTitle = session.title });
		}

		public void AddSpeakerToSession(string sessionSlug, SpeakerEntity speaker)
		{
			if (this.sessions == null)
				throw new ArgumentException("Cannot add speaker to session. Conference " + slug + " has no sessions.");

			var session = this.sessions.SingleOrDefault(s => s.slug == sessionSlug);

			if (session == null)
				throw new ArgumentException("Cannot add speaker to session. Conference : " + slug + " Session:" + sessionSlug);

			session.AddSpeaker(speaker);

			if (!_isInitializingFromBson && isSaved)
				_speakerAddedMessages.Add(new SpeakerAddedMessage() { SessionSlug = sessionSlug, SessionTitle = session.title, SpeakerSlug = speaker.slug, SpeakerName = speaker.fullName });
		}

		public void RemoveSpeakerFromSession(string sessionSlug, SpeakerEntity speaker)
		{
			if (this.sessions == null)
				return;

			var session = this.sessions.SingleOrDefault(s => s.slug == sessionSlug);

			if (session == null)
				return;

			session.RemoveSpeaker(speaker);

			if (!_isInitializingFromBson && isSaved)
				_speakerRemovedMessages.Add(new SpeakerRemovedMessage() { SessionSlug = sessionSlug, SessionTitle = session.title, SpeakerSlug = speaker.slug, SpeakerName = speaker.fullName });
		}


		[BsonId(IdGenerator = typeof(CombGuidGenerator))]
		public Guid _id { get; private set; }
		public bool isLive { get; private set; }
		public string slug { get; set; }
		public DateTime datePublished { get; private set; }
		public bool isSaved { get; private set; }
		public string name { get; set; }
		private DateTime? _start;
		public DateTime? start
		{
			get { return _start; }
			set
			{
				if (!_isInitializingFromBson && isSaved && _start != value)
				{
					_conferenceStartDateChangedMessages.Add(new ConferenceStartDateChangedMessage() { ConferenceSlug = this.slug, ConferenceName = this.name, OldValue = _start, NewValue = value });
				}

				_start = value;
			}
		}

		private DateTime? _end;
		public DateTime? end
		{
			get { return _end; }
			set
			{
				if (!_isInitializingFromBson && isSaved && _end != value)
					_conferenceEndDateChangedMessages.Add(new ConferenceEndDateChangedMessage() { ConferenceName = this.name, ConferenceSlug = this.slug, OldValue = _end, NewValue = value });

				_end = value;
			}
		}

		public DateTime callForSpeakersOpens { get; set; }
		public DateTime callForSpeakersCloses { get; set; }
		public DateTime registrationOpens { get; set; }
		public DateTime registrationCloses { get; set; }
		public DateTime dateAdded { get; set; }
		private string _location;
		public string location
		{
			get { return _location; }
			set
			{
				if (!_isInitializingFromBson && isSaved && _location != value)
					_conferenceLocationChangedMessages.Add(new ConferenceLocationChangedMessage() { ConferenceSlug = this.slug, ConferenceName = this.name, OldValue = _location, NewValue = value });

				_location = value;
			}
		}

		public AddressEntity address { get; set; }
		public string description { get; set; }
		public string imageUrl { get; set; }
		public string tagLine { get; set; }
		public string facebookUrl { get; set; }
		public string homepageUrl { get; set; }
		public string lanyrdUrl { get; set; }
		public string twitterHashTag { get; set; }
		public string twitterName { get; set; }
		public string meetupUrl { get; set; }
		public string googlePlusUrl { get; set; }
		public string vimeoUrl { get; set; }
		public string youtubeUrl { get; set; }
		public string githubUrl { get; set; }
		public string linkedInUrl { get; set; }

		private IList<SessionEntity> _sessions = new List<SessionEntity>();

		public IEnumerable<SessionEntity> sessions
		{
			get { return _sessions.AsEnumerable(); }
			set
			{
				if (value == null)
					value = new List<SessionEntity>();

				_sessions = value.ToList();
				foreach (var session in _sessions)
				{
					session.RoomChanged += SessionRoomChangedHandler;
				}
			}
		}

		private void SessionRoomChangedHandler(SessionEntity sessionEntity, RoomChanged e)
		{
			if (!_isInitializingFromBson && isSaved)
				_sessionRoomChangedMessages.Add(new SessionRoomChangedMessage() { ConferenceSlug = this.slug, SessionSlug = e.SessionSlug, SessionTitle = sessionEntity.title, OldValue = e.OldValue, NewValue = e.NewValue });
		}

		

		public void BeginInit()
		{
			_isInitializingFromBson = true;
		}

		public void EndInit()
		{
			_isInitializingFromBson = false;
		}
	}
}