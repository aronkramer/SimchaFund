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
    public class SimchaController : Controller
    {
        public ActionResult ListedSimcha()
        {
            DataBase db = new DataBase(Settings.Default.ConStr);
            SimchaInfo si = new SimchaInfo
            { 
                GetSimcha = db.GetSimcha(),
                AmountOfPpl = db.GetPplAmount()
            };
            return View(si);
        }

        public ActionResult AddSimcha(Simcha simcha)
        {
            DataBase db = new DataBase(Settings.Default.ConStr);
            db.AddSimcha(simcha);
            return RedirectToAction("ListedSimcha");
        }

        public ActionResult Give(int SimchaId)
        {
            DataBase db = new DataBase(Settings.Default.ConStr);
            SimchaContribution sc = new SimchaContribution
            {
                GetContributors = db.GetSimchaContributors(),
                Simcha = db.GetSimchaById(SimchaId)
            };
            return View(sc);
        }

        [HttpPost]
        public ActionResult Update(int simchaId, List<ContributionInclusion> contributors)
        {
            DataBase db = new DataBase(Settings.Default.ConStr);
            db.UpdateSimchaContributions(simchaId, contributors);
            return RedirectToAction("ListedSimcha");
        }

    }
}