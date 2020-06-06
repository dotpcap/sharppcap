#!/bin/bash

set -e

TEST_ARGS=("$@")

TEST_ARGS+=( -p:CollectCoverage=true )

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

CODECOV_ARGS=( -f '**\*.opencover.xml' )
if [ -n "$SYSTEM_JOBDISPLAYNAME" ]
then
    CODECOV_ARGS+=( -F "$SYSTEM_JOBDISPLAYNAME" )
fi

bash <(curl -s https://codecov.io/bash) "${CODECOV_ARGS[@]}"
