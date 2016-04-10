using HW.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        //[HttpPost]
        //public ActionResult search(FormCollection update)
        //{
        //    String orderId = update["orderId"];

        //    return RedirectToAction("update",
        //        new { orderId});
        //}

        public ActionResult update(String orderId)
        {
            //List<型態>
            List<Orders> bingo = db.Orders.Where(x => x.OrderID.ToString() == orderId).ToList();//OrderID存到一個List

            Orders data = new Orders();
            String CustomersCompanyName="";
            String EmployeesFirstName = "";
            String ShippersCompanyName = "";

            String orderDate = "";
            String orderMonth = "";
            String orderDay = "";

            String requireDate = "";
            String requireMonth = "";
            String requireDay = "";

            String shipDate = "";
            String shipMonth = "";
            String shipDay = "";


            foreach (var item in bingo) //item是一個小變數，bingo是list，有list就要用foreach
            {
                data.OrderID = item.OrderID;
                data.Freight = item.Freight;
                data.ShipCountry = item.ShipCountry;
                data.ShipCity = item.ShipCity;
                data.ShipRegion = item.ShipRegion;
                data.ShipPostalCode = item.ShipPostalCode;
                data.ShipAddress = item.ShipAddress;
                data.ShipName = item.ShipName;

                CustomersCompanyName = item.Customers.CompanyName;
                EmployeesFirstName = item.Employees.FirstName;
                ShippersCompanyName = item.Shippers.CompanyName;

                //日期，alterDate是function傳到下面
                //orderDate
                orderMonth =alterDate(item.OrderDate.Month.ToString());
                orderDay =alterDate(item.OrderDate.Day.ToString());
                orderDate = String.Format("{0}-{1}-{2}", item.OrderDate.Year, orderMonth, orderDay);

                //requireDate
                requireMonth = alterDate(item.RequiredDate.Month.ToString());
                requireDay =alterDate( item.RequiredDate.Day.ToString());
                requireDate= String.Format("{0}-{1}-{2}", item.RequiredDate.Year, requireMonth, requireDay);

                //ShippedDate
                shipMonth = alterDate(Convert.ToDateTime(item.ShippedDate).Month.ToString());
                shipDay = alterDate(Convert.ToDateTime(item.ShippedDate).Day.ToString());
                shipDate = String.Format("{0}-{1}-{2}", Convert.ToDateTime(item.ShippedDate).Year, shipMonth, shipDay);
            }

            ViewBag.data = data;
            ViewBag.CustCompanyName = CustomersCompanyName;
            ViewBag.EmployeesFirstName = EmployeesFirstName;
            ViewBag.ShippersCompanyName = ShippersCompanyName;

            ViewBag.orderDate = orderDate;
            ViewBag.requireDate = requireDate;
            ViewBag.shipDate = shipDate;

            //客戶名稱
            List<String> bingocomName = db.Customers.Where(x=>x.CompanyName!= CustomersCompanyName).Select(x=>x.CompanyName).ToList();//找全部Customers
            ViewBag.custcompanyNameList = bingocomName;

            //負責員工名稱
            List<String> bingofirstName = db.Employees.Where(x => x.FirstName != EmployeesFirstName).Select(x => x.FirstName).ToList();
            ViewBag.firstNameList = bingofirstName;

            //出貨公司名稱
            List<String> bingocompanyName = db.Shippers.Where(x => x.CompanyName != ShippersCompanyName).Select(x => x.CompanyName).ToList();
            ViewBag.shipcompanyNameList = bingocompanyName;

            //找到明細資料
            List<OrderDetails> orderDetail = db.OrderDetails.Where(x => x.OrderID.ToString() == orderId).ToList();
            ViewBag.orderDetail = orderDetail;

            //產品的list
            List<Products> productData = db.Products.ToList();
            ViewBag.productData = productData;

            return View();
        }

        //我是日期的function
        public String alterDate(String date) {
            StringBuilder change = new StringBuilder();
            if (date.Length < 2) {
                change.Append("0");//append字串相加
            }
            change.Append(date);
            return change.ToString();
        }

        public ActionResult add()
        {
            return View();
        }

    }
}