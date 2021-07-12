
## Steps to Run Locally
1. Have docker installed
2. Have NuGet configured correctly (See the "NuGet Configuration" topic below).
2. Open a command prompt
3. `cd <REPO_ROOT>/integration-tests/docker-glossary-api`
4. `docker-compose up --force-recreate`
   * This is being run without the detached (-d) option so that it can be easier to stop. You can choose however you want to run it.
5. `cd <REPO_ROOT>/integration-tests`
6. `./bin/load-integration-data.sh` -- this loads the test data
7. `./bin/karate ./features` -- This runs the tests
   * `./bin/karate -w ./features` will watch the feature files and rerun when they are changed. So good for devving tests

## NuGet Configuraiton
1. Create a GitHub [Personal Access token](https://github.com/settings/tokens/) with a descriptive name such as "NuGet package".
2. Assign the token the `packages:read` scope and save it.
3. Copy the token's value.
4. If you have the dotnet command line tool installed, run this command (it's all one line), substituting your username and token value.
    ```bash
    dotnet nuget add source https://nuget.pkg.github.com/nciocpl/index.json --name github --username <YOUR_GITHUB_USERNAME> --password <THE_TOKEN_VALUE> --store-password-in-clear-text
    ```
5. If you do NOT have the dotnet command line tool installed:
    1. Create `~/.nuget/NuGet/NuGet.Config`.
    2. Put these lines in the file (be sure to substitute your username and toke value)
       ```xml
        <?xml version="1.0" encoding="utf-8"?>
        <configuration>
            <packageSources>
                <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
                <add key="github" value="https://nuget.pkg.github.com/nciocpl/index.json" />
            </packageSources>
            <packageSourceCredentials>
                <github>
                    <add key="Username" value="<YOUR_GITHUB_USERNAME>" />
                    <add key="ClearTextPassword" value="<THE_TOKEN_VALUE>" />
                </github>
            </packageSourceCredentials>
            <disabledPackageSources>
            </disabledPackageSources>
        </configuration>
       ```


## Notes
* [Docs for understanding how to run Karate standalone](https://github.com/intuit/karate/blob/6de466bdcf105d72450a40cf31b8adb5c043037d/karate-netty/README.md#standalone-jar)
   * Specifically this has to do with the magic naming of the logging config which is really why I am posting this here!
* We have docker for dev testing because ES will no longer run on higher Java versions, this is the easiest way to get it up and running.
* .NET running locally on a Mac cannot talk to ES because of how NEST always uses the host name to connect to ES and ES exposes the Virtual Machine's hostname/IP that runs Linux on the Mac.
* You need to use the `--force-recreate` option to `docker-compose up` or run `docker-compose rm` after shutting down the cluster. If the elasticsearch container is not removed, it keeps its data, and any restarts will leave the cluster in a bad state.

