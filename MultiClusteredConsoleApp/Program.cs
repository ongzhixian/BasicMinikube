using k8s;
using k8s.KubeConfigModels;

KubernetesClientConfiguration config;
K8SConfiguration k8SConfiguration = new K8SConfiguration();

if (Environment.GetEnvironmentVariables().Contains("KUBERNETES_SERVICE_HOST"))
    config = KubernetesClientConfiguration.InClusterConfig();
else
{
    config = KubernetesClientConfiguration.BuildConfigFromConfigFile();

    // Use my packaged kubeconfig.yaml
    config = KubernetesClientConfiguration.BuildConfigFromConfigFile("kubeconfig.yaml");
    //KubernetesClientConfiguration.KubeConfigDefaultLocation
    //config = KubernetesClientConfiguration.BuildDefaultConfig();


    //K8SConfiguration k8SConfiguration = KubernetesClientConfiguration.LoadKubeConfig("kubeconfig.yaml");
    k8SConfiguration = KubernetesClientConfiguration.LoadKubeConfig(KubernetesClientConfiguration.KubeConfigDefaultLocation);

    //KubernetesClientConfiguration.BuildConfigFromConfigObject(mas)
    k8SConfiguration = new K8SConfiguration
    {
        Clusters = new List<Cluster> 
        { 
            new Cluster
            {
                Name = "minikube",
                ClusterEndpoint = new ClusterEndpoint
                {
                    Server = "https://172.25.113.128:8443",
                }
            },
            new Cluster
            {
                Name = "contoso",
                ClusterEndpoint = new ClusterEndpoint
                {
                    Server = "https://172.25.125.214:8443",
                }
            }
        },
        Contexts = new List<Context>
        {
            new Context
            {
                Name = "minikube",
                ContextDetails = new ContextDetails
                {
                    Namespace = "default",
                    User = "minikube", 
                    Cluster = "minikube"
                }
            },
            new Context
            {
                Name = "contoso",
                ContextDetails = new ContextDetails
                {
                    Namespace = "default",
                    User = "contoso",
                    Cluster = "contoso"
                }
            },
        },
        Users = new List<User>
        {
            new User
            {
                Name="minikube",
                
            },
            new User
            {
                Name="contoso"
            }
        }
    };

    //config = KubernetesClientConfiguration.BuildConfigFromConfigObject(k8SConfiguration, currentContext: "contoso");

    //config = KubernetesClientConfiguration.BuildConfigFromConfigObject(k8SConfiguration, currentContext: "contoso");

    // get K8SConfiguration from KubernetesClientConfiguration
}

//KubernetesYaml.Serialize
//var c = KubernetesClientConfiguration.BuildDefaultConfig();

config = KubernetesClientConfiguration.BuildConfigFromConfigObject(k8SConfiguration, currentContext: "contoso");
Console.WriteLine($"Current context {config.CurrentContext}");

IKubernetes client = new Kubernetes(config);

var list = client.CoreV1.ListNamespacedPod("default");
foreach (var item in list.Items)
{
    Console.WriteLine(item.Metadata.Name);
}

if (list.Items.Count == 0)
{
    Console.WriteLine("Empty!");
}
