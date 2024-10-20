using k8s;
using k8s.LeaderElection.ResourceLock;
using k8s.LeaderElection;
using k8s.Models;

KubernetesClientConfiguration config;

if (Environment.GetEnvironmentVariables().Contains("KUBERNETES_SERVICE_HOST"))
    config = KubernetesClientConfiguration.InClusterConfig();
else
    config = KubernetesClientConfiguration.BuildConfigFromConfigFile();

IKubernetes client = new Kubernetes(config);


var leaseName = "example-lease2";
var namespaceName = "default";
var identity = Guid.NewGuid().ToString();
var leaseDuration = TimeSpan.FromSeconds(15);
var renewDeadline = TimeSpan.FromSeconds(10);
var retryPeriod = TimeSpan.FromSeconds(2);
var machineName = Environment.MachineName;
identity = machineName;
var leaderWins = 0;

var leaderLock = new LeaseLock(client, namespaceName, leaseName, identity);

var leaderElectionConfig = new LeaderElectionConfig(leaderLock)
{
    LeaseDuration = TimeSpan.FromSeconds(15),
    RenewDeadline = TimeSpan.FromSeconds(10),
    RetryPeriod = TimeSpan.FromSeconds(2)
};
var leaderElector = new LeaderElector(leaderElectionConfig);
leaderElector.OnStartedLeading += () => Console.WriteLine("Got Leader Lock");
leaderElector.OnNewLeader += leader => Console.WriteLine("Leader changed to: {leader}", leader);
leaderElector.OnStoppedLeading += () => Console.WriteLine("Lost Leader Lock");


while (true)
{
    // Scenarios
    // 1.   Lease does not exists               -- create lease and be leader
    // 2.   Lease exists but has expired        -- declare leader
    // 3a.  Lease exists and has not expired    -- if is  holder, renew lease
    // 3b.  Lease exists and has not expired    -- if not holder, just cry
    await leaderElector.RunUntilLeadershipLostAsync();

    await Task.Delay(retryPeriod);
}
