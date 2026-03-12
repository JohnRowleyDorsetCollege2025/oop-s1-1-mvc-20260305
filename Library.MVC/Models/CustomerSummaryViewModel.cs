namespace Library.MVC.Models
{
    public class CustomerSummaryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int InvoiceCount { get; set; }
    }


    public class CustomerInvoicesViewModel
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = "";
        public List<CustomerInvoiceRowViewModel> Invoices { get; set; } = [];
    }

    public class CustomerInvoiceRowViewModel
    {
        public int InvoiceId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int LineCount { get; set; }
        public decimal Total { get; set; }
    }

    // ── Customer → Invoice detail page ─────────────────────────────────────────

    public class CustomerInvoiceDetailViewModel
    {
        // Customer section
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = "";

        // Invoice section
        public int InvoiceId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public List<InvoiceLineDetailsModel> Lines { get; set; } = [];
        public decimal Total => Lines.Sum(l => l.LineTotal);
    }
    public class InvoiceLineDetailsModel
    {
        public string ProductName { get; set; } = "";
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal => Quantity * UnitPrice;
    }

}
