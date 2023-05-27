namespace Ifasanat.MeterMonitoringManagement.Domain
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Customer> Customers { get; set; }
    }
}
