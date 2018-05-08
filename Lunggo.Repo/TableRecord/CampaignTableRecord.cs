using System;
using System.Collections.Generic;
using System.Linq;
using Lunggo.Framework.Database;

namespace Lunggo.Repository.TableRecord
{
    public class CampaignTableRecord : Lunggo.Framework.Database.TableRecord
    {
		private static List<ColumnMetadata> _recordMetadata;
        private static List<ColumnMetadata> _primaryKeys;
        private static String _tableName;

		public long? CampaignId
		{
		    get { return _CampaignId; }
		    set
		    {
		        _CampaignId = value;
		        IncrementLog("CampaignId");
		    }
		}
		public String Name
		{
		    get { return _Name; }
		    set
		    {
		        _Name = value;
		        IncrementLog("Name");
		    }
		}
		public String Description
		{
		    get { return _Description; }
		    set
		    {
		        _Description = value;
		        IncrementLog("Description");
		    }
		}
		public DateTime? StartDate
		{
		    get { return _StartDate; }
		    set
		    {
		        _StartDate = value;
		        IncrementLog("StartDate");
		    }
		}
		public DateTime? EndDate
		{
		    get { return _EndDate; }
		    set
		    {
		        _EndDate = value;
		        IncrementLog("EndDate");
		    }
		}
		public Decimal? ValuePercentage
		{
		    get { return _ValuePercentage; }
		    set
		    {
		        _ValuePercentage = value;
		        IncrementLog("ValuePercentage");
		    }
		}
		public Decimal? ValueConstant
		{
		    get { return _ValueConstant; }
		    set
		    {
		        _ValueConstant = value;
		        IncrementLog("ValueConstant");
		    }
		}
		public Decimal? MaxDiscountValue
		{
		    get { return _MaxDiscountValue; }
		    set
		    {
		        _MaxDiscountValue = value;
		        IncrementLog("MaxDiscountValue");
		    }
		}
		public Decimal? MinSpendValue
		{
		    get { return _MinSpendValue; }
		    set
		    {
		        _MinSpendValue = value;
		        IncrementLog("MinSpendValue");
		    }
		}
		public int? VoucherCount
		{
		    get { return _VoucherCount; }
		    set
		    {
		        _VoucherCount = value;
		        IncrementLog("VoucherCount");
		    }
		}
		public String DisplayName
		{
		    get { return _DisplayName; }
		    set
		    {
		        _DisplayName = value;
		        IncrementLog("DisplayName");
		    }
		}
		public String ProductType
		{
		    get { return _ProductType; }
		    set
		    {
		        _ProductType = value;
		        IncrementLog("ProductType");
		    }
		}

		
		private long? _CampaignId;
		private String _Name;
		private String _Description;
		private DateTime? _StartDate;
		private DateTime? _EndDate;
		private Decimal? _ValuePercentage;
		private Decimal? _ValueConstant;
		private Decimal? _MaxDiscountValue;
		private Decimal? _MinSpendValue;
		private int? _VoucherCount;
		private String _DisplayName;
		private String _ProductType;


		public static CampaignTableRecord CreateNewInstance()
        {
            var record = new CampaignTableRecord();
            var iRecord = record.AsInterface();
            iRecord.ManuallyCreated = true;
            return record;
        }

		public CampaignTableRecord()
        {
            ;
        }

        static CampaignTableRecord()
        {
            InitTableName();
            InitRecordMetadata();
            InitPrimaryKeysMetadata();
        }

        private static void InitTableName()
        {
            _tableName = "Campaign";
        }

        private static void InitRecordMetadata()
        {
            _recordMetadata = new List<ColumnMetadata>
            {
				new ColumnMetadata("CampaignId", true),
				new ColumnMetadata("Name", false),
				new ColumnMetadata("Description", false),
				new ColumnMetadata("StartDate", false),
				new ColumnMetadata("EndDate", false),
				new ColumnMetadata("ValuePercentage", false),
				new ColumnMetadata("ValueConstant", false),
				new ColumnMetadata("MaxDiscountValue", false),
				new ColumnMetadata("MinSpendValue", false),
				new ColumnMetadata("VoucherCount", false),
				new ColumnMetadata("DisplayName", false),
				new ColumnMetadata("ProductType", false),

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
