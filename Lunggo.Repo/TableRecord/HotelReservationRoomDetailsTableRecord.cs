using System;
using System.Collections.Generic;
using System.Linq;
using Lunggo.Framework.Database;

namespace Lunggo.Repository.TableRecord
{
    public class HotelReservationRoomDetailsTableRecord : Lunggo.Framework.Database.TableRecord
    {
		private static List<ColumnMetadata> _recordMetadata;
        private static List<ColumnMetadata> _primaryKeys;
        private static String _tableName;

		

		public static HotelReservationRoomDetailsTableRecord CreateNewInstance()
        {
            var record = new HotelReservationRoomDetailsTableRecord();
            var iRecord = record.AsInterface();
            iRecord.ManuallyCreated = true;
            return record;
        }

		public HotelReservationRoomDetailsTableRecord()
        {
            ;
        }

        static HotelReservationRoomDetailsTableRecord()
        {
            InitTableName();
            InitRecordMetadata();
            InitPrimaryKeysMetadata();
        }

        private static void InitTableName()
        {
            _tableName = "HotelReservationRoomDetails";
        }

        private static void InitRecordMetadata()
        {
            _recordMetadata = new List<ColumnMetadata>
            {
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
