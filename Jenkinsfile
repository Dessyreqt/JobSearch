pipeline {
	agent any
	stages {
		stage('Checkout') {
			steps {
				checkout scm
			}
		}

		stage('Update Database') {
			steps {
				bat 'build UpdateDatabase'
				bat 'build UpdateTestDatabase'
			}
		}

		stage('Build') {
			steps {
				bat 'build Clean'
				bat 'build Compile'
			}
		}

		stage('Tests') {
			steps {
				bat 'build Tests'
			}
		}

		stage('Deploy') {
			when {
				branch 'master'
			}
			steps {
				bat 'build Deploy'
			}
		}
	}
}
