﻿using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Elsa.Handlers
{
    public class LogWorkflowExecution : INotificationHandler<ActivityExecuting>, INotificationHandler<ActivityExecuted>, INotificationHandler<WorkflowExecutionBurstCompleted>
    {
        private readonly ILogger _logger;
        private readonly Stopwatch _stopwatch = new();

        public LogWorkflowExecution(ILogger<LogWorkflowExecution> logger)
        {
            _logger = logger;
        }
        
        public Task Handle(ActivityExecuting notification, CancellationToken cancellationToken)
        {
            _stopwatch.Start();
            var activityBlueprint = notification.Activity;
            var activityId = activityBlueprint.Id;
            var workflowInstanceId = notification.WorkflowExecutionContext.WorkflowInstance.Id;
            _logger.LogDebug("Executing activity {ActivityType} {ActivityId} for workflow {workflowInstanceId}", activityBlueprint.Type, activityId, workflowInstanceId);
            return Task.CompletedTask;
        }

        public Task Handle(ActivityExecuted notification, CancellationToken cancellationToken)
        {
            _stopwatch.Stop();
            var activityBlueprint = notification.Activity;
            var activityId = activityBlueprint.Id;
            var workflowInstanceId = notification.WorkflowExecutionContext.WorkflowInstance.Id;
            _logger.LogDebug("Executed activity {ActivityType} {ActivityId} for workflow {workflowInstanceId} in {ElapsedTime}", activityBlueprint.Type, activityId, workflowInstanceId, _stopwatch.Elapsed);
            return Task.CompletedTask;
        }

        public Task Handle(WorkflowExecutionBurstCompleted notification, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Burst of workflow execution completed for workflow {WorkflowInstanceId}", notification.WorkflowExecutionContext.WorkflowInstance.Id);
            return Task.CompletedTask;
        }
    }
}