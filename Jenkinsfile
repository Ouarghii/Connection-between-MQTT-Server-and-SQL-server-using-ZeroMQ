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
                    bat 'dotnet restore'
                    bat 'dotnet build'
                }
            }
        }
        
        stage('Run Tests MqttConnector') {
            steps {
                dir('tests/MqttConnectorTests') {
                    // Run unit tests for MqttConnector
                    bat 'dotnet test'
                }
            }
        }
        
        stage('Build SQLServerConnector') {
            steps {
                dir('SQLServerConnector') {
                    bat 'dotnet restore'
                    bat 'dotnet build'
                }
            }
        }
        
        stage('Run Tests SQLServerConnector') {
            steps {
                dir('tests/SQLServerConnectorTests') {
                    // Run unit tests for SQLServerConnector
                    bat 'dotnet test'
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

