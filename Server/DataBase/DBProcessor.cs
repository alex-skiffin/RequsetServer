﻿using System;
using System.Linq;
using MongoDB.Driver;

namespace Server.DataBase
{
    public class DBProcessor
    {
        private static MongoCollection<AnonimusInfo> _anonimusCollection;

        internal static MongoDatabase Database;
        
        public DBProcessor()
        {
            var settings = new MongoClientSettings
            {
                Servers = new[] {new MongoServerAddress("127.0.0.1")}
            };

            var mongoClient = new MongoClient(settings);
            if (mongoClient == null)
                throw new MongoAuthenticationException("Не удалось подключить MongoClient к серверу");

            var server = mongoClient.GetServer();
            Database = server.GetDatabase("anon");
            Database.CollectionExists("info");
            _anonimusCollection = Database.GetCollection<AnonimusInfo>("info");
        }

        public AnonimusInfo GetInfo()
        {
            return _anonimusCollection.FindOne();
        }
        public AnonimusInfo GetInfo(Guid id)
        {
            return _anonimusCollection.FindOneById(id);
        }

        public void AddInfo(Guid infoId, string info)
        {
        }
        public void AddInfo(string info)
        {
        }
    }
}