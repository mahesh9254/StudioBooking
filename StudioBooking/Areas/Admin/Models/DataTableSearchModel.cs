namespace StudioBooking.Areas.Admin.Models
{
    public class DataTableSearchModel
    {
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public SearchData search { get; set; }
        public List<Columns> columns { get; set; }
        public List<Order> order { get; set; }

        public DataTableSearchModel()
        {
            search = new SearchData();
            order = new List<Order>();
            columns = new List<Columns>();
        }
    }

    public class Columns
    {
        public string data { get; set; }
        public string name { get; set; }
        public bool searchable { get; set; }
        public bool orderable { get; set; }
        public SearchData search { get; set; }
    }

    public class SearchData
    {
        public string value { get; set; }
        public bool regex { get; set; }
    }

    public class Order
    {
        public int column { get; set; }
        public string dir { get; set; }
    }
}
