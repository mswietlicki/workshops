using Pulumi;
using Pulumi.AzureNative.Resources;
using System.Collections.Generic;

using App = Pulumi.AzureNative.App;
using ContainerRegistry = Pulumi.AzureNative.ContainerRegistry;
using OperationalInsights = Pulumi.AzureNative.OperationalInsights;
using Local = Pulumi.Command.Local;

return await Pulumi.Deployment.RunAsync(() =>
{
    var resourceGroup = new ResourceGroup("resourceGroup", new ResourceGroupArgs
    {
        ResourceGroupName = "workshop-ca-rg"
    });

    var registry = new ContainerRegistry.Registry("registry", new ContainerRegistry.RegistryArgs
    {
        AdminUserEnabled = true,
        RegistryName = "workshopcaacr",
        ResourceGroupName = resourceGroup.Name,
        Sku = new ContainerRegistry.Inputs.SkuArgs
        {
            Name = "Standard",
        }
    });

    var registryCredentials = ContainerRegistry.GetRegistryCredentials.Invoke(new ContainerRegistry.GetRegistryCredentialsInvokeArgs
    {
        ResourceGroupName = resourceGroup.Name,
        RegistryName = registry.Name
    });

    var workspace = new OperationalInsights.Workspace("loganalytics", new OperationalInsights.WorkspaceArgs
    {
        ResourceGroupName = resourceGroup.Name,
        WorkspaceName = "workshop-ca-log",
        Sku = new OperationalInsights.Inputs.WorkspaceSkuArgs
        {
            Name = "PerGB2018",
        }
    });

    var workspaceSharedKeys = OperationalInsights.GetSharedKeys.Invoke(new OperationalInsights.GetSharedKeysInvokeArgs
    {
        ResourceGroupName = resourceGroup.Name,
        WorkspaceName = workspace.Name,
    });

    var environment = new App.ManagedEnvironment("environment", new App.ManagedEnvironmentArgs
    {
        EnvironmentName = "workshop-ca",
        ResourceGroupName = resourceGroup.Name,
        AppLogsConfiguration = new App.Inputs.AppLogsConfigurationArgs
        {
            Destination = "log-analytics",
            LogAnalyticsConfiguration = new App.Inputs.LogAnalyticsConfigurationArgs
            {
                CustomerId = workspace.CustomerId,
                SharedKey = workspaceSharedKeys.Apply(_ => _.PrimarySharedKey).Apply(Output.Create),
            },
        },
    });


    var appName = "iac-ca-api-pub";
    var pubApiPath = "../src/iac-ca-api-pub/";
    var containerImageName = Output.Format($"{registry.LoginServer}/iac-ca-api-pub");
    var registryUsername = registryCredentials.Apply(creds => creds.Username).Apply(Output.Create);
    var registryPassword = registryCredentials.Apply(creds => creds.Password).Apply(Output.Create);

    var dotnetPublish = new Local.Command("publish-api-image", new Local.CommandArgs
    {
        Create = Output.Format($"./Publish-DotnetContainer.ps1 -Project {pubApiPath} -ContainerImageName {containerImageName} -Registry {registry.LoginServer} -RegistryUsername {registryUsername} -RegistryPassword {registryPassword}"),
        Interpreter = new[] { "pwsh", "-nol", "-nop", "-c" }
    });

    var app = new App.ContainerApp(appName, new App.ContainerAppArgs
    {
        Configuration = new App.Inputs.ConfigurationArgs
        {
            Dapr = new App.Inputs.DaprArgs
            {
                AppPort = 80,
                AppProtocol = "http",
                Enabled = true,
                AppId = appName
            },
            Ingress = new App.Inputs.IngressArgs
            {
                External = true,
                TargetPort = 80,
                AllowInsecure = true
            },
            Registries =
            {
                new App.Inputs.RegistryCredentialsArgs
                {
                    Server = registry.LoginServer,
                    Username = registryUsername,
                    PasswordSecretRef = "registry-pwd"
                }
            },
            Secrets = new InputList<App.Inputs.SecretArgs>
            {
                new App.Inputs.SecretArgs {
                    Name = "registry-pwd",
                    Value = registryPassword
                }
            }
        },
        ContainerAppName = appName,
        ManagedEnvironmentId = environment.Id,
        ResourceGroupName = resourceGroup.Name,
        Template = new App.Inputs.TemplateArgs
        {
            Containers =
            {
                new App.Inputs.ContainerArgs
                {
                    Image = containerImageName,
                    Name = appName,
                    Env = {}
                },
            }
        },
    }, new CustomResourceOptions
    {
        DependsOn = dotnetPublish
    });

    // Export the primary key of the Storage Account
    return new Dictionary<string, object?>
    {
        ["ResourceGroupName"] = resourceGroup.Name,
        ["RegistryName"] = registry.LoginServer,
        ["RegistryUsername"] = registryCredentials.Apply(creds => creds.Username).Apply(Output.Create),
        ["RegistryPassword"] = registryCredentials.Apply(creds => creds.Password).Apply(Output.CreateSecret),
        ["EnvironmentName"] = environment.Name,
        ["ApiPub"] = app.LatestRevisionFqdn
    };
});