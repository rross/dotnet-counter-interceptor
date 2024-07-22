using System.Numerics;

public class WorkerCounter 
{
    private static Dictionary<String, Dictionary<String, BigInteger?>> perWorkflowIdDictionary = 
        new Dictionary<String, Dictionary<String, BigInteger?>>();
    
    public static String NUM_OF_WORKFLOW_EXECUTIONS = "numOfWorkflowExec";
    public static String NUM_OF_CHILD_WORKFLOW_EXECUTIONS = "numOfChildWorkflowExec";
    public static String NUM_OF_ACTIVITY_EXECUTIONS = "numOfActivityExec";
    public static String NUM_OF_SIGNALS = "numOfSignals";
    public static String NUM_OF_QUERIES = "numOfQueries";

    public static void Add(String workflowId, String type) 
    {
        if (!perWorkflowIdDictionary.ContainsKey(workflowId))
        {
            perWorkflowIdDictionary.Add(workflowId, DefaultInfoMap());
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

    public static BigInteger? NumOfWorkflowExecutions(String workflowId) 
    {
        return perWorkflowIdDictionary[workflowId][NUM_OF_WORKFLOW_EXECUTIONS];
    }

    public static BigInteger? NumOfChildWorkflowExecutions(String workflowId) 
    {
        return perWorkflowIdDictionary[workflowId][NUM_OF_CHILD_WORKFLOW_EXECUTIONS];
    }

    public static BigInteger? NumOfActivityExecutions(String workflowId) 
    {
        return perWorkflowIdDictionary[workflowId][NUM_OF_ACTIVITY_EXECUTIONS];
    }

    public static BigInteger? NumOfSignals(String workflowId) 
    {
        return perWorkflowIdDictionary[workflowId][NUM_OF_SIGNALS];
    }

    public static BigInteger? NumOfQueries(String workflowId) 
    {
        return perWorkflowIdDictionary[workflowId][NUM_OF_QUERIES];
    }

    public static String Info() 
    {  
        String result = "";
        foreach(var item in perWorkflowIdDictionary)
        {
            var info = item.Value;
            result = result + 
                "\n** Workflow ID: " + item.Key +
                "\n\tTotal Number of Workflow Exec: " + info[NUM_OF_WORKFLOW_EXECUTIONS] +
                "\n\tTotal Number of Child Worflow Exec: " + info[NUM_OF_CHILD_WORKFLOW_EXECUTIONS] +
                "\n\tTotal Number of Activity Exec: " + info[NUM_OF_ACTIVITY_EXECUTIONS] +
                "\n\tTotal Number of Signals: " + info[NUM_OF_SIGNALS] +
                "\n\tTotal Number of Queries: " + info[NUM_OF_QUERIES];
        }

        return result;
    }

    
    // Creates a default counter info map for a workflowid
  private static Dictionary<String, BigInteger?> DefaultInfoMap() {
    return new Dictionary<String, BigInteger?>()
    {
        { NUM_OF_WORKFLOW_EXECUTIONS, 0 },
        { NUM_OF_CHILD_WORKFLOW_EXECUTIONS, 0 },
        { NUM_OF_ACTIVITY_EXECUTIONS, 0 },
        { NUM_OF_SIGNALS, 0},
        { NUM_OF_QUERIES, 0}
    };
  }
}