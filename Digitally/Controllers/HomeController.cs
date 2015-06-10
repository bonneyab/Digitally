using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Digitally.Controllers
{
    public class HomeController : Controller
    {
        readonly ReportingEntities _dbContext = new ReportingEntities();
        readonly List<int> _hours = new List<int>{10,11,12,13,14,15,16,17};
        readonly List<string> _ageGroups = new List<string> { "Child", "Teen", "Young Adult", "Adult", "Senior" }; 

        public ActionResult Index(DateTime? date = null)
        {
            if (date == null)
            {
                date = DateTime.Today;
            }
            var tomorrow = date.Value.AddDays(1);
            var results = _dbContext.Attendances.Where(a => a.date >= date.Value && a.date < tomorrow && a.group_id == null).ToList();
            var attendances = GetAttendanceGridDictionary(results);

            var groups = (from a in _dbContext.Attendances
                join g in _dbContext.GroupNames on a.group_id equals g.id
                      where a.date >= date.Value && a.date < tomorrow
                      group a by g into n
                      select new { GroupName = n.Key.group_name, GroupAttendance = n.ToList() })
                .ToList()
                .Select(g => new GroupModel { Group = GetGroupAttendanceRow(g.GroupAttendance), GroupName = g.GroupName })
                .ToList();
            return View(new AttendanceModel { Attendances = attendances, Date = date.Value, IsCreated = results.Any(), Groups = groups});
        }

        public ActionResult DeleteGroup(DateTime date, int groupId)
        {
            var attendances = _dbContext.Attendances.Where(a => a.group_id == groupId).ToList();
            var group = _dbContext.GroupNames.First(g => g.id == groupId);
            _dbContext.Attendances.RemoveRange(attendances);
            _dbContext.GroupNames.Remove(group);
            _dbContext.SaveChanges();

            return RedirectToAction("Index", new { date });
        }

        public ActionResult AddGroup(DateTime date, int hour, string groupName)
        {
            var attendances = new List<Attendance>();
            var currentUser = Environment.UserName;
            var now = DateTime.Now;
            var newGroup = new GroupName{group_name = groupName,CreatedBy = currentUser, ModifiedBy = currentUser, CreatedDate = now, ModifiedDate = now};
            _dbContext.GroupNames.Add(newGroup);
            _dbContext.SaveChanges();
            attendances.AddRange(
                _ageGroups.Select(
                    age => new Attendance
                    {
                        group_id = newGroup.id,
                        date = date.AddHours(hour),
                        CreatedBy = currentUser,
                        ModifiedBy = currentUser,
                        total = 0,
                        type = age,
                        CreatedDate = now,
                        ModifiedDate = now,
                    }));
            _dbContext.Attendances.AddRange(attendances);
            _dbContext.SaveChanges();
            return RedirectToAction("Index", new {date});
        }
        
        //TODO: duplicated for the sake of simplicity for now...
        //Need to move these database calls out
        public ActionResult Create(DateTime date)
        {
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
            return RedirectToAction("Index", new { date });
        }

        [HttpPost]
        public JsonResult UpdateTotal(int total, int id)
        {
            var attendance = _dbContext.Attendances.First(a => a.id == id);
            attendance.total = total;
            _dbContext.SaveChanges();
            return Json(new {success = true}, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult UpdateGroupName(string groupName, int id)
        {
            var attendance = _dbContext.GroupNames.First(a => a.id == id);
            attendance.group_name = groupName;
            _dbContext.SaveChanges();
            return Json(new { success = true }, JsonRequestBehavior.DenyGet);
        }

        private Dictionary<int, Dictionary<string, Attendance>> GetAttendanceGridDictionary(List<Attendance> results)
        {
            var attendances = _hours.Select(
                h => new KeyValuePair<int, Dictionary<string, Attendance>>(h,
                    results.Where(r2 => r2.date.Hour == h)
                        .Select(r2 => new KeyValuePair<string, Attendance>(r2.type, r2))
                        .ToDictionary(x => x.Key, x => x.Value)
                    )).ToDictionary(x => x.Key, x => x.Value);
            return attendances;
        }

        private Dictionary<string, Attendance> GetGroupAttendanceRow(List<Attendance> results)
        {
            return results.Select(r2 => new KeyValuePair<string, Attendance>(r2.type, r2))
                .ToDictionary(x => x.Key, x => x.Value);
        } 
    }

    public class AttendanceModel
    {
        public DateTime Date { get; set; }
        public Dictionary<int, Dictionary<string, Attendance>> Attendances{ get; set; }
        public bool IsCreated { get; set; }
        public List<GroupModel> Groups { get; set; }
    }

    public class GroupModel
    {
        public Dictionary<string, Attendance> Group { get; set; }
        public string GroupName { get; set; }
    }
}