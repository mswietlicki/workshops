param(
    [Parameter(Mandatory=$True)][string] $Project,
    [Parameter(Mandatory=$True)][string] $ContainerImageName,
    [Parameter(Mandatory=$True)][string] $Registry,
    [Parameter(Mandatory=$True)][string] $RegistryUsername,
    [Parameter(Mandatory=$True)][string] $RegistryPassword
)

dotnet publish $Project --os linux /t:PublishContainer -p:ContainerImageTag=latest -p:ContainerImageName=$ContainerImageName
Write-Host "docker login $Registry -u $RegistryUsername -p $RegistryPassword"
docker login $Registry -u $RegistryUsername -p $RegistryPassword 2>$null
docker push $ContainerImageName