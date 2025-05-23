pipeline {
    agent any

    environment {
        // Credentials
        SLACK_WEBHOOK_URL = credentials('SLACK_WEBHOOK_URL')
        DOCKERHUB_CREDS = credentials('DOCKERHUB_CREDS')
        
        // Variables
        IMAGE_NAME_BACKEND = "${env.DOCKERHUB_USERNAME}/image-backend-product"
        IMAGE_NAME_FRONTEND = "${env.DOCKERHUB_USERNAME}/image-frontend-product"
        BUILD_NUMBER = "${env.BUILD_NUMBER}" // Variable native Jenkins
    }

    stages {
        // Backend
        stage('Test Backend') {
            when {
                changeset 'ProductApp.API/**'
            }
            steps {
                script {
                    dir('ProductApp.API') {
                        sh 'dotnet restore'
                        sh 'dotnet build --configuration Release'
                        try {
                            sh 'dotnet test --configuration Release --logger "trx;LogFileName=test-results.trx" --collect:"XPlat Code Coverage"'
                        } catch (err) {
                            slackSend(
                                color: 'danger',
                                message: ":x: Backend Tests FAILED\nJob: ${env.JOB_NAME} #${env.BUILD_NUMBER}\nCommit: ${env.GIT_COMMIT.take(8)}\nURL: ${env.BUILD_URL}"
                            )
                            error('Tests failed')
                        }
                    }
                }
            }
            post {
                always {
                    junit '**/TestResults/*.trx'
                    archiveArtifacts artifacts: '**/TestResults/**', allowEmptyArchive: true
                }
                success {
                    slackSend(
                        color: 'good',
                        message: ":white_check_mark: Backend Tests PASSED\nJob: ${env.JOB_NAME} #${env.BUILD_NUMBER}\nCommit: ${env.GIT_COMMIT.take(8)}\nURL: ${env.BUILD_URL}"
                    )
                }
            }
        }

        stage('Build Backend Docker') {
            when {
                changeset 'ProductApp.API/**'
                expression { currentBuild.resultIsBetterOrEqualTo('SUCCESS') }
            }
            steps {
                script {
                    docker.withRegistry('https://registry.hub.docker.com', 'dockerhub-creds') {
                        def backendImage = docker.build("${IMAGE_NAME_BACKEND}:${BUILD_NUMBER}", "-f Dockerfile.backend .")
                        backendImage.push()
                        backendImage.push('latest')
                    }
                }
            }
            post {
                failure {
                    slackSend(
                        color: 'danger',
                        message: ":x: Backend Docker Build FAILED\nImage: ${IMAGE_NAME_BACKEND}:${BUILD_NUMBER}\nJob: ${env.JOB_NAME}\nURL: ${env.BUILD_URL}"
                    )
                }
                success {
                    slackSend(
                        color: 'good',
                        message: ":white_check_mark: Backend Image Pushed\nImage: ${IMAGE_NAME_BACKEND}:${BUILD_NUMBER}\nURL: ${env.BUILD_URL}"
                    )
                }
            }
        }

        // Frontend
        stage('Test Frontend') {
            when {
                changeset 'ProductApp.Client/**'
            }
            steps {
                script {
                    dir('ProductApp.Client') {
                        sh 'dotnet restore'
                        sh 'dotnet build --configuration Release'
                        try {
                            sh 'dotnet test --configuration Release'
                        } catch (err) {
                            slackSend(
                                color: 'danger',
                                message: ":x: Frontend Tests FAILED\nJob: ${env.JOB_NAME} #${env.BUILD_NUMBER}\nCommit: ${env.GIT_COMMIT.take(8)}\nURL: ${env.BUILD_URL}"
                            )
                            error('Tests failed')
                        }
                    }
                }
            }
            post {
                success {
                    slackSend(
                        color: 'good',
                        message: ":white_check_mark: Frontend Tests PASSED\nJob: ${env.JOB_NAME} #${env.BUILD_NUMBER}\nCommit: ${env.GIT_COMMIT.take(8)}\nURL: ${env.BUILD_URL}"
                    )
                }
            }
        }

        stage('Build Frontend Docker') {
            when {
                changeset 'ProductApp.Client/**'
                expression { currentBuild.resultIsBetterOrEqualTo('SUCCESS') }
            }
            steps {
                script {
                    docker.withRegistry('https://registry.hub.docker.com', 'dockerhub-creds') {
                        def frontendImage = docker.build("${IMAGE_NAME_FRONTEND}:${BUILD_NUMBER}", "-f Dockerfile.frontend .")
                        frontendImage.push()
                        frontendImage.push('latest')
                    }
                }
            }
            post {
                success {
                    slackSend(
                        color: 'good',
                        message: ":white_check_mark: Frontend Image Pushed\nImage: ${IMAGE_NAME_FRONTEND}:${BUILD_NUMBER}\nURL: ${env.BUILD_URL}"
                    )
                }
            }
        }
    }
}