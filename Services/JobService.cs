using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class JobService : IJobService
{
    private readonly AppDbContext _context;

    public JobService(AppDbContext context)
    {
        _context = context;
    }

    // Get all jobs
    public async Task<IEnumerable<Job>> GetJobs()
    {
        return await _context.Jobs.ToListAsync();
    }

    // Get job by ID
    public async Task<Job?> GetJobById(Guid id)
    {
        return await _context.Jobs.FindAsync(id);
    }

    // Add a new job
    public async Task<Job> AddJob(Job job)
    {
        job.Id = Guid.NewGuid();
        _context.Jobs.Add(job);
        await _context.SaveChangesAsync();
        return job;
    }

    // Apply for a job using email
    public async Task<bool> ApplyForJob(Guid jobId, string userEmail)
    {
        var job = await _context.Jobs.FindAsync(jobId);
        if (job == null)
            return false;

        var application = new JobApplication
        {
            Id = Guid.NewGuid(),
            JobId = jobId,
            UserEmail = userEmail,
            AppliedDate = DateTime.UtcNow
        };

        _context.JobApplications.Add(application);
        await _context.SaveChangesAsync();
        return true;
    }

    // Get applied jobs by user email
    public async Task<IEnumerable<Job>> GetAppliedJobsByEmail(string email)
    {
        var jobIds = await _context.JobApplications
            .Where(a => a.UserEmail == email)
            .Select(a => a.JobId)
            .ToListAsync();

        return await _context.Jobs
            .Where(j => jobIds.Contains(j.Id))
            .ToListAsync();
    }
    public bool CanConnectToDatabase()
    {
        try
        {
            return _context.Database.CanConnect();
        }
        catch
        {
            return false;
        }
    }
}
