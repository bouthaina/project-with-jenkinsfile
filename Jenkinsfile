pipeline {
    agent any // Exécute sur n'importe quel agent disponible (peut être Jenkins lui-même)

    environment {
        // Définir le nom de votre compte Docker Hub
        DOCKERHUB_CREDENTIALS_ID = 'DOCKERHUB_CREDS' // L'ID des credentials Jenkins pour Docker Hub
        DOCKERHUB_USERNAME = 'bouthainabakouch' // Votre nom d'utilisateur Docker Hub
        BACKEND_IMAGE_NAME = "${env.DOCKERHUB_USERNAME}/image-backend-product"
        FRONTEND_IMAGE_NAME = "${env.DOCKERHUB_USERNAME}/image-frontend-product"
    }

    stages {
        stage('Checkout') {
            steps {
                echo 'Récupération du code...'
                // Récupère le code depuis le dépôt configuré dans le job Jenkins
                checkout scm
            }
        }

        stage('Build Backend') {
            steps {
                echo 'Construction du backend...'
                // Utilise l'outil dotnet configuré dans Jenkins ou disponible sur l'agent
                bat 'dotnet build ProductApp.API/ProductApp.API.csproj --configuration Release'
            }
        }

        stage('Test Backend') {
            steps {
                echo 'Tests du backend...'
                // Exécute les tests s'ils existent
                bat '''
                IF EXIST "ProductApp.API.Tests" (
                    dotnet test ProductApp.API.Tests/ProductApp.API.Tests.csproj --configuration Release --logger "trx;LogFileName=backend-test-results.trx" --results-directory .\\TestResults\\Backend
                ) ELSE (
                    echo "Pas de projet de test trouvé pour le backend"
                )
                '''
            }

            // Publier les résultats des tests
            post {
                always {
                    junit allowEmptyResults: true, testResults: 'TestResults/Backend/*.trx'
                }
            }
        }

        stage('Build Frontend') {
            steps {
                echo 'Construction du frontend...'
                bat 'dotnet build ProductApp.Client/ProductApp.Client.csproj --configuration Release'
            }
        }

        stage('Test Frontend') {
            steps {
                echo 'Tests du frontend...'
                // Exécute les tests s'ils existent
                bat '''
                IF EXIST "ProductApp.Client.Tests" (
                    dotnet test ProductApp.Client.Tests/ProductApp.Client.Tests.csproj --configuration Release --logger "trx;LogFileName=frontend-test-results.trx" --results-directory .\\TestResults\\Frontend
                ) ELSE (
                    echo "Pas de projet de test trouvé pour le frontend"
                )
                '''
            }
            // Publier les résultats des tests
            post {
                always {
                    junit allowEmptyResults: true, testResults: 'TestResults/Frontend/*.trx'
                }
            }
        }

        stage('Build Docker Images') {
            steps {
                echo 'Construction des images Docker...'
                // Assurez-vous que Docker est accessible par Jenkins
                bat "docker build -t ${env.BACKEND_IMAGE_NAME}:latest -f Dockerfile.backend ."
                bat "docker build -t ${env.FRONTEND_IMAGE_NAME}:latest -f Dockerfile.frontend ."
            }
        }

        stage('Push Docker Images') {
            // Cette étape est optionnelle, décommentez si vous voulez pousser vers Docker Hub

            when {
                branch 'main' // Pousser uniquement depuis la branche main
            }
            steps {
                echo 'Push des images vers Docker Hub...'
                withCredentials([usernamePassword(credentialsId: env.DOCKERHUB_CREDENTIALS_ID, usernameVariable: 'DOCKER_USER', passwordVariable: 'DOCKER_PASS')]) {
                    bat "echo ${env.DOCKER_PASS} | docker login -u ${env.DOCKER_USER} --password-stdin"
                    bat "docker push ${env.BACKEND_IMAGE_NAME}:latest"
                    bat "docker push ${env.FRONTEND_IMAGE_NAME}:latest"
                }
            }
            post {
                always {
                    bat 'docker logout'
                }
            }
            
        }

    
    }

    post {
        // Actions à exécuter à la fin du pipeline (succès, échec, etc.)
        always {
            echo 'Pipeline terminé.'
            // Nettoyage si nécessaire
            cleanWs()
        }
        success {
            echo 'Pipeline réussi !'
            // Envoyer une notification Slack ou email
        }
        failure {
            echo 'Pipeline échoué !'
            // Envoyer une notification Slack ou email
        }
    }
}
