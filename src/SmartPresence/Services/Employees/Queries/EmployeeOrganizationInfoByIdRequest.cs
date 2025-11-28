namespace SmartPresence.Services.Employees.Queries
{
    public class EmployeeOrganizationInfoByIdRequest
    {
        public int Id { get; set; }

        public EmployeeOrganizationInfoByIdRequest(int id)
        {
            this.Id = id;
        }
    }
}
