namespace insure_fixlast.Models
{
    public class Claim
    {
        public int Id { get; set; }
        public string Form { get; set; }
        public string Hospital { get; set; }
        public Decimal amount { get; set; }
        public Status status { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
    public enum Status
    {
        Success,Await,Fail
    }
}
