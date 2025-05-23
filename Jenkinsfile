pipeline {
    agent any

    environment {
        SLACK_WEBHOOK_URL = credentials('SLACK_WEBHOOK_URL')
        DOCKERHUB_CREDS = credentials('DOCKERHUB_CREDS')
        IMAGE_NAME_BACKEND = "${env.DOCKERHUB_USERNAME}/image-backend-product"
        IMAGE_NAME_FRONTEND = "${env.DOCKERHUB_USERNAME}/image-frontend-product"
    }

    stages {
        // Étape de debug pour voir les fichiers modifiés
        stage('Debug Changes') {
            steps {
                script {
                    def changes = scm.changeSets.collect { it.items }.flatten().collect { it.path }
                    echo "Changed files: ${changes}"
                    echo "Build reason: ${currentBuild.buildCauses}"
                }
            }
        }

        // Backend - Version améliorée de la condition when
        stage('Test Backend') {
            when {
                anyOf {
                    changeset 'ProductApp.API/**'
                    expression { params.FORCE_RUN == true }
                    buildingTag()
                }
            }
            steps {
                script {
                    dir('ProductApp.API') {
                        sh 'dotnet restore'
                        sh 'dotnet build --configuration Release'
                        sh 'dotnet test --configuration Release --logger "trx;LogFileName=test-results.trx" --collect:"XPlat Code Coverage"'
                    }
                }
            }
            post {
                always {
                    junit '**/TestResults/*.trx'
                    archiveArtifacts artifacts: '**/TestResults/**', allowEmptyArchive: true
                }
            }
        }

        // Autres étapes avec la même logique conditionnelle améliorée
        stage('Build Backend Docker') {
            when {
                anyOf {
                    changeset 'ProductApp.API/**'
                    expression { params.FORCE_RUN == true }
                    buildingTag()
                }
                expression { currentBuild.resultIsBetterOrEqualTo('SUCCESS') }
            }
            steps {
                script {
                    docker.withRegistry('https://registry.hub.docker.com', 'dockerhub-creds') {
                        docker.build("${env.IMAGE_NAME_BACKEND}:${env.BUILD_NUMBER}", "-f Dockerfile.backend .").push()
                    }
                }
            }
        }

        // ... [appliquer la même logique aux étapes frontend]
    }

    parameters {
        booleanParam(name: 'FORCE_RUN', defaultValue: false, description: 'Forcer l\'exécution complète du pipeline')
    }
}