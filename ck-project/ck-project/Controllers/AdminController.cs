﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ck_project.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        ckdatabase db = new ckdatabase();

        public ActionResult AdminMainPage(string search, string searchby)
        {
            if (searchby == "Designer" && search != "")
            {
                var result = (from l in db.leads.Take(10)
                              join e in db.employees on l.emp_number equals e.emp_number
                              join s in db.project_status on l.project_status_number equals s.project_status_number
                              where s.project_status_name != "Closed"
                              where l.deleted == false
                              where (e.emp_firstname.StartsWith(search) || e.emp_lastname.StartsWith(search))
                              orderby l.Last_update_date
                              select l);
                return View(result);
            }
            else if (searchby == "Status" && search != "")
            {
                var result = (from l in db.leads.Take(10)
                              join s in db.project_status on l.project_status_number equals s.project_status_number
                              where (s.project_status_name != "Closed" && s.project_status_name.StartsWith(search))
                              where l.deleted == false
                              orderby l.Last_update_date
                              select l);
                return View(result);
            }
            else
            {
                var result = (from l in db.leads.Take(10)
                              join s in db.project_status on l.project_status_number equals s.project_status_number
                              where s.project_status_name != "Closed"
                              where l.deleted == false
                              orderby l.Last_update_date
                              select l);
                return View(result);
            }
        }
    }
}