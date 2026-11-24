using System;
using System.Collections.Generic;
using System.Text;

namespace RadioButton
{
    public class CurrencyRates
    {
        public string result { get; set; }

        public string provider { get; set; }

        public string time_last_update_utc { get; set; }

        public string time_next_update_utc { get; set; }

        public string base_code { get; set; }

        public Dictionary<string, decimal> rates { get; set; }
    }
}
