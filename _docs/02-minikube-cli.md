# Minikube CLI

## Enabling normal user account to run minikube

By default, Minikube requires administrative privileges (because it makes use Hyper-V)
To run minikube without administrative privileges, open a PowerShell session in administrative mode
Then run the following command:

```
Add-LocalGroupMember -Group "Hyper-V Administrators" -Member ([System.Security.Principal.WindowsIdentity]::GetCurrent().Name)

```

## Create a minikube cluster

minikube start --cpus='2' --memory='4096MB' --disk-size='20480mb'

## Enable some commonly use minikube addons

minikube addons enable registry
minikube addons enable metrics-server
minikube addons enable dashboard
minikube addons enable ingress
minikube addons enable ingress-dns


Add-DnsClientNrptRule -Namespace ".localhost" -NameServers "$(minikube ip)"

## Access registry

```
kubectl port-forward --namespace kube-system service/registry 5000:80
kubectl port-forward --namespace kube-system service/registry 18002:80

```

curl.exe http://localhost:18002/v2/_catalog

Note: The command `kubectl port-forward` is all blocking executables.

## Minikube dashboard

Method 1:

kubectl proxy --address='0.0.0.0' --accept-hosts='^*$' --port=18001

http://localhost:18001/api/v1/namespaces/kubernetes-dashboard/services/http:kubernetes-dashboard:/proxy/#/workloads?namespace=default

--OR--

Method 2:

minikube dashboard

http://127.0.0.1:52501/api/v1/namespaces/kubernetes-dashboard/services/http:kubernetes-dashboard:/proxy/#/workloads?namespace=default


Method 1 exposes dashboard to be accessible remotely
Method 2 is more restrictive in the sense that it only allows `127.0.0.1:<random-port>` to access the dashboard.
Note: The command `minikube dashboard` and `kubectl proxy` are all blocking executables.


## Initial test deployment



kubectl create deployment my-nginx --image=nginx

kubectl create deployment web --image=gcr.io/google-samples/hello-app:1.0

kubectl expose deployment web --type=NodePort --port=8080


## Ingress DNS

We can use the ingress DNS add-on in Minikube to solve the `hosts` file pollution issue
that comes along with using just plain old ingress.

Open Powershell as Administrator and execute the following.

```
Add-DnsClientNrptRule -Namespace ".lan" -NameServers "$(minikube ip)"
```

``` When Minikube IP changes (after restart)
Get-DnsClientNrptRule | Where-Object {$_.Namespace -eq '.test'} | Remove-DnsClientNrptRule -Force; Add-DnsClientNrptRule -Namespace ".test" -NameServers "$(minikube ip)"
```


```ps1 ; to test
nslookup hello-world.lan $(minikube ip)

ping hello-world.lan

http://hello-world.lan/
```

https://minikube.sigs.k8s.io/docs/handbook/addons/ingress-dns/#Windows


## Create ingress

```ps1 ;syntax
kubectl create ingress NAME --rule=host/path=service:port[,tls[=secret]]  [options]
kubectl create ingress example-ingress --rule=host/path=service:port
```

```ps1 ;equivalent
kubectl create ingress example-ingress --rule="hello-world.info/=web:8080"      // pathType=Exact
kubectl create ingress example-ingress --rule="hello-world.info/*=web:8080"     // pathType=Prefix

kubectl create ingress example-ingress --rule="hello-world.lan/*=web:8080"     // pathType=Prefix
```

```yaml ;equivalent
apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  name: example-ingress
  annotations:
    nginx.ingress.kubernetes.io/rewrite-target: /$1
spec:
  rules:
    - host: hello-world.info
      http:
        paths:
          - path: /
            pathType: Prefix
            backend:
              service:
                name: web
                port:
                  number: 8080
```

```ps1 ; test
curl.exe --resolve hello-world.info:80:172.26.76.31 -i http://hello-world.info
curl.exe --resolve hello-world.info:80:172.26.76.31 -i http://hello-world.info
curl.exe --resolve hello-world.lan:80:172.26.76.31 -i http://hello-world.lan
```

## Add entry to `/etc/hosts`

If we are not using ingress-dns add-on and we want to access the ingress directly from
minikube host PC, we would need to add the ingress hostnames to host PC's hosts file:

<minikube-ip> hello-world.lan
172.17.0.15 hello-world.lan

(This is the `hosts` file pollution problem)

## Other ways to solve the hosts file pollution

1.  Have one single entry in the host file that points to a reverse-proxy.
    (which then resolves to other services in the cluster)

2.  kubectl port-forward --address 0.0.0.0 deployment/ingress-nginx-controller 8443:443 --namespace ingress-nginx
    Then access ingress via https://my.host.running.minikube:8443/

3.  minikube tunnel
    (require administrative privileges)

4.  Customize DNS (have not tried this before)
    See: https://kubernetes.io/docs/tasks/administer-cluster/dns-custom-nameservers/

5.  Host your own DNS (have not tried this before)

