kubectl create configmap sample-python -o yaml --dry-run=client --save-config `
    --from-file=config.json | kubectl apply -f -


kubectl create cronjob sample-python --image=docker.io/library/sample-python:0.0.0.2 --schedule="0 */2 * * *" --dry-run=client -o yaml --save-config

