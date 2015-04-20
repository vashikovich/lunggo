﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lunggo.ApCommon.Flight.Constant;
using Lunggo.ApCommon.Flight.Interface;
using Lunggo.ApCommon.Flight.Model;
using Lunggo.ApCommon.Flight.Model;
using Lunggo.ApCommon.Mystifly.OnePointService.Flight;

namespace Lunggo.ApCommon.Mystifly
{
    internal partial class MystiflyWrapper : WrapperBase
    {
        private static readonly MystiflyWrapper Instance = new MystiflyWrapper();
        private bool _isInitialized;

        private MystiflyWrapper()
        {
            
        }

        internal static MystiflyWrapper GetInstance()
        {
            return Instance;
        }

        internal void Init(string accountNumber, string userName, string password, TargetServer target)
        {
            MystiflyClientHandler.Init(accountNumber, userName, password, target);
            if (!_isInitialized)
            {
                _isInitialized = true;
            }
            else
            {
                throw new InvalidOperationException("MystiflyWrapper is already initialized");
            }
        }
    }
}