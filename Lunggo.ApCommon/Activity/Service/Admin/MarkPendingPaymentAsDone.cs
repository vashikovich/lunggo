﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunggo.ApCommon.Activity.Service
{
    public partial class ActivityService
    {
        public bool MarkPendingPaymentAsDone(string rsvNo, string pendingPaymentStatus)
        {
            var output = MarkPendingPaymentAsDoneDb(rsvNo, pendingPaymentStatus);
            return output;
        }
    }
}