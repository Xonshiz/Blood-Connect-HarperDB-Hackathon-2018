using System;
using System.Collections.Generic;
using System.Text;

namespace BloodConnect.models
{
    class PendingRequests
    {
        public string donation_id { get; set; }
        public string requester_id { get; set; }
        public string requester_name { get; set; }
        public string requester_country { get; set; }
        public string requester_state { get; set; }
        public string requester_area_code { get; set; }
        public string requester_phone_number { get; set; }
    }
}
