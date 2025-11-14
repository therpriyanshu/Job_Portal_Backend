using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Route("api/jobs")]
[ApiController]
public class JobController : ControllerBase
{
    private readonly IJobService _jobService;

    public JobController(IJobService jobService)
    {
        _jobService = jobService;
    }

    // Get All Jobs (Public API)
    [HttpGet]
    public async Task<IActionResult> GetJobs()
    {
        return Ok(await _jobService.GetJobs());
    }

    // Get Job By ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetJobById(Guid id)
    {
        var job = await _jobService.GetJobById(id);
        if (job == null)
            return NotFound("Job not found");

        return Ok(job);
    }

    // Add a new job (Public - No Authorization)
    [HttpPost]
    public async Task<IActionResult> AddJob([FromBody] Job job)
    {
        if (job == null)
            return BadRequest("Invalid job data.");

        var createdJob = await _jobService.AddJob(job);
        return CreatedAtAction(nameof(GetJobById), new { id = createdJob.Id }, createdJob);
    }

    // Model for Job Application Request
    public class ApplyJobRequest
    {
        public string Email { get; set; }
    }

    // Apply for a job using email
    [HttpPost("{jobId}/apply")]
    public async Task<IActionResult> ApplyForJob(Guid jobId, [FromBody] ApplyJobRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.Email))
            return BadRequest(new { message = "Email is required" });

        var result = await _jobService.ApplyForJob(jobId, request.Email);
        if (!result) return BadRequest("Could not apply for the job. Either job does not exist, user is not found, or already applied.");

        return Ok(new { message = "Applied successfully." });
    }

    // Get all jobs applied by a user using email
    [HttpGet("applied")]
    public async Task<IActionResult> GetJobsAppliedByEmail([FromQuery] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return BadRequest("Email is required.");

        var appliedJobs = await _jobService.GetAppliedJobsByEmail(email);
        if (appliedJobs == null || !appliedJobs.Any())
            return NotFound("No jobs found for this user.");

        return Ok(appliedJobs);
    }

    

    
}
