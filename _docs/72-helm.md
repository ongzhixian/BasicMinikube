# Helm


## Files in Helm 

.helmignore: 
    It is used to define all the files that we don’t want to include in the helm chart. It works similarly to the .gitignore file.

Chart.yaml: 
    It contains information about the helm chart like version, name, description, etc.

values.yaml: 
    In this file, we define the values for the YAML templates. For example, image name, replica count, HPA values, etc. 
    As we explained earlier only the values.yaml file changes in each environment. Also, you can override these values dynamically or at the time of installing the chart using --values or --set command.
    
charts: 
    We can add another chart’s structure inside this directory if our main charts have some dependency on others. By default this directory is empty.
    
templates: 
    This directory contains all the Kubernetes manifest files that form an application. 
    These manifest files can be templated to access values from values.yaml file. 
    Helm creates some default templates for Kubernetes objects like deployment.yaml, service.yaml etc, which we can use directly, modify, or override with our files.
    
templates/NOTES.txt: 
    This is a plaintext file that gets printed out after the chart is successfully deployed. 
    
templates/_helpers.tpl:
     That file contains several methods and sub-template. These files are not rendered to Kubernetes object definitions but are available everywhere within other chart templates for use. 
    
templates/tests/: We can define tests in our charts to validate that your chart works as expected when it is installed. 



## CLI

helm create nginx-chart


# Reference

https://devopscube.com/create-helm-chart/