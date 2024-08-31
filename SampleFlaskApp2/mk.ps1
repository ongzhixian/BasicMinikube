# REMINDER: For local development, you NEED to tag with concrete version number
# If you tag it with latest, Kubernetes will ALWAYS lookup container repository
# No known/good way to get around this for now
minikube image build . -t sample-flask-app2:1
kubectl create deployment sample-flask-app2 --image=sample-flask-app2:1
kubectl expose deployment sample-flask-app2 --port=5052
