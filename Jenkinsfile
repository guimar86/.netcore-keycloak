pipeline{
    agent any

    stages{
        stage("build"){
            steps{
                echo "Building docker file"
                docker-compose --quiet
            }
            
        }
    }
    
}