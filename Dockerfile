FROM public.ecr.aws/lambda/dotnet:8 AS base
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ["src/Evalr.API/Evalr.API.csproj", "src/Evalr.API/"]
COPY ["src/Evalr.Core/Evalr.Core.csproj", "src/Evalr.Core/"]

RUN dotnet restore "src/Evalr.API/Evalr.API.csproj"

COPY . .

WORKDIR "/src/src/Evalr.API"
RUN dotnet build "Evalr.API.csproj" -c Release -o /app/build
RUN dotnet publish "Evalr.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM public.ecr.aws/lambda/dotnet:8

WORKDIR /var/task
COPY --from=build /app/publish .

ENTRYPOINT ["/lambda-entrypoint.sh"]
CMD ["Evalr.API::Evalr.API.LambdaHandler::HandleRequest"]