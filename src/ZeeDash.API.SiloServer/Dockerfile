# To build this Dockerfile, run the following command from the solution directory:
# docker build --file "src\ZeeDash.API.SiloServer\Dockerfile" --tag orleans .
# Or you can use the experimental buildx command for a better experience:
# docker buildx build --progress plain --file "src\ZeeDash.API.SiloServer\Dockerfile" --tag orleans .

# Base image used by Visual Studio at development time
# (See https://docs.microsoft.com/en-us/visualstudio/containers/container-msbuild-properties)
FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
# Open Container Initiative (OCI) labels (See https://github.com/opencontainers/image-spec/blob/master/annotations.md).
LABEL org.opencontainers.image.title="ZeeDash.API" \
    org.opencontainers.image.description="API of the ZeeDash backend" \
    org.opencontainers.image.documentation="https://github.com/zeedash/ZeeDash.API" \
    org.opencontainers.image.source="https://github.com/zeedash/ZeeDash.API.git" \
    org.opencontainers.image.url="https://github.com/zeedash/ZeeDash.API" \
    org.opencontainers.image.vendor="Emilien GUILMINEAU"
# Disable the culture invariant mode which defaults to true in the base alpine image
# (See https://github.com/dotnet/corefx/blob/8245ee1e8f6063ccc7a3a60cafe821d29e85b02f/Documentation/architecture/globalization-invariant-mode.md)
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
RUN apk add --no-cache \
    # Install cultures to enable use of System.CultureInfo
    icu-libs \
    # Install time zone database to enable use of System.TimeZoneInfo
    tzdata
# Set the default locale and language.
ENV LC_ALL=en_US.UTF-8
ENV LANG=en_US.UTF-8
WORKDIR /app
EXPOSE 80 11111 30000

# SDK image used to build and publish the application
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS sdk
# To use the debug build configuration pass --build-arg Configuration=Debug
ARG Configuration=Release
ENV DOTNET_CLI_TELEMETRY_OPTOUT=true \
    DOTNET_SKIP_FIRST_TIME_EXPERIENCE=true
WORKDIR /src
COPY "ZeeDash.API.sln" "."
COPY "src/ZeeDash.API.Abstractions/*.csproj" "src/ZeeDash.API.Abstractions/"
COPY "src/ZeeDash.API.Client/*.csproj" "src/ZeeDash.API.Client/"
COPY "src/ZeeDash.API.Grains/*.csproj" "src/ZeeDash.API.Grains/"
COPY "src/ZeeDash.API.SiloServer/*.csproj" "src/ZeeDash.API.SiloServer/"
COPY "tests/ZeeDash.API.SiloServer.IntegrationTest/*.csproj" "tests/ZeeDash.API.SiloServer.IntegrationTest/"
# Run the restore and cache the packages on the host for faster subsequent builds.
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet restore
COPY . .
# To view the files that have been copied into the container file system for debugging purposes uncomment this line
# RUN apk add --no-cache tree && tree
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet build --configuration $Configuration --no-restore
RUN dotnet test --configuration $Configuration --no-build
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish "src/ZeeDash.API.SiloServer/ZeeDash.API.SiloServer.csproj" --configuration $Configuration --no-build --output /app

# Runtime image used to run the application
FROM base AS runtime
WORKDIR /app
COPY --from=sdk /app .
ENTRYPOINT ["dotnet", "ZeeDash.API.SiloServer.dll"]
