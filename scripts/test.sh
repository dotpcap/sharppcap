#!/bin/bash

set -e

TEST_ARGS=("$@")

TEST_ARGS+=( -p:CollectCoverage=true )
TEST_ARGS+=( --blame-crash )

# select logger based on CI server

if [ -n "$APPVEYOR" ] # AppVeyor
then
    TEST_ARGS+=( --logger:Appveyor --test-adapter-path:. )
elif [ -n "$SYSTEM_TEAMFOUNDATIONCOLLECTIONURI" ] # Azure Pipelines
then
	TEST_ARGS+=( --logger:trx )
else
    TEST_ARGS+=( --logger:junit --test-adapter-path:. )
fi

dotnet test "${TEST_ARGS[@]}"

# coverage

bash $(dirname $0)/codecov.sh -f '**/*.opencover.xml'
