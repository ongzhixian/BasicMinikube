minikube image build . -t sample-flask-app1:1
kubectl create deployment sample-flask-app1 --image=sample-flask-app1:1
kubectl expose deployment sample-flask-app1 --port=5050 
#kubectl expose deployment sample-flask-app1 --port=5050 --target-port=5050

kubectl patch configmap tcp-services --patch '{"data":{"6379":"default/redis:6379"}}'


docker image rm $(docker images | grep "^<none>" | awk "{print $3}")


https://www.geeksforgeeks.org/how-to-run-a-flask-application/
https://www.geeksforgeeks.org/how-to-deploy-flask-app-on-kubernetes/

https://testdriven.io/blog/running-flask-on-kubernetes/
https://github.com/ginomempin/sample-flask-with-kubernetes/blob/master/app/main.py
https://reetesh.in/blog/nginx-as-reverse-proxy-for-kubernetes-services

