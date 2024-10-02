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
                dotnetBuild(configuration: 'Release', noRestore: true, nologo: true, optionsString: "-p:AssemblyVersion=0.0.0.${BUILD_NUMBER} -nowarn:DV2001")
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
                sh 'minikube'
                echo 'Deploying application...'
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
