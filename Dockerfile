FROM mcr.microsoft.com/dotnet/sdk:8.0 AS project
WORKDIR /app
EXPOSE 21

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS publish
WORKDIR /src

COPY . .

RUN dotnet restore
RUN dotnet publish -c Release -o output

FROM project AS final
WORKDIR /app
COPY --from=publish ./src/output .

ENTRYPOINT ["dotnet", "FtpServer.dll"]
