﻿using Lunggo.Framework.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunggo.ApCommon.Account.Model.Database
{
    public class UpdatePasswordToDbQuery : NoReturnDbQueryBase<UpdatePasswordToDbQuery>
    {
        protected override string GetQuery(dynamic condition = null)
        {
            return "UPDATE [User] SET PasswordHash = @NewPasswordHash WHERE PhoneNumber = @PhoneNumber";
        }
    }
}