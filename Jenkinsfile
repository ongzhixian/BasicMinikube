pipeline {

    agent any

    stages {
        stage('Clean') {
            steps {
                dotnetClean(configuration: 'Release', nologo: true)
            }
        }

        stage('Build') {
            steps {
                dotnetRestore()
                dotnetBuild(project: 'SimpleJobConsoleApp', configuration: 'Release', noRestore: true, nologo: true, optionsString: "-p:AssemblyVersion=0.0.0.${BUILD_NUMBER} -nowarn:DV2001")
            }
        }

        stage('Test') {
            steps {
                dotnetTest(configuration: 'Release', noBuild: true, noRestore: true, nologo: true)
            }
        }

        stage('Deploy') {
            steps {
                // Deployment logic goes here (e.g., pushing to a cloud provider)
                echo 'Deploying application...'
                pwsh '''
minikube status
$nextContainerImageName = "simple-job-console-app:0.0.0.$env:BUILD_NUMBER"
Write-Host $nextContainerImageName

Push-Location
Set-Location .\\SimpleJobConsoleApp\\

minikube image build . -t $nextContainerImageName -f .\\Dockerfile
kubectl set image cronjob/test-job test-job=docker.io/library/$nextContainerImageName

Pop-Location
'''
                echo 'Deploying application done'
            }
        }
    }

    post {
        always {
            echo 'Pipeline completed.'
        }
        failure {
            echo 'Pipeline failed!'
        }
    }
}
