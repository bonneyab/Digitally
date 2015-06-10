using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Digitally.Controllers
{
    public class MobileController : Controller
    {
        private readonly ReportingEntities _dbContext = new ReportingEntities();
        private readonly List<int> _hours = new List<int> {10, 11, 12, 13, 14, 15, 16, 17, 18};
        private readonly List<string> _ageGroups = new List<string> {"Child", "Teen", "Young Adult", "Adult", "Senior"};

        public ActionResult Index(DateTime? date = null, int increment = 0)
        {
            if (date == null)
            {
                date = DateTime.Today.AddHours(DateTime.Now.Hour);
            }
            date = date.Value.AddHours(increment);
            //if(date.Value.Hour > _hours.Max())

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            var results = _dbContext.Attendances.Where(a => a.date >= today && a.date < tomorrow && a.group_id == null);
            var model = new TallyModel
            {
                Date = date.Value,
                Attendances = results.Where(r => r.date.Hour == date.Value.Hour).ToList(),
                IsCreated = results.Any()
            };

            return
                View(model);
        }

        //TODO: duplicated for the sake of simplicity for now...
        //Need to move these database calls out
        public ActionResult Create(DateTime date)
        {
            date = DateTime.Today;
            var tomorrow = date.AddDays(1);
            var exists = _dbContext.Attendances.Any(a => a.date >= date && a.date < tomorrow && a.group_id == null);
            if (!exists)
            {
                var attendances = new List<Attendance>();
                var currentUser = Environment.UserName;
                foreach (var hour in _hours)
                {
                    attendances.AddRange(
                        _ageGroups.Select(
                            age =>
                                new Attendance
                                {
                                    date = date.AddHours(hour),
                                    CreatedBy = currentUser,
                                    ModifiedBy = currentUser,
                                    total = 0,
                                    type = age,
                                    CreatedDate = DateTime.Now,
                                    ModifiedDate = DateTime.Now,
                                }));
                }
                _dbContext.Attendances.AddRange(attendances);
                _dbContext.SaveChanges();
            }
            return RedirectToAction("Index", new { date = DateTime.Today.AddHours(DateTime.Now.Hour) });
        }
    }

    public class TallyModel
    {
        public DateTime Date { get; set; }
        public List<Attendance> Attendances { get; set; }
        public bool IsCreated { get; set; }
    }
}