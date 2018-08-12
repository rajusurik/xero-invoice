using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xero.Api.Core.Model;
using Xero.Api.Core.Model.Status;
using Xero.Api.Core.Model.Types;
using Xero.Api.Example.MVC.Helpers;
using Xero.Api.Infrastructure.Exceptions;

namespace Xero.Api.Example.MVC.Controllers
{
    public class InvoiceController : Controller
    {
        // GET: Invoice
        public ActionResult Index()
        {
            var api = XeroApiHelper.CoreApi();

            try
            {
                var invoicesList = api.Invoices.FindAsync().Result;
                var activeinvoicesList = invoicesList.Where(x => x.Status != InvoiceStatus.Deleted);

                return View(activeinvoicesList);
            }
            catch (RenewTokenException e)
            {
                Console.WriteLine(e);
                return RedirectToAction("Connect", "Home");
            }
        }


        // GET: Invoice/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Invoice/Create
        [HttpPost]
        public ActionResult Create(Invoice invoice)
        {

            var api = XeroApiHelper.CoreApi();
            var newInvoice = new Invoice
            {
                Contact = new Contact { Name = invoice.Contact.Name, Id = new Guid() },
                Type = InvoiceType.AccountsReceivable,
                Date = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(90),
                LineAmountTypes = LineAmountType.Exclusive,
                LineItems = new List<LineItem>
                {
                    new LineItem
                    {
                        Description="Consulting services as agreed (20% off standard rate)",
                        AccountCode = "200",
                        UnitAmount =invoice.LineItems[0].UnitAmount ,
                        Quantity = invoice.LineItems[0].Quantity,
                        DiscountRate=invoice.LineItems[0].DiscountRate
                    }
                }

            };

            try
            {
                var CreatedAccpayInvoice = api.Invoices.CreateAsync(newInvoice);
                AddApiAuditLog(newInvoice);
                return RedirectToAction("Index", "Invoice");
            }
            catch (RenewTokenException e)
            {
                Console.WriteLine(e);
                return RedirectToAction("Connect", "Home");
            }
        }

        public bool AddApiAuditLog(Invoice model)
        {
            SqlConnection con = new SqlConnection("Data Source=WINCTRL-IURSOMN;Initial Catalog=XeroApiDatabase;Integrated Security=True");

            SqlCommand cmd = new SqlCommand("AddApiAuditLog", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Name", model.Contact.Name);
            cmd.Parameters.AddWithValue("@InvoiceType", model.Type);
            cmd.Parameters.AddWithValue("@LineAmountTypes", model.LineAmountTypes);
            cmd.Parameters.AddWithValue("@CreateDate", model.Date);
            cmd.Parameters.AddWithValue("@DueDate", model.DueDate);


            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();

            if (i == -1)
                return true;
            else
                return false;
        }

    }
}