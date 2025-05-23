# Build runtime image
FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /app

# Get the NIH Root certificates and install them so we work on VPN
# Download from https://ocio.nih.gov/Smartcard/Pages/PKI_chain.aspx
RUN mkdir /usr/local/share/ca-certificates/nih \
    && curl -o /usr/local/share/ca-certificates/nih/NIH-DPKI-ROOT-1A-base64.crt https://ocio.nih.gov/Smartcard/Documents/Certificates/NIH-DPKI-ROOT-1A-base64.cer \
    && curl -o /usr/local/share/ca-certificates/nih/NIH-DPKI-CA-1A-base64.crt https://ocio.nih.gov/Smartcard/Documents/Certificates/NIH-DPKI-CA-1A-base64.cer \
    && update-ca-certificates

## This does not need to wait for the loader or other resources.
## Any integration tests will need to wait for the API to report being healthy
EXPOSE 5001
ENV ASPNETCORE_ENVIRONMENT=inttest
ENV ASPNETCORE_URLS="http://*:5000"
ENTRYPOINT ["dotnet", "watch", "--project", "src/NCI.OCPL.Api.Glossary", "run"]