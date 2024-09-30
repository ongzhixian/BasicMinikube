# Jenkins

Jenkins pipelines are based Groovy-based.


```
$env:JENKINS_HOME=C:/Apps/Jenkins/jenkins.config
java -jar C:/Apps/Jenkins/jenkins.war --httpPort=15080

We would need `hudson.plugins.git.GitSCM.ALLOW_LOCAL_CHECKOUT=true`, to allow Jenkins to build from local repository
java.exe -D"hudson.plugins.git.GitSCM.ALLOW_LOCAL_CHECKOUT=true" -jar jenkins.war 

```

## Plugins

Plugins to install for Jenkins visualization:
1.  Pipeline Stage View Plugin  <-- This is the plugin that shows the "green boxes" 😂😂😂
2.  Pipeline Graph View         <-- This is the plugin that shows the pipeline flow diagram


## Example script

```groovy
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

```


## Plugins
.NET SDK Support 

msbuild
dotnet

PowerShell
MSTest
dotcover

# Alternatives

Can think of looking at:

1.  TeamCity
    https://www.jetbrains.com/teamcity/download/

2.  UrbanCode (UrbanCode Velocity trial)
    https://www.ibm.com/products/urbancode

3.  GoCD


3.  Woodpecker CI
    https://woodpecker-ci.org/docs/intro

4.  https://kraken.ci/
5.  https://www.drone.io/


See: https://www.lambdatest.com/blog/best-jenkins-alternatives/

# Reference

Pipeline syntax
https://www.jenkins.io/doc/book/pipeline/syntax/

Groovy syntax
http://groovy-lang.org/syntax.html