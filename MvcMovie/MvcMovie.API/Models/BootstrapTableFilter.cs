namespace MvcMovie.API.Models
{
    public class BootstrapModel
    {
        public BootstrapModel() { }

        public int Offset { get; set; }
        public int? Limit { get; set; }
        public IList<BootstrapTableFilter> Search { get; set; }
    }

    public class BootstrapTableFilter
    {
        public BootstrapTableFilter() { }

        public string Name { get; set; }
        public string Value { get; set; }
    }
}
