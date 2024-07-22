
using Temporalio.Client;
using Temporalio.Worker;

internal class Program
{
    private static async Task Main(string[] args)
    {
        static string getEnvVarWithDefault(string envName, string defaultValue)
        {
            string? value = Environment.GetEnvironmentVariable(envName);
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }
            return value;
        }

        var address = getEnvVarWithDefault("TEMPORAL_ADDRESS", "127.0.0.1:7233");
        var temporalNamespace = getEnvVarWithDefault("TEMPORAL_NAMESPACE", "default");
        var tlsCertPath = getEnvVarWithDefault("TEMPORAL_TLS_CERT", "");
        var tlsKeyPath = getEnvVarWithDefault("TEMPORAL_TLS_KEY", "");
        TlsOptions? tls = null;
        if (!string.IsNullOrEmpty(tlsCertPath) && !string.IsNullOrEmpty(tlsKeyPath))
        {
            tls = new()
            {
                ClientCert = await File.ReadAllBytesAsync(tlsCertPath),
                ClientPrivateKey = await File.ReadAllBytesAsync(tlsKeyPath),
            };
        }

        var client = await TemporalClient.ConnectAsync(
            options: new(address)
            {
                Namespace = temporalNamespace,
                Tls = tls,
                Interceptors = new[] {
            new SimpleClientCallsInterceptor()
                }
            });

        using var tokenSource = new CancellationTokenSource();
        Console.CancelKeyPress += (_, eventArgs) =>
        {
            tokenSource.Cancel();
            eventArgs.Cancel = true;
        };

        var activities = new MyActivities();

        var workerOptions = new TemporalWorkerOptions(Constants.TASK_QUEUE).
                AddAllActivities(activities).
                AddWorkflow<MyWorkflow>().
                AddWorkflow<MyChildWorkflow>();

        workerOptions.Interceptors = new[] { new SimpleCounterWorkerInterceptor() };

        using var worker = new TemporalWorker(
            client,
            workerOptions);

        // Run worker until cancelled
        Console.WriteLine("Running worker...");
        try
        {
            // Start the workers
            var workerResult = worker.ExecuteAsync(tokenSource.Token);

            // start the workflow
            var handle = await client.StartWorkflowAsync((MyWorkflow wf) => wf.Exec(),
                new(id: Guid.NewGuid().ToString(), taskQueue: Constants.TASK_QUEUE));

            Console.WriteLine("Sending name and title to workflow");
            await handle.SignalAsync(wf => wf.SignalNameAndTitle("John", "Customer"));

            var name = await handle.QueryAsync(wf => wf.QueryName());
            var title = await handle.QueryAsync(wf => wf.QueryTitle());

            // send exit signal to workflow
            await handle.SignalAsync(wf => wf.Exit());

            var result = await handle.GetResultAsync();

            Console.WriteLine($"Workflow result is {result}", result);

            Console.WriteLine("Query results: ");
            Console.WriteLine("Name: " + name);
            Console.WriteLine("Title: " + title);

            // print worker counter info
            Console.WriteLine("Collected Worker Counter Info: ");
            Console.WriteLine(WorkerCounter.Info());

            // print client counter info
            Console.WriteLine();
            Console.WriteLine("Collected Client Counter Info:");
            Console.WriteLine(ClientCounter.Info());
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Worker cancelled");
        }
    }
}