using Temporalio.Workflows;

public record SimpleInput(String val);
public record SimpleOutput(String result);

[Workflow]
public class MyWorkflow {

    private String name = "";
    private String title = "";
    private Boolean exit = false;

    [WorkflowRun]
    public async Task<String> Exec()
    {
        // wait for greeting info
        await Workflow.WaitConditionAsync(() => name != null && title != null);

        // Execute Child Workflow
        String result = await Workflow.ExecuteChildWorkflowAsync((MyChildWorkflow wf) => wf.ExecChild(name,title),
            new()
            {
                Id = Constants.CHILD_WF_ID,
            });

        // Wait for exit signal
        await Workflow.WaitConditionAsync(() => exit != false);

        return result;
    }

    [WorkflowSignal]
    public async Task SignalNameAndTitle(String name, String title) 
    {
        this.name = name;
        this.title = title;
    }

    [WorkflowQuery]
    public String QueryName()
    {
        return name;
    }

    [WorkflowQuery]
    public String QueryTitle() 
    {
        return title;
    }

    [WorkflowSignal]
    public async Task Exit() 
    {
        this.exit = true;
    }
}