FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

COPY ./src/AssignmentService ./src/AssignmentService
COPY ./src/AssignmentService.Host ./src/AssignmentService.Host

RUN dotnet restore src/AssignmentService.Host/AssignmentService.Host.csproj
COPY . .
WORKDIR /src/src/AssignmentService.Host
RUN dotnet build AssignmentService.Host.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish AssignmentService.Host.csproj  -c Release -o /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS final
WORKDIR /app
COPY --from=publish /app .

ARG HOST_PORT=80

ENV ASPNETCORE_URLS=http://+:${HOST_PORT}

EXPOSE ${HOST_PORT}
ENTRYPOINT ["dotnet", "AssignmentService.Host.dll"]