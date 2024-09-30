pipeline {

    agent any

    stages {
        stage('Checkout') {
            steps {
                // git url: 'https://github.com/your-username/my-first-pipeline.git', branch: 'main'
                echo 'Checkout repo'
            }
        }

        stage('Build') {
            steps {
                // Install dependencies
                //sh 'npm install'
                echo 'Build assembly'
            }
        }

        stage('Test') {
            steps {
                // Run tests
                // sh 'npm test'
                echo 'Test'
            }
        }

        stage('Deploy') {
            steps {
                // Deployment logic goes here (e.g., pushing to a cloud provider)
                echo 'Deploying application...'
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
