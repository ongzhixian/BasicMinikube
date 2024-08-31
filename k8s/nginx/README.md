# Update image of deployment

kubectl set image (-f FILENAME | TYPE NAME) CONTAINER_NAME_1=CONTAINER_IMAGE_1 ... CONTAINER_NAME_N=CONTAINER_IMAGE_N

                  [MY-DEPLOYMENT] [CONTAINER] [NEW-IMAGE]
kubectl set image deployment/prox nginx-proxy=nginx-proxy:3

# Set a deployment's nginx container image to 'nginx:1.9.1', and its busybox container image to 'busybox'
kubectl set image deployment/nginx busybox=busybox nginx=nginx:1.9.1

https://github.com/ginomempin/sample-flask-with-kubernetes/tree/master
https://reetesh.in/blog/nginx-as-reverse-proxy-for-kubernetes-services
https://benincosa.com/?p=3845
