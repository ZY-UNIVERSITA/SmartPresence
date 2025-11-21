namespace SmartPresence.Services.Employees.Queries
{
    public class EmployeeByIdRequest
    {
        public int Id { get; set; }

        public EmployeeByIdRequest(int id)
        {
            this.Id = id;
        }
    }
}
