using System.Diagnostics;
using Temporalio.Activities;

public class MyActivities
{
    [Activity]
    public String SayHello(String name, String title) 
    {
        return "Hello " + title + " " + name;
    }

    [Activity]
    public String SayGoodBye(String name, String title)
    {
        return "Goodbye " + title + " " + name;
    }
}
