﻿using HW.Models;
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
            String CustomersCampanyId = "";
            String EmployeesFirstName = "";
            String EmployeeID = "";
            String ShippersCompanyName = "";
            String ShipperID = "";

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
                CustomersCampanyId = item.CustomerID.ToString();

                EmployeesFirstName = item.Employees.FirstName;
                EmployeeID = item.EmployeeID.ToString();

                ShippersCompanyName = item.Shippers.CompanyName;
                ShipperID = item.ShipperID.ToString();

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
            ViewBag.CustomersCampanyId = CustomersCampanyId;

            ViewBag.EmployeesFirstName = EmployeesFirstName;
            ViewBag.EmployeeID = EmployeeID;

            ViewBag.ShippersCompanyName = ShippersCompanyName;
            ViewBag.ShipperID = ShipperID;

            ViewBag.orderDate = orderDate;
            ViewBag.requireDate = requireDate;
            ViewBag.shipDate = shipDate;

            //客戶名稱
            List<Customers> bingocomName = db.Customers.Where(x=>x.CompanyName!= CustomersCompanyName).ToList();//找全部Customers
            ViewBag.custcompanyNameList = bingocomName;

            //負責員工名稱
            List<Employees> bingofirstName = db.Employees.Where(x => x.FirstName != EmployeesFirstName).ToList();
            ViewBag.firstNameList = bingofirstName;

            //出貨公司名稱
            List<Shippers> bingocompanyName = db.Shippers.Where(x => x.CompanyName != ShippersCompanyName).ToList();
            ViewBag.shipcompanyNameList = bingocompanyName;

            //找到明細資料
            List<OrderDetails> orderDetail = db.OrderDetails.Where(x => x.OrderID.ToString() == orderId).ToList();
            ViewBag.orderDetail = orderDetail;

            //產品的list
            List<Products> productData = db.Products.ToList();
            ViewBag.productData = productData;

            Double total = 0;
            foreach (var item in orderDetail)
            {
                total += item.Qty * Convert.ToDouble(item.UnitPrice);
            }
            total = Math.Round(total + Convert.ToDouble(data.Freight));
            ViewBag.total = total;

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
        
        //找到對應的產品 傳價錢過去
        public String orderPrice(int productId) {
            String price = db.Products.Find(productId).UnitPrice.ToString();
            return price;
        }

        [HttpPost]
        public ActionResult update(FormCollection inputs) {
            String orderId= inputs["orderId"];//用名字找value
            String custId = inputs["custName"];
            String empId = inputs["emp"];
            String orderDate = inputs["orderDate"];
            String requestDate = inputs["requestDate"];
            String shipDate = inputs["shipDate"];
            String shipNameId = inputs["shipName"];
            String cost = inputs["cost"];
            String shipcountry = inputs["shipcountry"];
            String shipcity = inputs["shipcity"];
            String shiparea = inputs["shiparea"];
            String shipaddno = inputs["shipaddno"];
            String shipaddress = inputs["shipaddress"];
            String shipdesc = inputs["shipdesc"];

            Orders data = db.Orders.Find(Convert.ToInt32(orderId));

            data.CustomerID = Convert.ToInt32(custId);
            data.EmployeeID = Convert.ToInt32(empId);
            data.OrderDate = Convert.ToDateTime(orderDate);
            data.RequiredDate = Convert.ToDateTime(requestDate);
            data.ShippedDate = Convert.ToDateTime(shipDate);
            data.ShipperID = Convert.ToInt32(shipNameId);
            data.Freight = Convert.ToDecimal(cost);
            data.ShipCountry = shipcountry;
            data.ShipCity = shipcity;
            data.ShipRegion = shiparea;
            data.ShipPostalCode = shipaddno;
            data.ShipAddress = shipaddress;
            data.ShipName = shipdesc;

            int count = 0;
            for (int i = 1; i < inputs.Count; i++)
            {
                if (inputs.AllKeys.Contains("productName[" + i + "]"))
                {
                    count++;
                }
            }
            List<OrderDetails> detailData = db.OrderDetails.Where(x => x.OrderID == data.OrderID).ToList();

            //資料庫原本有的資料修改
            int j = 0;
            foreach (var item in detailData)
            {
                j++;
                item.ProductID = Convert.ToInt32(inputs["productName[" + j + "]"]);
                item.UnitPrice = Convert.ToDecimal(inputs["price[" + j + "]"]);
                item.Qty = Convert.ToInt16(inputs["qty[" + j + "]"]);

                data.OrderDetails.Add(item);
            }

            //新增的detail資料
            for (int i = j+1; i <= count; i++)
            {
                OrderDetails addorderDetail = new OrderDetails();

                addorderDetail.OrderID = data.OrderID;
                addorderDetail.ProductID = Convert.ToInt32(inputs["productName[" + i + "]"]);
                addorderDetail.UnitPrice = Convert.ToDecimal(inputs["price[" + i + "]"]);
                addorderDetail.Qty = Convert.ToInt16(inputs["qty[" + i + "]"]);

                data.OrderDetails.Add(addorderDetail);
            }
            db.SaveChanges();
            return RedirectToAction("Index");
            // Content("修改成功");
        }

        public ActionResult add() {
            int orderId = db.Orders.Select(x => x.OrderID).Max()+1;
            ViewBag.orderId = orderId;

            //找出下拉式選單全部欄位
            List<Customers> customers = db.Customers.ToList();
            List<Employees> employees = db.Employees.ToList();
            List<Shippers> shippers = db.Shippers.ToList();
            List<Products> products = db.Products.ToList();
            ViewBag.customers = customers;
            ViewBag.employees = employees;
            ViewBag.shippers = shippers;
            ViewBag.products = products;
            return View();
        }
        [HttpPost]
        public ActionResult add(FormCollection inputs)
        {
            //取得
            String orderId = inputs["orderId"];//用名字找value
            String custId = inputs["custName"];
            String empId = inputs["emp"];
            String orderDate = inputs["orderDate"];
            String requestDate = inputs["requestDate"];
            String shipDate = inputs["shipDate"];
            String shipNameId = inputs["shipName"];
            String cost = inputs["cost"];
            String shipcountry = inputs["shipcountry"];
            String shipcity = inputs["shipcity"];
            String shiparea = inputs["shiparea"];
            String shipaddno = inputs["shipaddno"];
            String shipaddress = inputs["shipaddress"];
            String shipdesc = inputs["shipdesc"];

            Orders data = new Orders();
            //修改
            data.CustomerID = Convert.ToInt32(custId);
            data.EmployeeID = Convert.ToInt32(empId);
            data.OrderDate = Convert.ToDateTime(orderDate);
            data.RequiredDate = Convert.ToDateTime(requestDate);
            data.ShippedDate = Convert.ToDateTime(shipDate);
            data.ShipperID = Convert.ToInt32(shipNameId);
            data.Freight = Convert.ToDecimal(cost);
            data.ShipCountry = shipcountry;
            data.ShipCity = shipcity;
            data.ShipRegion = shiparea;
            data.ShipPostalCode = shipaddno;
            data.ShipAddress = shipaddress;
            data.ShipName = shipdesc;

            db.Orders.Add(data);

            //看有幾個select/product
            int count = 0;
            for (int i = 1; i < inputs.Count; i++) {
                if (inputs.AllKeys.Contains("productName[" + i + "]")) {
                    count++;
                }
            }
            for (int i = 1; i <= count; i++) {
                OrderDetails detailData = new OrderDetails();
                detailData.OrderID = data.OrderID;
                detailData.ProductID = Convert.ToInt32(inputs["productName[" + i + "]"]);
                detailData.UnitPrice = Convert.ToDecimal(inputs["price[" + i + "]"]);
                detailData.Qty = Convert.ToInt16(inputs["qty[" + i + "]"]);

                data.OrderDetails.Add(detailData);
            }

            db.SaveChanges();
            return RedirectToAction("Index");
            // 或者是打=> Content("修改成功");
        }

        public void delete(int orderId) {
            Orders order = db.Orders.Find(orderId); //找到orderId的order單
            //orderdetail單
            List<OrderDetails> orderDetail = db.OrderDetails.Where(x => x.OrderID == orderId).ToList();
           
            db.Orders.Remove(order);
            db.OrderDetails.RemoveRange(orderDetail);
            db.SaveChanges();
                  
        }
    }
}