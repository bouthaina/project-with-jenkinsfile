pipeline {
    agent any

    environment {
        SLACK_WEBHOOK_URL = credentials('SLACK_WEBHOOK_URL')
        DOCKERHUB_CREDS = credentials('DOCKERHUB_CREDS')
        IMAGE_NAME_BACKEND = "${env.DOCKERHUB_USERNAME}/image-backend-product"
        IMAGE_NAME_FRONTEND = "${env.DOCKERHUB_USERNAME}/image-frontend-product"
    }

    stages {
        stage('Test Backend') {
            when {
                expression { 
                    def changedFiles = scm.changeSets.collect { it.items }.flatten().collect { it.path }
                    return changedFiles.any { it.startsWith('ProductApp.API/') }
                }
            }
            steps {
                script {
                    def testResultsDir = "TestResults_${env.BUILD_NUMBER}"
                    
                    dir('ProductApp.API') {
                        // Étape 1: Vérification de l'environnement
                        sh '''
                            echo "=== ENVIRONMENT DEBUG ==="
                            dotnet --version
                            ls -la
                            echo "========================"
                        '''
                        
                        // Étape 2: Restauration et build avec gestion d'erreur
                        try {
                            sh 'dotnet restore'
                            sh 'dotnet build --configuration Release --no-restore'
                            
                            // Étape 3: Exécution des tests avec journalisation détaillée
                            sh """
                                mkdir -p ../${testResultsDir}
                                dotnet test --configuration Release \
                                    --logger "trx;LogFileName=test-results.trx" \
                                    --collect:"XPlat Code Coverage" \
                                    --results-directory ../${testResultsDir} \
                                    --verbosity detailed
                            """
                        } catch (Exception ex) {
                            slackSend(
                                color: 'danger',
                                message: """
                                    :x: Backend Tests FAILED
                                    *Job*: ${env.JOB_NAME} #${env.BUILD_NUMBER}
                                    *Commit*: ${env.GIT_COMMIT.take(8)}
                                    *Error*: ${ex.toString()}
                                    *URL*: ${env.BUILD_URL}
                                """
                            )
                            error("Tests failed: ${ex.getMessage()}")
                        }
                    }
                    
                    // Archive des résultats même en cas d'échec
                    junit allowEmptyResults: true, testResults: "${testResultsDir}/*.trx"
                    archiveArtifacts artifacts: "${testResultsDir}/**/*", allowEmptyArchive: true
                }
            }
            post {
                success {
                    slackSend(
                        color: 'good',
                        message: """
                            :white_check_mark: Backend Tests PASSED
                            *Job*: ${env.JOB_NAME} #${env.BUILD_NUMBER}
                            *Commit*: ${env.GIT_COMMIT.take(8)}
                            *URL*: ${env.BUILD_URL}
                        """
                    )
                }
            }
        }

        // ... [les autres stages restent inchangés]
    }
}