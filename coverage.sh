#!/bin/sh -eu

# function to display commands
exe() { echo; echo "\$ $*" ; "$@" ; }

# Parameters
framework="${1-netcoreapp2.1}"
config="${2-Debug}"

include="[AutoTest.*]*"
exclude="\"[*.Tests]*,[AutoTest.ExampleLibrary]*\""

# Cannot use a bash solution in alpine builds https://stackoverflow.com/a/246128
#rootDir="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
rootDir=$(pwd)

testResults="test/TestResults"
output="$rootDir/$testResults/output"
tools="$rootDir/$testResults/tools"

testProj1="$rootDir/test/AutoTest.ExampleLibrary.Tests/AutoTest.ExampleLibrary.Tests.csproj"
testProj2="$rootDir/test/AutoTest.ArgumentNullException.Tests/AutoTest.ArgumentNullException.Tests.csproj"

# Restore the packages
exe dotnet restore "$rootDir"

# Build the test projects
exe dotnet build --no-restore -f "$framework" -c "$config" "$testProj1"
exe dotnet build --no-restore -f "$framework" -c "$config" "$testProj2"

# Execute the tests
exe dotnet test --no-restore --no-build -f "$framework" -c "$config" \
"$testProj1" \
--results-directory "$output/" \
--logger "\"trx;LogFileName=$(basename "$testProj1" .csproj).trx\"" \
--logger "\"Console;noprogress=true\"" \
/p:CollectCoverage=true \
/p:Include="$include" \
/p:Exclude="$exclude" \
/p:CoverletOutput="$output/internal.coverage.json"

exe dotnet test --no-restore --no-build -f "$framework" -c "$config" \
"$testProj2" \
--results-directory "$output/" \
--logger "\"trx;LogFileName=$(basename "$testProj2" .csproj).trx\"" \
--logger "\"Console;noprogress=true\"" \
/p:CollectCoverage=true \
/p:Include="$include" \
/p:Exclude="$exclude" \
/p:MergeWith="$output/internal.coverage.json" \
/p:CoverletOutput="$output/" \
/p:CoverletOutputFormat="\"json,opencover,cobertura\""

# Install trx2junit if not already installed
if [ ! -f "$tools/trx2junit" ]
then
   exe dotnet tool install trx2junit --tool-path "$tools"
fi

# Install ReportGenerator if not already installed
if [ ! -f "$tools/reportgenerator" ]
then
   exe dotnet tool install dotnet-reportgenerator-globaltool --tool-path "$tools"
fi

# Convert the MSTest trx files to junit xml
exe "$tools/trx2junit" "$output"/*.trx

# Generate the reports
exe "$tools/reportgenerator" \
"-verbosity:Info" \
"-reports:$output/coverage.opencover.xml" \
"-targetdir:$output/Report" \
"-reporttypes:Html"
