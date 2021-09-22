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

CODECOV_ARGS=( -f '**/*.opencover.xml' )
if [ -n "$SYSTEM_JOBDISPLAYNAME" ]
then
    CODECOV_ARGS+=( --flag "$SYSTEM_JOBDISPLAYNAME" )
fi

if [ -n "$SYSTEM_PULLREQUEST_SOURCECOMMITID" ] # Azure Pipelines
then
    CODECOV_ARGS+=( --sha "$SYSTEM_PULLREQUEST_SOURCECOMMITID" )
    CODECOV_ARGS+=( --branch "$SYSTEM_PULLREQUEST_SOURCEBRANCH" )
fi

# Depending on CI, dotnet tool or bash should be used
# This is temporary until codecov fixes CI detection in the dotnet tool

if [ -n "$SYSTEM_TEAMFOUNDATIONCOLLECTIONURI"  ]
then
    dotnet tool restore
    dotnet codecov ${CODECOV_ARGS[@]}
else
    bash <(curl -s https://codecov.io/bash) "${CODECOV_ARGS[@]}"
fi
