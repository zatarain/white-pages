# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base

ARG Postgres__Hostname
ARG Postgres__Database
ARG Postgres__Username
ARG Postgres__Password

ENV Postgres__Hostname=${Postgres__Hostname}
ENV Postgres__Database=${Postgres__Database}
ENV Postgres__Username=${Postgres__Username}
ENV Postgres__Password=${Postgres__Password}

WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ConsumerManager/ConsumerManager.csproj", "ConsumerManager/"]
RUN dotnet restore "ConsumerManager/ConsumerManager.csproj"
COPY . .
WORKDIR "/src/ConsumerManager"
# RUN dotnet dev-certs https && \
#     dotnet dev-certs https -ep /usr/local/share/ca-certificates/aspnet/https.crt --format PEM && \
#     update-ca-certificates
RUN dotnet build "ConsumerManager.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ConsumerManager.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ./entrypoint.sh .
CMD ["dotnet", "ConsumerManager.dll"]
ENTRYPOINT ["./entrypoint.sh"]
# ENTRYPOINT ["dotnet", "ConsumerManager.dll"]
