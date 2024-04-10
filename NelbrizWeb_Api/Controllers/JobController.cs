using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NelbrizWeb_Api.Jobs;

namespace NelbrizWeb_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {

        [HttpPost]
        [Route("CreateBackgroundJob")]
        public async Task<IActionResult> CreateBackgroundJob()
        {

            //BackgroundJob.Enqueue(() => Console.WriteLine("Background Job Triggered"));
            BackgroundJob.Enqueue<SampleJob>(x => x.WriteLog("Background Job Triggered"));
            return Ok();
        }


        [HttpPost]
        [Route("CreateScheduledJob")]
        public async Task<IActionResult> CreateScheduledJob()
        {
            var scheduleDateTime = DateTime.UtcNow.AddSeconds(5);
            var dateTimeOffset = new DateTimeOffset(scheduleDateTime);

           //BackgroundJob.Schedule(() => Console.WriteLine("Background Job Triggered"), dateTimeOffset);
            BackgroundJob.Schedule<SampleJob>(x => x.WriteLog("Background Job Triggered"), dateTimeOffset);
            return Ok();
        }




        [HttpPost]
        [Route("CreateContinuationJob")]
        public async Task<IActionResult> CreateContinuationJob()
        {
            var scheduleDateTime = DateTime.UtcNow.AddSeconds(5);
            var dateTimeOffset = new DateTimeOffset(scheduleDateTime);
            var jobId = BackgroundJob.Schedule<SampleJob>(x => x.WriteLog("Background Job Triggered"), dateTimeOffset);

            var jobId2 =  BackgroundJob.ContinueJobWith<SampleJob>(jobId, x => x.WriteLog("Background Job Triggered"));
            var jobId3 =  BackgroundJob.ContinueJobWith<SampleJob>(jobId2, x => x.WriteLog("Background Job 2 Triggered"));
            var jobId4 =  BackgroundJob.ContinueJobWith<SampleJob>(jobId3, x => x.WriteLog("Background Job 2 Triggered"));

            return Ok();
        }




        [HttpPost]
        [Route("CreateRecurringJob")]
        public async Task<IActionResult> CreateRecurringJob()
        {
            //running jobs at spoecific time inyervals! e.g like every 30mins e.t.c.
            RecurringJob.AddOrUpdate<SampleJob>( "RecurringJob1", x => x.WriteLog("Background Job Triggered"), "* * * * *");
            return Ok();
        }

    }
}
