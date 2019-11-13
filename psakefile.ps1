properties {
    $base_dir = resolve-path .

    $db_dir = "$base_dir\db"
	$db_server = if ($env:db_server) { $env:db_server } else { "localhost" }
	$db_name = if ($env:db_name) { $env:db_name } else { "JobSearch" }
	$db_test_name = if ($env:db_test_name) { $env:db_test_name } else { "$db_name.Tests" }

    $api_name = "JobSearch"
    $api_dir = "$base_dir\api"
	$api_path = "$api_dir\$api_name.sln"
	$api_project_dir = "$api_dir\$api_name"
	$api_test_dir = "$api_dir\$api_name.Tests"
	$api_test_runtime_id = "win10-x86"
	$api_test_exe_dir = "$api_test_dir\bin\Debug\netcoreapp3.0\$api_test_runtime_id"
    $api_test_exe_path = "$api_test_exe_dir\$api_name.Tests.exe"
	$api_coverage_report_html_path = "$api_dir\CoverageReports\CoverageReport.html"
	$api_coverage_report_xml_path = "$api_dir\CoverageReports\CoverageReport.xml"
	$api_coverage_percent_minimum = 0
	$api_app_pool_name = "jobsearchapi.dscarroll.com"
	$api_publish_location = "C:\Sites\$api_app_pool_name"

	$roundhouse_dir = "$base_dir\tools\roundhouse"
	$roundhouse_exe_path = "$roundhouse_dir\rh.exe"

	$dotcover_dir = "$base_dir\tools\dotCover"
	$dotcover_exe_path = "$dotcover_dir\dotCover.exe"

	$db_object_dir = "$base_dir\tools\DbObject"
	$db_object_exe_path = "$db_object_dir\DbObject.exe"
}

#these tasks are for developers to run
task default -depends UpdateDatabase, UpdateTestDatabase, Clean, Compile, Tests
task coverage -depends CoverageReport
task dbobjs -depends GenerateDbObjects

task UpdateDatabase {
    exec { & $roundhouse_exe_path /d=$db_name /f=$db_dir /s=$db_server /silent /transaction }
}

task UpdateTestDatabase {
    exec { & $roundhouse_exe_path /d=$db_test_name /f=$db_dir /s=$db_server /silent /transaction }
}

task Clean {
    exec { & dotnet clean $api_path }
}

task Compile {
    exec { & dotnet build $api_path }
}

task Tests {
	Push-Location -Path $api_test_dir
	exec { & dotnet fixie --no-build } 
	Pop-Location
}

task CoverageReport {
	Push-Location -Path $api_test_dir
	exec { & dotnet restore } 
	exec { & dotnet publish -c Debug -r $api_test_runtime_id } 
	Pop-Location
	exec { & $dotcover_exe_path cover /TargetExecutable=$api_test_exe_path /Output="$api_coverage_report_html_path" /ReportType="HTML" /Filters="+:type=$api_name.Features.*;-:type=*Controller" } 
    exec { & $api_coverage_report_html_path }
}

task CICoverageReport {
	Push-Location -Path $api_test_dir
	exec { & dotnet restore } 
	exec { & dotnet publish -c Debug -r $api_test_runtime_id } 
	Pop-Location
	exec { & $dotcover_exe_path cover /TargetExecutable=$api_test_exe_path /Output="$api_coverage_report_xml_path" /ReportType="XML" /Filters="+:type=$api_name.Features.*;-:type=*Controller" } 
	$xml = [xml](Get-Content $api_coverage_report_xml_path)
	$coveragePercent = [int](($xml.Root | Select-Object CoveragePercent).CoveragePercent)
	if ($coveragePercent -lt $api_coverage_percent_minimum) {
		throw "Code coverage is $coveragePercent%. Minimum allowed is $api_coverage_percent_minimum%."
	}
	Write-Host "$coveragePercent% code coverage!"
}

task GenerateDbObjects {
	exec { & $db_object_exe_path baseclass --force --namespace "$api_name.Domain" --path "$api_project_dir\Domain" }
	exec { & $db_object_exe_path class --force --namespace "$api_name.Domain" --path "$api_project_dir\Domain" --connectionString "Server=$db_server;Database=$db_name;Integrated Security=true" }
}

task StopApiAppPool {
	Stop-WebAppPool $deploy_app_pool_name
}

task DeployApi {
	Push-Location $api_project_dir
	exec { & dotnet publish -c Release -o $api_publish_location }
	Pop-Location
}

task StartApiAppPool{
	Start-WebAppPool $api_app_pool_name
}
