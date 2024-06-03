using Models.Models;

namespace ServiceLayer.Interfaces
{
    public interface IEmployeeProjectService
    {
        Task<List<EmployeePairResult>> GetLongestWorkingPairsAsync(List<EmployeeProject> projects);

        Task<List<EmployeeProject>> ParseProjectsAsync(string csvContent);
    }
}
