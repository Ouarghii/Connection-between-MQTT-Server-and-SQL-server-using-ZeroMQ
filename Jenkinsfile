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
        
        stage('Run Tests MqttConnector') {
            steps {
                dir('tests/MqttConnectorTests') {
                    // Run unit tests for MqttConnector
                    sh 'dotnet test'
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
        
        stage('Run Tests SQLServerConnector') {
            steps {
                dir('tests/SQLServerConnectorTests') {
                    // Run unit tests for SQLServerConnector
                    sh 'dotnet test'
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
