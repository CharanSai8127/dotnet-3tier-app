pipeline {
    agent any

    environment {
        SCANNER_HOME = tool 'sonar-scanner'
        SONAR_ENV = 'sonar'
        DOCKERHUB_CRED = 'docker-cred'
        DOCKER_USER = 'charansait372'
        SONAR_TOKEN = credentials('sonar-token')
    }

    stages {
        stage('Git Checkout') {
            steps {
                git branch: 'main', url: 'https://github.com/CharanSai8127/dotnet-3tier-app.git'
            }
        }

        stage('Restore Dependencies') {
            steps {
                sh 'dotnet restore'
            }
        }

        stage('Build') {
            steps {
                sh 'dotnet build --no-restore -c Release'
            }
        }

        stage('Run Unit Tests with Coverage') {
            steps {
                sh 'dotnet test --collect:"XPlat Code Coverage"'
            }
        }

        stage('SonarQube Analysis') {
            steps {
                withSonarQubeEnv("${SONAR_ENV}") {
                    sh '''
                    dotnet-sonarscanner begin \
                        /k:"DotNetMongoCRUDApp" \
                        /d:sonar.host.url="${SONAR_HOST_URL}" \
                        /d:sonar.login="${SONAR_TOKEN}" \
                        /d:sonar.cs.vscoveragexml.reportsPaths="**/coverage.cobertura.xml"

                    dotnet build
                    dotnet test --collect:"XPlat Code Coverage"

                    dotnet-sonarscanner end /d:sonar.login="${SONAR_TOKEN}"
                    '''
                }
            }
        }

        stage('OWASP Dependency Check') {
            steps {
                withCredentials([usernamePassword(credentialsId: 'owasp-cred', usernameVariable: 'OSSINDEX_USER', passwordVariable: 'OSSINDEX_TOKEN')]) {
                    dependencyCheck additionalArguments: '--scan ./', odcInstallation: 'owasp'
                    dependencyCheckPublisher pattern: '**/dependency-check-report.xml'
                }
            }
        }

        stage('Trivy Scan Source') {
            steps {
                sh '''
                trivy fs . --exit-code 0 --severity HIGH,CRITICAL --format table --scanners vuln > trivy-source-report.txt
                '''
            }
        }

        stage('Docker Build and Tag') {
            steps {
                withCredentials([usernamePassword(credentialsId: "${DOCKERHUB_CRED}", usernameVariable: 'DOCKER_USER', passwordVariable: 'DOCKER_PASS')]) {
                    sh '''
                    docker login -u "$DOCKER_USER" -p "$DOCKER_PASS"
                    docker build -t ${DOCKER_USER}/dotnet-proj:v3 .
                    '''
                }
            }
        }

        stage('Trivy Scan Image') {
            steps {
                sh '''
                trivy image ${DOCKER_USER}/dotnet-proj:v3 --exit-code 0 --severity HIGH,CRITICAL --format table > trivy-image-report.txt
                '''
            }
        }

        stage('Docker Push') {
            steps {
                withCredentials([usernamePassword(credentialsId: "${DOCKERHUB_CRED}", usernameVariable: 'DOCKER_USER', passwordVariable: 'DOCKER_PASS')]) {
                    sh '''
                    docker login -u "$DOCKER_USER" -p "$DOCKER_PASS"
                    docker push ${DOCKER_USER}/dotnet-proj:v3
                    '''
                }
            }
        }
    }   

    post {
        always {
            archiveArtifacts artifacts: '**/dependency-check-report.xml, trivy-*.txt', onlyIfSuccessful: true
            sh 'docker logout || true'
        }
    }
}
