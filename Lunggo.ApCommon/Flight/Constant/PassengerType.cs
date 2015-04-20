﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunggo.ApCommon.Flight.Constant
{
    public enum PassengerType
    {
        Undefined = 0,
        Adult = 1,
        Child = 2,
        Infant = 3
    }

    public class PassengerTypeCd
    {
        public static string Mnemonic(PassengerType type)
        {
            switch (type)
            {
                case PassengerType.Adult:
                    return "ADT";
                    case PassengerType.Child:
                    return "CHD";
                    case PassengerType.Infant:
                    return "INF";
                default:
                    return "";
            }
        }

        public static PassengerType Mnemonic(string type)
        {
            switch (type)
            {
                case "ADT":
                    return PassengerType.Adult;
                case "CHD":
                    return PassengerType.Child;
                case "INF":
                    return PassengerType.Infant;
                default:
                    return PassengerType.Undefined;
            }
        }
    }
}