using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimchaFund.Data
{
    public class Deposits
    {
        public int Id { get; set; }
        public int ContributorsId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
