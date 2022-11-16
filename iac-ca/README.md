


https://learn.microsoft.com/en-us/dotnet/core/docker/publish-as-container

Lets create resource group

    az group create -l westeurope -g workshop-ca

Setup ContainerApps and Registry

    az containerapp env create -l westeurope -g workshop-ca-rg -n workshop-ca
    az acr create -g workshop-ca -n workshopcaacr --sku Standard --admin-enabled
    az acr credential show -g workshop-ca -n workshopcaacr

Building dotnet app

    dotnet new sln
    mkdir src/iac-ca-api-pub | cd
    dotnet new webapi

Creating docker image

    dotnet add package Microsoft.NET.Build.Containers

    dotnet publish --os linux /t:PublishContainer -p:ContainerImageTag=latest -p:ContainerImageName=workshopcaacr.azurecr.io/iac-ca-api-pub
    docker run -it -p 80:80 iac-ca-api-pub

    docker tag iac-ca-api-pub workshopcaacr.azurecr.io/iac-ca-api-pub
    docker push workshopcaacr.azurecr.io/iac-ca-api-pub

Publish out api as ContainerApp

    az containerapp create -n iac-ca-api-pub -g workshop-ca -i workshopcaacr.azurecr.io/iac-ca-api-pub --environment workshop-ca --ingress external --target-port 80 --registry-server workshopcaacr.azurecr.io --registry-username $registry_login --registry-password $registry_pass

Clean after ourselves

    az group delete -g workshop-ca-rg -y --no-wait

Setup Pulumi

pulumi new



