using HW.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HW.Controllers
{
    public class searchController : Controller
    {
        DB db = new DB();

        // GET: search
        public ActionResult Index()
        {
            List<String> ename= db.Employees.Select(x => x.FirstName).ToList();
            List<String> comp = db.Shippers.Select(x => x.CompanyName).ToList();
            ViewBag.ename = ename;
            ViewBag.comp = comp;
            return View();
        }

        [HttpPost]
        public ActionResult Index(FormCollection search)
        {
            String orderId = search["orderId"];
            String custName = search["custName"];
            String emp = search["emp"];
            String comp = search["comp"];
            String orderDate = search["orderDate"];
            String shipDate = search["shipDate"];
            String requestDate = search["requestDate"];

            return RedirectToAction("search",
                new { orderId, custName, emp, comp, orderDate, shipDate, requestDate });
        }

        public ActionResult search(String orderId, String custName, String emp, String comp, String orderDate, String shipDate, String requestDate)
        {
            DateTime oddt = Convert.ToDateTime(orderDate);
            DateTime shdt = Convert.ToDateTime(shipDate);
            DateTime rqdt = Convert.ToDateTime(requestDate);

            List<Orders> bingo= db.Orders.Where(x =>
                (String.IsNullOrEmpty(orderId) ? true : x.OrderID.ToString() == orderId) &&
                (String.IsNullOrEmpty(custName) ? true : x.Customers.CompanyName == custName) &&
                (String.IsNullOrEmpty(emp) ? true : x.Employees.FirstName == emp) &&
                (String.IsNullOrEmpty(comp) ? true : x.Shippers.CompanyName == comp) &&
                (String.IsNullOrEmpty(orderDate) ? true : x.OrderDate == oddt) &&
                (String.IsNullOrEmpty(shipDate) ? true : x.ShippedDate == shdt) &&
                (String.IsNullOrEmpty(requestDate) ? true : x.RequiredDate == rqdt)
                ).ToList();
            ViewBag.bingo = bingo;
            return View();
        }
    }
}