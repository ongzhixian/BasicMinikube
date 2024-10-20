using k8s;
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

while (true)
{
    // Scenarios
    // 1.   Lease does not exists               -- create lease and be leader
    // 2.   Lease exists but has expired        -- declare leader
    // 3a.  Lease exists and has not expired    -- if is  holder, renew lease
    // 3b.  Lease exists and has not expired    -- if not holder, just cry
    try
    {
        var lease = await GetLeaseAsync(leaseName, namespaceName);
        if (lease == null)
        {
            lease = new V1Lease
            {
                Metadata = new V1ObjectMeta { Name = leaseName, NamespaceProperty = namespaceName },
                Spec = new V1LeaseSpec
                {
                    HolderIdentity = identity,
                    LeaseDurationSeconds = (int)leaseDuration.TotalSeconds,
                    RenewTime = DateTime.UtcNow,
                    AcquireTime = DateTime.UtcNow,
                }
            };
            await client.CoordinationV1.CreateNamespacedLeaseAsync(lease, namespaceName);
            leaderWins++;
            Console.WriteLine($"Created new lease and acquired leadership ({leaderWins})");
        }
        else if (leaseHasExpired(lease))
        {
            lease.Spec.HolderIdentity = identity;
            lease.Spec.RenewTime = DateTime.UtcNow;
            lease.Spec.AcquireTime = DateTime.UtcNow;
            await client.CoordinationV1.ReplaceNamespacedLeaseAsync(lease, leaseName, namespaceName);
            leaderWins++;
            Console.WriteLine($"{machineName} Acquired leadership ({leaderWins})");
        }
        else if (lease.Spec.HolderIdentity == identity)
        {
            Console.WriteLine($"{machineName} Already the leader ({leaderWins}), renewing lease");
            lease.Spec.RenewTime = DateTime.UtcNow;
            await client.CoordinationV1.ReplaceNamespacedLeaseAsync(lease, leaseName, namespaceName);
        }
        else
        {
            Console.WriteLine($"Already has leader ({lease.Spec.HolderIdentity}) -- sobs. ");
            //lease.Spec.RenewTime = DateTime.UtcNow;
            //await client.CoordinationV1.ReplaceNamespacedLeaseAsync(lease, leaseName, namespaceName);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }

    await Task.Delay(retryPeriod);
}

async Task<V1Lease?> GetLeaseAsync(string leaseName, string namespaceName)
{
    var leaseList = await client.CoordinationV1.ListNamespacedLeaseAsync(namespaceName);

    return leaseList.Items.FirstOrDefault(item => item.Metadata.Name == leaseName);

    //if (leaseList.Items.Any(item => item.Metadata.Name == leaseName))
    //    return await client.CoordinationV1.ReadNamespacedLeaseAsync(leaseName, namespaceName);

    //// Note:If we try to ReadNamespacedLeaseAsync for a non-existing lease, it will throw an exception.
    ////      So instead, we get least and check there
    ////      If we are fully confident, we can change the code to use `ReadNamespacedLeaseAsync`

    //return null;
}

bool leaseHasExpired(V1Lease lease)
{
    if (lease.Spec.RenewTime.HasValue && lease.Spec.RenewTime.Value.AddSeconds(lease.Spec.LeaseDurationSeconds.GetValueOrDefault()) < DateTime.UtcNow)
    {
        return true;
    }

    return false;
}