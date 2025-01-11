whoami

echo "APPYING PUBENV: $PUB_ENV VERSION: $V Release: $RELEASE"

cd $PROJ_DIR
ls
pwd

echo "publishing the app"
dotnet test
