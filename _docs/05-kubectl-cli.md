# kubectl

kubectl create deployment 

kubectl create deployment weather-forecast-webapi --image=docker.io/library/weather-forecast-webapi:v1

kubectl expose deployment weather-forecast-webapi --type=NodePort --port=80

minikube service weather-forecast-webapi --url
--> http://172.26.76.31:31146/WeatherForecast

// pathType=Prefix
kubectl create ingress weather-forecast-webapi --rule="weather-forecast-webapi.lan/*=weather-forecast-webapi:80"
--> http://weather-forecast-webapi.lan/WeatherForecast


## Accessing running pod

kubectl exec weather-forecast-webapi-bb49fcf65-txf25 -it -- /bin/bash

## Running ad-hoc pods

https://www.busybox.net/
kubectl run test4 -it --image=busybox

kubectl run bb -it --image=busybox --rm
kubectl run bb -it --image=busybox

kubectl attach bb -it -c bb 

kubectl exec bb -c bb -i -t

If running busybox with a PV, we have to exec to get a shell
Attach won't work because the main thread is tied up in the loop.
```
kubectl apply -f .\pv-demo.yaml
kubectl exec test -it -- sh
```

## Volumes

kubectl delete pv <pv-name> --grace-period=0 --force
use `kubectl cp` to copy files to mounted volume