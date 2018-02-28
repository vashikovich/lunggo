﻿using Lunggo.Framework.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunggo.ApCommon.Account.Model.Database
{

    public class UpdateDateResetPasswordToDbQuery : NoReturnDbQueryBase<UpdateDateResetPasswordToDbQuery>
    {
        protected override string GetQuery(dynamic condition = null)
        {
            return "UPDATE ResetPassword SET OtpHash = @OtpHash, ExpireTime = @ExpireTime WHERE PhoneNumber = @Contact OR Email = @Contact";
        }
    }
}