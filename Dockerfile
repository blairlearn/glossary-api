# Build me with the following command:
#   docker build --tag glossary-api --secret "id=GITHUB_USERNAME,env=GITHUB_USERNAME"  --secret "id=GITHUB_TOKEN,env=GITHUB_TOKEN" .


# Build the executable
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

# ARG GITHUB_USERNAME
# ARG GITHUB_TOKEN

RUN --mount=type=secret,id=GITHUB_USERNAME,env=GITHUB_USERNAME \
    --mount=type=secret,id=GITHUB_TOKEN,env=GITHUB_TOKEN \
    dotnet nuget add source https://nuget.pkg.github.com/nciocpl/index.json \
      --name github \
      --username ${GITHUB_USERNAME} \
      --password ${GITHUB_TOKEN} \
      --store-password-in-clear-text

COPY ./src /app/src
COPY ./test /app/test
COPY ./glossary-api.sln /app/app.sln
WORKDIR /app
RUN dotnet publish src/NCI.OCPL.Api.Glossary -p:DebugType=None -p:DebugSymbols=false -c Release -o /app/out

# Get the NIH Root certificates and install them so we work on VPN
# Download from https://ocio.nih.gov/Smartcard/Pages/PKI_chain.aspx
RUN mkdir /usr/local/share/ca-certificates/nih \
  && curl -o /usr/local/share/ca-certificates/nih/NIH-DPKI-ROOT-1A-base64.crt https://ocio.nih.gov/Smartcard/Documents/Certificates/NIH-DPKI-ROOT-1A-base64.cer \
  && curl -o /usr/local/share/ca-certificates/nih/NIH-DPKI-CA-1A-base64.crt https://ocio.nih.gov/Smartcard/Documents/Certificates/NIH-DPKI-CA-1A-base64.cer \
  && update-ca-certificates

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app


COPY --from=build-env /app/out .
COPY --from=build-env /usr/local/share/ca-certificates/nih /usr/local/share/ca-certificates/nih

## This does not need to wait for the loader or other resources.
## Any integration tests will need to wait for the API to report being healthy
EXPOSE 5001
ENV ASPNETCORE_URLS="http://*:5000"
ENTRYPOINT ["dotnet", "/app/NCI.OCPL.Api.Glossary.dll"]
