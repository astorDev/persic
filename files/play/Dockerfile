FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN cd play && dotnet publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
ARG VERSION
ENV VERSION=$VERSION
WORKDIR /app
COPY --from=build /src/play/bin/Release/net9.0/publish .
ENTRYPOINT [ "dotnet", "Persic.Files.Playground.dll" ]