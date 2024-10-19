# TestDummyConsoleApp

A simple console app for testing purposes.

# History

## 0.0.1


# Notes

## How to quickly setup pipeline in localhost Jenkins 

In Jenkins, add "item" with the following details:
Build Triggers:
	1.	Enable Poll SCM (just enable, no need to add schedule or anything)
Pipeline:
	1.	Definition: Pipeline script from SCM
		SCM: Git
		Repositories:
			Repository URL: file://C:/src/github.com/ongzhixian/Minikube
			Branches to build:
				Branch Specifier: */main
		Script path: TestDummyConsoleApp/Jenkinsfile
