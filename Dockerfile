FROM amazonlinux:2023 AS build

RUN dnf install -y wget tar gzip findutils icu && \
    wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh && \
    chmod +x dotnet-install.sh && \
    ./dotnet-install.sh --channel 8.0 --install-dir /usr/share/dotnet && \
    ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet && \
    dnf clean all

WORKDIR /src

COPY ["src/Evalr.API/Evalr.API.csproj", "src/Evalr.API/"]
COPY ["src/Evalr.Core/Evalr.Core.csproj", "src/Evalr.Core/"]

RUN dotnet restore "src/Evalr.API/Evalr.API.csproj"

COPY . .

WORKDIR "/src/src/Evalr.API"
RUN dotnet build "Evalr.API.csproj" -c Release -o /app/build
RUN dotnet publish "Evalr.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM public.ecr.aws/lambda/dotnet:8 AS runtime

WORKDIR /var/task
COPY --from=build /app/publish .

ENTRYPOINT ["/lambda-entrypoint.sh"]
CMD ["Evalr.API::Evalr.API.LambdaHandler::HandleRequest"]