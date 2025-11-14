using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IJobService
{
    Task<IEnumerable<Job>> GetJobs();
    Task<Job?> GetJobById(Guid jobId);
    Task<Job> AddJob(Job job);
    Task<bool> ApplyForJob(Guid jobId, string email);
    Task<IEnumerable<Job>> GetAppliedJobsByEmail(string email);
    bool CanConnectToDatabase();

}

