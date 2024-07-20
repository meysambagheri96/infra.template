set -e
VERSION=`git describe --tags` && echo "##vso[task.setvariable variable=VERSION_TAG]$VERSION_TAG"
PUBS_DIR="pubs"
IMAGE_TAG=$IMAGE_ID:$V

echo "APPYING PUBENV: $PUB_ENV VERSION: $V IMAGE_TAG: $IMAGE_TAG Release: $RELEASE"
SETTING_OVERRIDE=" -f values.$PUB_ENV.yaml"

echo $SETTING_OVERRIDE

cd $PROJ_DIR
mkdir $PUBS_DIR

echo "publishing the app"
ls
dotnet publish -o $PUBS_DIR -c Release --source https://api.nuget.org/v3/index.json --source http://nexus.local/repository/nuget-hosted/ 

cd $PUBS_DIR
ls 
echo "Creating docker images for $IMAGE_TAG"

docker build -t $IMAGE_TAG . 
docker push $IMAGE_TAG

echo "Image pushed to repository $IMAGE_TAG"

cd ..
cd helm


helm upgrade $RELEASE . --set env.ASPNETCORE_ENVIRONMENT=$PUB_ENV --set image.tag=${V} $SETTING_OVERRIDE --install --wait --atomic --create-namespace -n $NAMESPACE
