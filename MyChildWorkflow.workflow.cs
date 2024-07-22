using Temporalio.Workflows;

[Workflow]
public class MyChildWorkflow 
{
    readonly ActivityOptions activityOptions = new () {
        StartToCloseTimeout = TimeSpan.FromSeconds(10),
    };

    [WorkflowRun]
    public async Task<String> ExecChild(String name, String title)
    {
        String result = await Workflow.ExecuteActivityAsync((MyActivities act) => act.SayHello(name, title), activityOptions);
        result = result + await Workflow.ExecuteActivityAsync((MyActivities act) => act.SayGoodBye(name, title), activityOptions);  

        return result;
    }
}