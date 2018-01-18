using System;
using System.Collections.Generic;
using Lunggo.Framework.Database;
using Lunggo.Repository.TableRecord;
using System.Data;

namespace Lunggo.Repository.TableRepository
{
	public class ActivityPackageReservationTableRepo : TableDao<ActivityPackageReservationTableRecord>, IDbTableRepository<ActivityPackageReservationTableRecord> 
    {
		private static readonly ActivityPackageReservationTableRepo Instance = new ActivityPackageReservationTableRepo("ActivityPackageReservation");
        
        private ActivityPackageReservationTableRepo(String tableName) : base(tableName)
        {
            ;
        }

		public static ActivityPackageReservationTableRepo GetInstance()
        {
            return Instance;
        }

        public int Insert(IDbConnection connection, ActivityPackageReservationTableRecord record)
        {
            return Insert(connection, record, CommandDefinition.GetDefaultDefinition());
        }

        public int Delete(IDbConnection connection, ActivityPackageReservationTableRecord record)
        {
            return Delete(connection, record, CommandDefinition.GetDefaultDefinition());
        }

		public int Update(IDbConnection connection, ActivityPackageReservationTableRecord record)
        {
            return Update(connection, record, CommandDefinition.GetDefaultDefinition());
        }

		public ActivityPackageReservationTableRecord Find1(IDbConnection connection, ActivityPackageReservationTableRecord record)
        {
            return Find1(connection, record, CommandDefinition.GetDefaultDefinition());
        }

		public IEnumerable<ActivityPackageReservationTableRecord> Find(IDbConnection connection, ActivityPackageReservationTableRecord record)
        {
            return Find(connection, record, CommandDefinition.GetDefaultDefinition());
        }

        public IEnumerable<ActivityPackageReservationTableRecord> FindAll(IDbConnection connection)
        {
            return FindAll(connection, CommandDefinition.GetDefaultDefinition());
        }

        public int DeleteAll(IDbConnection connection)
        {
            return DeleteAll(connection, CommandDefinition.GetDefaultDefinition());
        }

        public int Insert(IDbConnection connection, ActivityPackageReservationTableRecord record, CommandDefinition definition)
        {
            return InsertInternal(connection, record, definition);
        }

        public int Delete(IDbConnection connection, ActivityPackageReservationTableRecord record, CommandDefinition definition)
        {
            return DeleteInternal(connection, record, definition);
        }

        public int Update(IDbConnection connection, ActivityPackageReservationTableRecord record, CommandDefinition definition)
        {
            return UpdateInternal(connection, record, definition);
        }

		public ActivityPackageReservationTableRecord Find1(IDbConnection connection, ActivityPackageReservationTableRecord record, CommandDefinition definition)
        {
			return Find1Internal(connection, record, definition);
        }

		public IEnumerable<ActivityPackageReservationTableRecord> Find(IDbConnection connection, ActivityPackageReservationTableRecord record, CommandDefinition definition)
        {
			return FindInternal(connection, record, definition);
        }

        public int DeleteAll(IDbConnection connection, CommandDefinition definition)
        {
            return DeleteAllInternal(connection, definition);
        }

        public IEnumerable<ActivityPackageReservationTableRecord> FindAll(IDbConnection connection, CommandDefinition definition)
        {
            return FindAllInternal(connection, definition);
        }
	}	
}