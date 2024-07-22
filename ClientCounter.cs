using System.Numerics;

public class ClientCounter
{
    private static String NUM_OF_WORKFLOW_EXECUTIONS = "numOfWorkflowExec";
    private static String NUM_OF_SIGNALS = "numOfSignals";
    private static String NUM_OF_QUERIES = "numOfQueries";
    private static Dictionary<String, Dictionary<String, BigInteger?>> perWorkflowIdDictionary = 
        new Dictionary<String, Dictionary<String, BigInteger?>>();

    public static String Info() 
    {  
        String result = "";
        foreach(var item in perWorkflowIdDictionary)
        {
            var info = item.Value;
            result = result + 
                "\n** Workflow ID: " + item.Key +
                "\n\tTotal Number of Workflow Exec: " + info[NUM_OF_WORKFLOW_EXECUTIONS] +
                "\n\tTotal Number of Signals: " + info[NUM_OF_SIGNALS] +
                "\n\tTotal Number of Queries: " + info[NUM_OF_QUERIES];
        }

        return result;
    }    

    private void Add(String workflowId, String type) 
    {
        if (!perWorkflowIdDictionary.ContainsKey(workflowId)) 
        {
            perWorkflowIdDictionary.Add(workflowId, getDefaultInfoMap());
        }

        if (perWorkflowIdDictionary[workflowId][type] == null) 
        {
            perWorkflowIdDictionary[workflowId][type] = 1;
        } 
        else 
        {
            var current = perWorkflowIdDictionary[workflowId][type];
            var next = current + 1;
            perWorkflowIdDictionary[workflowId][type] = next;
        }
    }

    public BigInteger? NumOfWorkflowExecutions(String workflowId) 
    {
        return perWorkflowIdDictionary[workflowId][NUM_OF_WORKFLOW_EXECUTIONS];
    }

    public BigInteger? NumOfSignals(String workflowId) 
    {
        return perWorkflowIdDictionary[workflowId][NUM_OF_SIGNALS];
    }

    public BigInteger? NumOfQueries(String workflowId)
    {
        return perWorkflowIdDictionary[workflowId][NUM_OF_QUERIES];
    }

    // Creates a default counter info map for a workflowid
    private static Dictionary<String, BigInteger?> getDefaultInfoMap() {
        return new Dictionary<String, BigInteger?>()
        {
            { NUM_OF_WORKFLOW_EXECUTIONS, 0 },
            { NUM_OF_SIGNALS, 0 },
            { NUM_OF_QUERIES, 0}
        };
    }

    public void AddStartInvocation(String workflowId)
    {
        Add(workflowId, NUM_OF_WORKFLOW_EXECUTIONS);
    }

    public void AddSignalInvocation(String workflowId)
    {
        Add(workflowId, NUM_OF_SIGNALS);
    }

    public void AddQueryInvocation(String workflowId)
    {
        Add(workflowId, NUM_OF_QUERIES);
    }
}