# Exploration on Kubernets

tldr; ingress is not such a good solution for resource limited environments (like your dev PC)

ClusterIP       - Internal to Kubernetes only
NodePort        - Maps a port (ranging from ? to ?) to the port that app is exposing
LoadBalancer    - Assigns an IP to every service! 
                  Requires `minikube tunnel` to get IPs (which requires admin rights)

Ingress only works well with NodePort and LoadBalancer.
But if we are trying to reduce port (NodePort) or IP (LoadBalancer) utilization, 
ingress is then not such a good solution.

What we are probably better off is with a reverse proxy service.


# Stuff here is based

To create ingress apply

kubectl apply -f ingress-nginx.ingress.yaml

curl.exe --resolve "mk-ingress.localhost:80:$( minikube ip )" -i http://hello-world.example
curl.exe --resolve "mk-ingress.localhost:80:172.18.87.225" -i http://mk-ingress.localhost


curl.exe --resolve "mk-ingress.localhost:80:$( minikube ip )" -i http://mk-ingress.localhost
curl.exe --resolve "mk-ingress.localhost:80:$( minikube ip )" -i http://mk-ingress.localhost

Other YAML files not used yet:

- service.yaml
- redis-patch.yaml
- configmaps.yaml


1.  Set up Ingress on Minikube with the NGINX Ingress Controller
    https://kubernetes.io/docs/tasks/access-application-cluster/ingress-minikube/

2.  Ingress-Nginx Controller > Exposing TCP and UDP services
    https://kubernetes.github.io/ingress-nginx/user-guide/exposing-tcp-udp-services/


# Further reading

https://akomljen.com/kubernetes-nginx-ingress-controller/
https://akomljen.com/get-automatic-https-with-lets-encrypt-and-kubernetes-ingress/