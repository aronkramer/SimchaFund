using SimchaFund.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimchaFund.Models
{
    public class SimchaInfo
    {
        public IEnumerable<Simcha> GetSimcha { get; set; }
        public int AmountOfPpl { get; set; }
    }
}