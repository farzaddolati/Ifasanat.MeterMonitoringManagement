namespace Ifasanat.MeterMonitoringManagement.Dto
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public int? CityId { get; set; }
        public string CityName { get; set; }
    }
}
