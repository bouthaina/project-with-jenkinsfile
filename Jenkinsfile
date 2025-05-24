pipeline {
    agent any // Exécute sur n'importe quel agent disponible (peut être Jenkins lui-même)

    environment {
        // Définir le nom de votre compte Docker Hub
        DOCKERHUB_CREDENTIALS_ID = 'DOCKERHUB_CREDS' // L'ID des credentials Jenkins pour Docker Hub
        DOCKERHUB_USERNAME = 'bouthainabakouch' // Votre nom d'utilisateur Docker Hub
        BACKEND_IMAGE_NAME = "${env.DOCKERHUB_USERNAME}/image-backend-product"
        FRONTEND_IMAGE_NAME = "${env.DOCKERHUB_USERNAME}/image-frontend-product"
        //SLACK_WEBHOOK_URL = 'SLACK_WEBHOOK_URL'
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
                    bat "docker push %BACKEND_IMAGE_NAME%:latest"
                    bat "docker push %FRONTEND_IMAGE_NAME%:latest"
                }
            }
        
        
            post {

                always {
                    bat 'docker logout'
                }
                success {
                    script {
                        withCredentials([string(credentialsId: 'SLACK_WEBHOOK_URL', variable: 'SLACK_WEBHOOK')]) {
                            def payload = """
                            {
                                "text": ":white_check_mark: *Docker Push réussi !*\\n*Backend:* ${BACKEND_IMAGE_NAME}:latest\\n*Frontend:* ${FRONTEND_IMAGE_NAME}:latest\\n<${env.BUILD_URL}|Voir le build>"
                            }
                            """
                            writeFile file: 'slack-success.json', text: payload
                            bat "curl -X POST -H \"Content-type: application/json\" --data @slack-success.json %SLACK_WEBHOOK%"
                        }
                    }
                }
                failure {
                    script {
                        withCredentials([string(credentialsId: 'SLACK_WEBHOOK_URL', variable: 'SLACK_WEBHOOK')]) {
                            def payload = """
                            {
                                "text": ":x: *Échec du push Docker !*\\nJob: ${env.JOB_NAME} #${env.BUILD_NUMBER}\\n<${env.BUILD_URL}|Voir le build>"
                            }
                            """
                            writeFile file: 'slack-failure.json', text: payload
                            bat "curl -X POST -H \"Content-type: application/json\" --data @slack-failure.json %SLACK_WEBHOOK%"
                        }
                    }
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
