# After restart of MiniKube...

minikube start --cpus='2' --memory='4096MB' --disk-size='20480mb'

Knative needs more than 2 CPU? Quickstart uses 3 CPU so lets go with 3
minikube start --cpus='3' --memory='4096MB' --disk-size='20480mb'

minikube addons enable registry
minikube addons enable metrics-server
minikube addons enable dashboard
minikube addons enable ingress
minikube addons enable ingress-dns


minikube tunnel --profile knative

minikube mount C:\data\basic-minikube:/data/basic-minikube
--This is a blocking execution ; so maybe place this elsewhere

-- Instead of this can we leverage of Knative-Serving sslip.io magic DNS?

Get-DnsClientNrptRule | Where-Object { $_.Namespace -eq ".internal" } | Foreach-Object { Remove-DnsClientNrptRule -Name $_.Name -Force }
Add-DnsClientNrptRule -Namespace ".internal" -NameServers "$(minikube ip)"


KNATIVE

SERVING
kubectl apply -f https://github.com/knative/serving/releases/download/knative-v1.14.1/serving-crds.yaml
kubectl apply -f https://github.com/knative/serving/releases/download/knative-v1.14.1/serving-core.yaml

KOURIER -- NEEDS Minikube tunnel (aka Load Balancer)
kubectl apply -f https://github.com/knative/net-kourier/releases/download/knative-v1.14.0/kourier.yaml
kubectl patch configmap/config-network --namespace knative-serving --type merge --patch '{"data":{"ingress-class":"kourier.ingress.networking.knative.dev"}}'
kubectl --namespace kourier-system get service kourier

"Magic" DNS -- Defines a Kubernetes Job called default-domain that configures Knative Serving to use 'sslip.io' as the default DNS suffix 😲
kubectl apply -f https://github.com/knative/serving/releases/download/knative-v1.14.1/serving-default-domain.yaml

ENABLE SERVING HPA -- use Kubernetes Horizontal Pod Autoscaler (HPA) to drive autoscaling decisions
kubectl apply -f https://github.com/knative/serving/releases/download/knative-v1.14.1/serving-hpa.yaml

VET SERVING + KOURIER
kubectl get pods -n knative-serving
kubectl api-resources --api-group='serving.knative.dev'
kubectl get service kourier --namespace kourier-system 


EVENTING

kubectl apply -f https://github.com/knative/eventing/releases/download/knative-v1.14.2/eventing-crds.yaml
kubectl apply -f https://github.com/knative/eventing/releases/download/knative-v1.14.2/eventing-core.yaml

kubectl apply -f https://github.com/knative/eventing/releases/download/knative-v1.14.2/in-memory-channel.yaml
kubectl apply -f https://github.com/knative/eventing/releases/download/knative-v1.14.2/mt-channel-broker.yaml

(OPTIONAL? unless we want to configure broker?)
kubectl apply -f .\k8s\knative-broker.configmap.yaml

kubectl get pods -n knative-eventing
kubectl api-resources --api-group='messaging.knative.dev'
kubectl api-resources --api-group='eventing.knative.dev'
kubectl api-resources --api-group='sources.knative.dev'


kubectl get pods -n knative-serving
kubectl get pods --all-namespaces -l 'app in(3scale-kourier-gateway,3scale-kourier-control)'
kubectl get pods -n knative-eventing


$a = kubectl get pods -n knative-serving -o json | ConvertFrom-Json | Select-Object @{Name="pods";Expression={$_.items}} 
$a.pods | Where-Object { $_.status.phase -ne "Running" }

kubectl get pods -n knative-serving -o json | ConvertFrom-Json | Where-Object { $_.items.status.phase -eq "Running" }
| Select-Object @{Name="pods";Expression={$_.items}} 


kubectl get pods <pod-name> -o custom-columns=NAME:.metadata.name,RSRC:.metadata.resourceVersion
kubectl get pods -n knative-serving -o custom-columns=NAME:.metadata.name,RSRC:.metadata.resourceVersion



TEST SERVING

kn service create hello --image ghcr.io/knative/helloworld-go:latest --port 8080 --env TARGET=World
kn service list

kn service update hello --env TARGET=Knative
kn revisions list

kn service update hello --traffic hello-00001=50 --traffic `@latest=50

Use to wait and observe a pod scale down to zero (0)
kubectl get pod -l serving.knative.dev/service=hello -w

TEST EVENTING

FAILED here already -- no broker; Quickstart defines `example-broker` ; Maybe not needed? Continue on first
kn broker create example-broker
kn broker list

kn service create cloudevents-player --image quay.io/ruben/cloudevents-player:latest

Oh wait, we do need  the `example-broker` when defining sinkbinding; grrr!
kn source binding create ce-player-binding --subject "Service:serving.knative.dev/v1:cloudevents-player" --sink broker:example-broker


kn trigger create cloudevents-trigger --sink cloudevents-player  --broker example-broker


APPLICATIONS INSTALLATIONS

TODO -- ADD BUILD STEPS

dotnet publish .\WeatherForecastWebApi\ --configuration Release

Copy-Item .\WeatherForecastWebApi\Dockerfile .\WeatherForecastWebApi\bin\Release\net6.0\publish\

Push-Location

Set-Location C:/src/github.com/ongzhixian/BasicMinikube/WeatherForecastWebApi/bin/Release/net6.0/publish

minikube image build . -t dev.local/weather-forecast-webapi:v1 -f ./Dockerfile

Pop-Location


weather-forecast-webapi

kubectl rollout restart deployment weather-forecast-webapi

kubectl create deployment weather-forecast-webapi --image=docker.io/library/weather-forecast-webapi:v1

kubectl expose deployment weather-forecast-webapi --type=NodePort --port=80

kubectl create ingress weather-forecast-webapi --rule="weather-forecast-webapi.internal/*=weather-forecast-webapi:80"

http://weather-forecast-webapi.internal/WeatherForecast




KNATIVE SETUP

kubectl apply --filename https://github.com/knative/serving/releases/download/knative-v1.8.3/serving-crds.yaml 
kubectl apply --filename https://github.com/knative/eventing/releases/download/knative-v1.8.5/eventing-crds.yaml

kubectl apply --filename https://github.com/knative/eventing/releases/download/knative-v1.14.2/eventing-crds.yaml
kubectl apply --filename https://github.com/knative/serving/releases/download/knative-v1.14.1/serving-crds.yaml