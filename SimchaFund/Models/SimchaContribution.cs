using SimchaFund.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimchaFund.Models
{
    public class SimchaContribution
    {
        public IEnumerable<SimchaContributor> GetContributors { get; set; }
        public Simcha Simcha { get; set; }
    }
}