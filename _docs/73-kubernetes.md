# Kubernetes

Limits and requests for memory are measured in bytes. 
You can express memory as a plain integer or as a fixed-point number using one of these quantity suffixes: 
 E,  P,  T,  G,  M,  k. 
You can also use the power-of-two equivalents: 
Ei, Pi, Ti, Gi, Mi, Ki. 


 (Note that 1024 = 1Ki but 1000 = 1k; I didn't choose the capitalization.)

1.5 will be serialized as "1500m" - 1.5Gi will be serialized as "1536Mi"

For example, the following represent roughly the same value:
128974848, 
129e6, 
129M,  
128974848000m, 
123Mi

## Redis
kubectl expose deployment redis --port=6379