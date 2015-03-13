﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lunggo.Flight.Dictionary;

namespace Lunggo.ApCommon.Model
{
    public static class TrieIndex
    {
        public static readonly TrieNode AirlineName = new TrieNode();
        public static readonly TrieNode AirportName = new TrieNode();

        static TrieIndex()
        {
            foreach (var airline in FlightCode.Airline)
            {
                AirlineName.InsertWord(airline.Value.Abbr, airline.Key);
                AirlineName.InsertWordsBySentence(airline.Value.Full, airline.Key);
            }
            foreach (var airport in FlightCode.Airport)
            {
                AirportName.InsertWord(airport.Value.Abbr, airport.Key);
                AirportName.InsertWordsBySentence(airport.Value.Full, airport.Key);
            }
        }
    }
}
