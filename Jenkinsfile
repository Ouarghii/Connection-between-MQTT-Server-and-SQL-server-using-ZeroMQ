pipeline {
    agent any
    
    stages {
        stage('Initialize') {
            steps {
                checkout scm
            }
        }
        
        stage('Build MqttConnector') {
            steps {
                dir('MqttConnector') {
                    sh 'dotnet restore'
                    sh 'dotnet build'
                }
            }
        }
        
        stage('Build SQLServerConnector') {
            steps {
                dir('SQLServerConnector') {
                    sh 'dotnet restore'
                    sh 'dotnet build'
                }
            }
        }
        
        stage('Deploy MqttConnector') {
            steps {
                dir('MqttConnector') {
                    // Deployment steps for MqttConnector
                    // Example: deploy to Docker container
                }
            }
        }
        
        stage('Deploy SQLServerConnector') {
            steps {
                dir('SQLServerConnector') {
                    // Deployment steps for SQLServerConnector
                    // Example: deploy to Kubernetes
                }
            }
        }
    }
    
    post {
        always {
            // Clean up resources if needed
            echo 'Always block executed'
        }
        success {
            // Send notification on success (e.g., email, Slack)
            echo 'Deployment successful!'
        }
        failure {
            // Send notification on failure (e.g., email, Slack)
            echo 'Deployment failed!'
        }
    }
}


