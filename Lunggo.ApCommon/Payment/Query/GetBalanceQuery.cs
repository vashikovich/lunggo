﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lunggo.Framework.Database;
using Lunggo.Repository.TableRecord;

namespace Lunggo.ApCommon.Payment.Query
{
    internal class GetBalanceQuery : DbQueryBase<GetBalanceQuery, AccountBalanceTableRecord>
    {
        protected override string GetQuery(dynamic condition = null)
        {
            var getAccountNo = "(SELECT AccountNo FROM AccountUser WHERE UserId = @userId)";
            return "SELECT * FROM AccountBalance WHERE AccountNo = " + getAccountNo;
        }
    }
}
