using System;
using System.Collections.Generic;
using System.Linq;
using Lunggo.Framework.Database;

namespace Lunggo.Repository.TableRecord
{
    public class NotificationTableRecord : Lunggo.Framework.Database.TableRecord
    {
		private static List<ColumnMetadata> _recordMetadata;
        private static List<ColumnMetadata> _primaryKeys;
        private static String _tableName;

		public String Handle
		{
		    get { return _Handle; }
		    set
		    {
		        _Handle = value;
		        IncrementLog("Handle");
		    }
		}
		public String DeviceId
		{
		    get { return _DeviceId; }
		    set
		    {
		        _DeviceId = value;
		        IncrementLog("DeviceId");
		    }
		}

		
		private String _Handle;
		private String _DeviceId;


		public static NotificationTableRecord CreateNewInstance()
        {
            var record = new NotificationTableRecord();
            var iRecord = record.AsInterface();
            iRecord.ManuallyCreated = true;
            return record;
        }

		public NotificationTableRecord()
        {
            ;
        }

        static NotificationTableRecord()
        {
            InitTableName();
            InitRecordMetadata();
            InitPrimaryKeysMetadata();
        }

        private static void InitTableName()
        {
            _tableName = "Notification";
        }

        private static void InitRecordMetadata()
        {
            _recordMetadata = new List<ColumnMetadata>
            {
				new ColumnMetadata("Handle", true),
				new ColumnMetadata("DeviceId", true),

            };
        }

        private static void InitPrimaryKeysMetadata()
        {
            _primaryKeys = _recordMetadata.Where(p => p.IsPrimaryKey).ToList();
        }

		public override List<ColumnMetadata> GetMetadata()
        {
            return _recordMetadata;
        }

        public override string GetTableName()
        {
            return _tableName;
        }

        public override List<ColumnMetadata> GetPrimaryKeys()
        {
            return _primaryKeys;
        }



    }
}