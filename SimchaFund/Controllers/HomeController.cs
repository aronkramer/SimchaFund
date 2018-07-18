using SimchaFund.Data;
using SimchaFund.Models;
using SimchaFund.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimchaFund.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            DataBase db = new DataBase(Settings.Default.ConStr);
            return View(db.GetContributors());
        }

        public ActionResult AddContributor(Contributors contributor, Deposits deposit)
        {
            DataBase db = new DataBase(Settings.Default.ConStr);
            db.AddContributor(contributor);
            Deposits d = new Deposits
            {
                ContributorsId = contributor.Id,
                Amount = deposit.Amount,
                Date = contributor.Date
            };
            db.Deposit(d);
            return Redirect("/");
        }

        public ActionResult Depositmore(Deposits deposit)
        {
            DataBase db = new DataBase(Settings.Default.ConStr);
            db.Deposit(deposit);
            return Redirect("/");
        }

        public ActionResult UpdatePerson(Contributors contributor)
        {
            DataBase db = new DataBase(Settings.Default.ConStr);
            db.UpdateContributor(contributor);
            return Redirect("/");
        }

        public ActionResult History(int contribId)
        {
            var db = new DataBase(Properties.Settings.Default.ConStr);
            IEnumerable<Deposits> deposits = db.GetDepositsById(contribId);
            IEnumerable<Contribution> contributions = db.GetContributionsById(contribId);

            IEnumerable<Transaction> transactions = deposits.Select(d => new Transaction
            {
                Action = "Deposit",
                Amount = d.Amount,
                Date = d.Date
            }).Concat(contributions.Select(c => new Transaction
            {
                Action = "Contribution for the " + c.SimchaName + " simcha",
                Amount = -c.Amount,
                Date = c.Date
            })).OrderByDescending(t => t.Date);

            var vm = new HistoryViewModel
            {
                Transactions = transactions,   
                ContributorName = db.GetContributorsName(contribId),
                ContributorBalance = db.CurrentBalance(contribId)
            };
            
            return View(vm);
        }
    }
}