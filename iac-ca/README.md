IaC: Container Apps with some Pulumi (by Mateusz Świetlicki)
======================================

Azure Container Apps https://azure.microsoft.com/en-us/products/container-apps is a "new" Microsoft solution for "simple" K8s.

Here you will find how to create and publish dotnet app to Container Apps.

Setup Azure environment
-------------------------

Lets create resource group

    az group create -l westeurope -g workshop-ca-rg

Setup ContainerApps and Registry

    az containerapp env create -l westeurope -g workshop-ca-rg -n workshop-ca
    az acr create -g workshop-ca-rg -n workshopcaacr --sku Standard --admin-enabled
    $acrCredentials = az acr credential show -g workshop-ca-rg -n workshopcaacr | ConvertFrom-Json
    $registry_login = $acrCredentials.username
    $registry_pass = $acrCredentials.passwords[0].value


Building dotnet api
---------------------

    dotnet new sln
    
    dotnet new web -o src/iac-ca-api-int
    dotnet add src/iac-ca-api-int package Microsoft.NET.Build.Containers
    dotnet sln add src/iac-ca-api-int

    dotnet new web -o src/iac-ca-api-pub
    dotnet add src/iac-ca-api-pub package Microsoft.NET.Build.Containers
    dotnet sln add src/iac-ca-api-pub

Creating docker image
------------------------

https://learn.microsoft.com/en-us/dotnet/core/docker/publish-as-container


    dotnet add package Microsoft.NET.Build.Containers

    dotnet publish --os linux /t:PublishContainer -p:ContainerImageTag=latest -p:ContainerImageName=workshopcaacr.azurecr.io/iac-ca-api-pub
    docker run -it -p 80:80 iac-ca-api-pub

    # docker tag iac-ca-api-pub workshopcaacr.azurecr.io/iac-ca-api-pub
    docker push workshopcaacr.azurecr.io/iac-ca-api-pub

Publish out api as ContainerApp
------------------------------------

    az containerapp create -n iac-ca-api-pub -g workshop-ca-rg -i workshopcaacr.azurecr.io/iac-ca-api-pub --environment workshop-ca --ingress external --target-port 80 --registry-server workshopcaacr.azurecr.io --registry-username $registry_login --registry-password $registry_pass

Calling Int API via Dapr
-------------------------
https://docs.dapr.io/reference/api/service_invocation_api/

    var http = new HttpClient();
    return await http.GetStringAsync("http://localhost:3500/v1.0/invoke/iac-ca-api-int/method/hello");

Clean after ourselves
-------------------------

    az group delete -g workshop-ca-rg -y --no-wait

Setup Pulumi
---------------

    mkdir iac-ca-setup | cd
    pulumi new
    ...
    pulumi up -y
