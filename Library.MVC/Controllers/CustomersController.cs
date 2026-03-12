using Library.Domain;
using Library.MVC.Data;
using Library.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.MVC.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Customers
        public async Task<IActionResult> Index()
        {
            var customers = await _context.Customers
                .OrderBy(c => c.Name)
                .Select(c => new CustomerSummaryViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    InvoiceCount = c.Invoices.Count
                })
                .ToListAsync();

            return View(customers);
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Customer customer)
        {
            if (id != customer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
        // GET: Customers/InvoiceDetail/5  — detail of one invoice, with customer context
        public async Task<IActionResult> InvoiceDetail(int? id)
        {
            if (id is null) return NotFound();

            var invoice = await _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.Lines)
                    .ThenInclude(l => l.Product)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice is null) return NotFound();

            var vm = new CustomerInvoiceDetailViewModel
            {
                CustomerId = invoice.Customer!.Id,
                CustomerName = invoice.Customer.Name,
                InvoiceId = invoice.Id,
                InvoiceDate = invoice.InvoiceDate,
                Lines = invoice.Lines.Select(l => new InvoiceLineDetailsModel
                {
                    ProductName = l.Product!.Name,
                    Quantity = l.Quantity,
                    UnitPrice = l.UnitPrice
                }).ToList()
            };

            return View(vm);
        }
        // GET: Customers/Invoices/5  — lists all invoices for a customer
        public async Task<IActionResult> Invoices(int? id)
        {
            if (id is null) return NotFound();

            var customer = await _context.Customers
                .Include(c => c.Invoices)
                    .ThenInclude(i => i.Lines)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer is null) return NotFound();

            var vm = new CustomerInvoicesViewModel
            {
                CustomerId = customer.Id,
                CustomerName = customer.Name,
                Invoices = customer.Invoices
                    .OrderByDescending(i => i.InvoiceDate)
                    .Select(i => new CustomerInvoiceRowViewModel
                    {
                        InvoiceId = i.Id,
                        InvoiceDate = i.InvoiceDate,
                        LineCount = i.Lines.Count,
                        Total = i.Lines.Sum(l => l.Quantity * l.UnitPrice)
                    })
                    .ToList()
            };

            return View(vm);
        }
    }
}
