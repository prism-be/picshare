docker build . --file Prism.Picshare.Services.Mailing/Dockerfile --tag simonbaudart/picshare:service-mailing-latest --build-arg NUGET_AUTH_TOKEN=%1