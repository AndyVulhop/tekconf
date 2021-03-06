﻿using System;
using System.Linq;

using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace TekConf.Common.Entities
{
	public class SpeakerRemovedRepository : IRepository<SpeakerRemovedMessage>
	{
		private readonly IEntityConfiguration _entityConfiguration;

		public SpeakerRemovedRepository(IEntityConfiguration entityConfiguration)
		{
			this._entityConfiguration = entityConfiguration;
		}

		public void Save(SpeakerRemovedMessage entity)
		{
			var collection = MongoCollection();
			collection.Save(entity);
		}

		public IQueryable<SpeakerRemovedMessage> AsQueryable()
		{
			var collection = MongoCollection();
			return collection.AsQueryable();
		}

		public void Remove(Guid id)
		{
			var collection = this.LocalDatabase.GetCollection<SpeakerRemovedMessage>("speakerRemovedEvents");
			collection.Remove(Query.EQ("_id", id));
		}

		private MongoCollection<SpeakerRemovedMessage> MongoCollection()
		{
			var collection = this.LocalDatabase.GetCollection<SpeakerRemovedMessage>("speakerRemovedEvents");
			return collection;
		}

		private MongoServer _localServer;
		private MongoDatabase _localDatabase;
		private MongoDatabase LocalDatabase
		{
			get
			{
				if (_localServer == null)
				{
					var mongoServer = this._entityConfiguration.MongoServer;
					_localServer = MongoServer.Create(mongoServer);
				}

				if (_localDatabase == null)
				{
					_localDatabase = _localServer.GetDatabase("tekconf");

				}
				return _localDatabase;
			}
		}
	}


}