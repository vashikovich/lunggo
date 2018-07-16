﻿using Lunggo.Framework.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunggo.ApCommon.Activity.Database.Query
{
    internal class UpdatePaymentStepStatus : NoReturnDbQueryBase<UpdatePaymentStepStatus>
    {
        protected override string GetQuery(dynamic condition = null)
        {
            return "UPDATE ActivityReservationStepOperator SET StepStatus = @StepStatus WHERE StepName = @StepName AND RsvNo = @RsvNo";
        }
    }
}
