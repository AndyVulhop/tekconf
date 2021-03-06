﻿using System;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using TekConf.Common.Entities.Messages;

namespace TekConf.Common.Entities
{
	public class ScheduleCreatedRepository : IRepository<ScheduleCreatedMessage>
	{
		private readonly IEntityConfiguration _entityConfiguration;
		private const string dbName = "scheduleCreatedEvents";

		public ScheduleCreatedRepository(IEntityConfiguration entityConfiguration)
		{
			this._entityConfiguration = entityConfiguration;
		}

		public void Save(ScheduleCreatedMessage entity)
		{
			var collection = MongoCollection();
			collection.Save(entity);
		}

		public IQueryable<ScheduleCreatedMessage> AsQueryable()
		{
			var collection = MongoCollection();
			return collection.AsQueryable();
		}

		public void Remove(Guid id)
		{
			var collection = this.LocalDatabase.GetCollection<ScheduleCreatedMessage>(dbName);
			collection.Remove(Query.EQ("_id", id));
		}

		private MongoCollection<ScheduleCreatedMessage> MongoCollection()
		{
			var collection = this.LocalDatabase.GetCollection<ScheduleCreatedMessage>(dbName);
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