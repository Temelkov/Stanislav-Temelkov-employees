using Models.Models;
using ServiceLayer.Interfaces;
using System.Globalization;

namespace ServiceLayer.Services
{
    public class EmployeeProjectService : IEmployeeProjectService
    {
        private readonly string[] dateFormats = { "yyyy-MM-dd", "MM/dd/yyyy", "dd/MM/yyyy" };

        public async Task<List<EmployeePairResult>> GetLongestWorkingPairsAsync(List<EmployeeProject> projects)
        {
            if (projects == null || projects.Count == 0)
            {
                return new List<EmployeePairResult>();
            }

            return await Task.Run(() =>
            {
                var pairs = new Dictionary<(int, int, int), int>();

                foreach (var project in projects)
                {
                    foreach (var otherProject in projects)
                    {
                        if (project.ProjectID == otherProject.ProjectID && project.EmpID != otherProject.EmpID)
                        {
                            var pair = project.EmpID < otherProject.EmpID
                                ? (project.EmpID, otherProject.EmpID, project.ProjectID)
                                : (otherProject.EmpID, project.EmpID, project.ProjectID);

                            var commonDays = GetCommonDays(project, otherProject);

                            if (!pairs.ContainsKey(pair))
                                pairs[pair] = 0;

                            pairs[pair] += commonDays;
                        }
                    }
                }

                var results = pairs.Select(p => new EmployeePairResult
                {
                    EmpID1 = p.Key.Item1,
                    EmpID2 = p.Key.Item2,
                    ProjectID = p.Key.Item3,
                    DaysWorked = p.Value
                }).OrderByDescending(p => p.DaysWorked).ToList();

                return results;
            });
        }

        private int GetCommonDays(EmployeeProject project1, EmployeeProject project2)
        {
            var start = project1.DateFrom > project2.DateFrom ? project1.DateFrom : project2.DateFrom;
            var end1 = project1.DateTo ?? DateTime.Now;
            var end2 = project2.DateTo ?? DateTime.Now;
            var end = end1 < end2 ? end1 : end2;

            return (end - start).Days > 0 ? (end - start).Days : 0;
        }

        public async Task<List<EmployeeProject>> ParseProjectsAsync(string csvContent)
        {
            var projects = new List<EmployeeProject>();

            using (var reader = new StringReader(csvContent))
            {
                bool isFirstLine = true;
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (isFirstLine)
                    {
                        isFirstLine = false;
                        continue;
                    }

                    try
                    {
                        var values = line.Split(',');

                        var project = new EmployeeProject
                        {
                            EmpID = int.Parse(values[0]),
                            ProjectID = int.Parse(values[1]),
                            DateFrom = DateTime.ParseExact(values[2], dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None),
                            DateTo = string.IsNullOrEmpty(values[3]) ? (DateTime?)null : DateTime.ParseExact(values[3], dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None)
                        };

                        projects.Add(project);
                    }
                    catch (FormatException ex)
                    {
                        Console.WriteLine($"Error parsing line: {line}. Error: {ex.Message}");
                    }
                }
            }

            return projects;
        }
    }
}
