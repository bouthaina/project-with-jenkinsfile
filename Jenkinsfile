pipeline {
    agent any

    environment {
        DOCKERHUB_CREDENTIALS_ID = 'DOCKERHUB_CREDS'
        DOCKERHUB_USERNAME = 'bouthainabakouch'
        BACKEND_IMAGE_NAME = "${env.DOCKERHUB_USERNAME}/image-backend-product"
        FRONTEND_IMAGE_NAME = "${env.DOCKERHUB_USERNAME}/image-frontend-product"
    }

    stages {
        stage('Checkout') {
            steps {
                echo 'Récupération du code...'
                checkout scm
            }
        }

        stage('Build Backend') {
            steps {
                echo 'Construction du backend...'
                bat 'dotnet build ProductApp.API/ProductApp.API.csproj --configuration Release'
            }
        }

        stage('Test Backend') {
            steps {
                echo 'Tests du backend...'
                bat '''
                IF EXIST "ProductApp.API.Tests" (
                    dotnet test ProductApp.API.Tests/ProductApp.API.Tests.csproj --configuration Release --logger "trx;LogFileName=backend-test-results.trx" --results-directory .\\TestResults\\Backend
                ) ELSE (
                    echo "Pas de projet de test trouvé pour le backend"
                )
                '''
            }
            post {
                always {
                    junit allowEmptyResults: true, testResults: 'TestResults/Backend/*.trx'
                }
                success {
                    script {
                        withCredentials([string(credentialsId: 'SLACK_WEBHOOK_URL', variable: 'SLACK_WEBHOOK')]) {
                            writeFile file: 'slack-backend-success.json', text: """
                            { "text": ":white_check_mark: *Tests backend réussis !*\n<${env.BUILD_URL}|Voir le build>" }
                            """
                            bat "curl -X POST -H \"Content-type: application/json\" --data @slack-backend-success.json %SLACK_WEBHOOK%"
                        }
                    }
                }
                failure {
                    script {
                        withCredentials([string(credentialsId: 'SLACK_WEBHOOK_URL', variable: 'SLACK_WEBHOOK')]) {
                            writeFile file: 'slack-backend-failure.json', text: """
                            { "text": ":x: *Échec des tests backend !*\n<${env.BUILD_URL}|Voir le build>" }
                            """
                            bat "curl -X POST -H \"Content-type: application/json\" --data @slack-backend-failure.json %SLACK_WEBHOOK%"
                        }
                    }
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
                bat '''
                IF EXIST "ProductApp.Client.Tests" (
                    dotnet test ProductApp.Client.Tests/ProductApp.Client.Tests.csproj --configuration Release --logger "trx;LogFileName=frontend-test-results.trx" --results-directory .\\TestResults\\Frontend
                ) ELSE (
                    echo "Pas de projet de test trouvé pour le frontend"
                )
                '''
            }
            post {
                always {
                    junit allowEmptyResults: true, testResults: 'TestResults/Frontend/*.trx'
                }
                success {
                    script {
                        withCredentials([string(credentialsId: 'SLACK_WEBHOOK_URL', variable: 'SLACK_WEBHOOK')]) {
                            writeFile file: 'slack-frontend-success.json', text: """
                            { "text": ":white_check_mark: *Tests frontend réussis !*\n<${env.BUILD_URL}|Voir le build>" }
                            """
                            bat "curl -X POST -H \"Content-type: application/json\" --data @slack-frontend-success.json %SLACK_WEBHOOK%"
                        }
                    }
                }
                failure {
                    script {
                        withCredentials([string(credentialsId: 'SLACK_WEBHOOK_URL', variable: 'SLACK_WEBHOOK')]) {
                            writeFile file: 'slack-frontend-failure.json', text: """
                            { "text": ":x: *Échec des tests frontend !*\n<${env.BUILD_URL}|Voir le build>" }
                            """
                            bat "curl -X POST -H \"Content-type: application/json\" --data @slack-frontend-failure.json %SLACK_WEBHOOK%"
                        }
                    }
                }
            }
        }

        stage('Build Docker Images') {
            steps {
                echo 'Construction des images Docker...'
                bat "docker build -t ${env.BACKEND_IMAGE_NAME}:latest -f Dockerfile.backend ."
                bat "docker build -t ${env.FRONTEND_IMAGE_NAME}:latest -f Dockerfile.frontend ."
            }
        }

        stage('Push Docker Images') {
            when {
                expression {
                    env.GIT_BRANCH == 'origin/main' ||
                    env.BRANCH_NAME == 'main' ||
                    (env.CHANGE_TARGET == 'main' && env.CHANGE_ID)
                }
            }
            steps {
                echo 'Push des images vers Docker Hub...'
                withCredentials([usernamePassword(
                    credentialsId: env.DOCKERHUB_CREDENTIALS_ID,
                    usernameVariable: 'DOCKER_USER',
                    passwordVariable: 'DOCKER_PASS'
                )]) {
                    bat 'echo %DOCKER_PASS% | docker login -u %DOCKER_USER% --password-stdin'
                    bat "docker push ${env.BACKEND_IMAGE_NAME}:latest"
                    bat "docker push ${env.FRONTEND_IMAGE_NAME}:latest"
                }
            }
            post {
                always {
                    bat 'docker logout'
                }
                success {
                    script {
                        withCredentials([string(credentialsId: 'SLACK_WEBHOOK_URL', variable: 'SLACK_WEBHOOK')]) {
                            writeFile file: 'slack-docker-success.json', text: """
                            {
                                "text": ":whale: *Images Docker poussées avec succès !*\n<${env.BUILD_URL}|Voir le build>"
                            }
                            """
                            bat "curl -X POST -H \"Content-type: application/json\" --data @slack-docker-success.json %SLACK_WEBHOOK%"
                        }
                    }
                }
                failure {
                    script {
                        withCredentials([string(credentialsId: 'SLACK_WEBHOOK_URL', variable: 'SLACK_WEBHOOK')]) {
                            writeFile file: 'slack-docker-failure.json', text: """
                            {
                                "text": ":x: *Échec du push des images Docker !*\n<${env.BUILD_URL}|Voir le build>"
                            }
                            """
                            bat "curl -X POST -H \"Content-type: application/json\" --data @slack-docker-failure.json %SLACK_WEBHOOK%"
                        }
                    }
                }
            }
        }
    }

    post {
        always {
            echo 'Pipeline terminé.'
            cleanWs()
        }
    }
}
