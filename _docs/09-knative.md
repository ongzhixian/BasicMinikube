Knative

LATEST
kubectl apply --filename https://github.com/knative/eventing/releases/download/knative-v1.14.2/eventing-crds.yaml
kubectl apply --filename https://github.com/knative/serving/releases/download/knative-v1.14.1/serving-crds.yaml


SERVING
kubectl apply -f https://github.com/knative/serving/releases/download/knative-v1.14.1/serving-crds.yaml
kubectl apply -f https://github.com/knative/serving/releases/download/knative-v1.14.1/serving-core.yaml
kubectl apply -f https://github.com/knative/serving/releases/download/knative-v1.14.1/serving-default-domain.yaml
kubectl apply -f https://github.com/knative/serving/releases/download/knative-v1.14.1/serving-hpa.yaml

KOURIER
kubectl apply -f https://github.com/knative/net-kourier/releases/download/knative-v1.14.0/kourier.yaml
kubectl patch configmap/config-network --namespace knative-serving --type merge --patch '{"data":{"ingress-class":"kourier.ingress.networking.knative.dev"}}'
kubectl --namespace kourier-system get service kourier

EVENTING
kubectl apply -f https://github.com/knative/eventing/releases/download/knative-v1.14.2/eventing-crds.yaml
kubectl apply -f https://github.com/knative/eventing/releases/download/knative-v1.14.2/eventing-core.yaml
kubectl apply -f https://github.com/knative/eventing/releases/download/knative-v1.14.2/in-memory-channel.yaml
kubectl apply -f https://github.com/knative/eventing/releases/download/knative-v1.14.2/mt-channel-broker.yaml

NATS Channel (instead of the in-memory)
kubectl apply -f https://github.com/knative-extensions/eventing-natss/releases/download/knative-v1.14.1/eventing-natss.yaml

CONTOUR (ingress controller ; Can we use nginx?)
kubectl apply --filename https://projectcontour.io/quickstart/contour.yaml
kubectl patch configmap/config-domain -n knative-serving --type merge -p "{ `"data`":{ `"$(minikube ip).nip.io`" : `"`" } }"

```
apiVersion: v1
kind: ConfigMap
metadata:
  name: config-br-defaults
  namespace: knative-eventing
data:
  default-br-config: |
    # This is the cluster-wide default broker channel.
    clusterDefault:
      brokerClass: MTChannelBasedBroker
      apiVersion: v1
      kind: ConfigMap
      name: imc-channel
      namespace: knative-eventing
    # This allows you to specify different defaults per-namespace,
    # in this case the "some-namespace" namespace will use the Kafka
    # channel ConfigMap by default (only for example, you will need
    # to install kafka also to make use of this).
    namespaceDefaults:
      some-namespace:
        brokerClass: MTChannelBasedBroker
        apiVersion: v1
        kind: ConfigMap
        name: kafka-channel
        namespace: knative-eventing
```

VERIFY INSTALLATION
kubectl get pods -n knative-serving
kubectl --namespace kourier-system get service kourier
kubectl get pods --all-namespaces -l 'app in(3scale-kourier-gateway,3scale-kourier-control)'
kubectl get pods -n projectcontour

CHECK RESOURCES
kubectl api-resources --api-group='serving.knative.dev'
kubectl api-resources --api-group='messaging.knative.dev'
kubectl api-resources --api-group='eventing.knative.dev'
kubectl api-resources --api-group='sources.knative.dev'



Testing

kn service create cloudevents-player --image quay.io/ruben/cloudevents-player:latest


minikube image pull quay.io/rhdevelopers/eventinghello:0.0.2
kubectl apply -f .\k8s\hello.sink.yaml
kubectl apply -f .\k8s\hello.pingsource.yaml
kubectl get pingsources.sources.knative.dev eventinghello-ping-source

kn source ping delete eventinghello-ping-source
kn service delete eventinghello

kubectl patch configmap/config-network --namespace knative-serving --type merge --patch '{"data":{"ingress-class":"kourier.ingress.networking.knative.dev"}}'


------------------

Done! kubectl is now configured to use "minikube" cluster and "default" namespace by default
minikube -p minikube addons enable registry

kubectl apply --filename https://github.com/knative/serving/releases/download/knative-v1.8.3/serving-crds.yaml 
kubectl apply --filename https://github.com/knative/eventing/releases/download/knative-v1.8.5/eventing-crds.yaml

kubectl apply --filename https://github.com/knative/serving/releases/download/knative-v1.8.3/serving-core.yaml

kubectl apply --filename https://github.com/knative/net-kourier/releases/download/knative-v1.8.1/kourier.yaml
kubectl patch configmap/config-network -n knative-serving --type merge -p '{"data":{"ingress.class":"kourier.ingress.networking.knative.dev"}}'



--- 
Using QuickStart

kn service create hello --image ghcr.io/knative/helloworld-go:latest --port 8080 --env TARGET=World

kn service update hello --env TARGET=Knative

kn service update hello --traffic hello-00001=50 --traffic hello-00003=50
kn service update hello --traffic hello-00001=50 --traffic `@latest=50

kn revisions list

http://hello.default.10.104.83.2.sslip.io/
http://hello.default.10.104.83.2.sslip.io

kn broker list

kn service create cloudevents-player --image quay.io/ruben/cloudevents-player:latest
kn source binding create ce-player-binding --subject "Service:serving.knative.dev/v1:cloudevents-player" --sink broker:example-broker
kn trigger create cloudevents-trigger --sink cloudevents-player  --broker example-broker