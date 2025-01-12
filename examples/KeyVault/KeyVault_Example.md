﻿# How To Run KeyVault.Tests.ps1

`KeyVault.Tests.ps1` contains examples of using the `Confirm-AzBPKeyVault`, `Confirm-AzBPKeyVaultKey`,
and `Confirm-AzBPKeyVaultSecret` cmdlets.

## Pre-Requisites

- Follow the [setup instructions](../README.md)

## Steps

1. Navigate to KeyVault directory:

   ```Powershell
   cd examples\KeyVault\
   ```

1. Deploy the Key Vault to your resource group:

   ```Powershell
    New-AzResourceGroupDeployment -ResourceGroupName "<your-resource-group-name>"`
    -TemplateFile ".\keyVault.bicep"
   ```

1. When prompted in your terminal, set the `svcPrincipalObjectId` parameter with your service principal's
   enterprise/managed application object ID.

1. Update `KeyVault.Tests.ps1` variables to point to your expected resources:

   - `rg-test`          -> `your-resource-group-name`
   - `kvbenchpresstest` -> `your-key-vault-name`
   - `samplecert`       -> `your-certifcate-name`
   - `westus3`          -> `your-key-vault-location`
   - `svcprinoid`       -> `your-svc-principal-object-id (the same object id from step 3)`

1. If using a local copy of `Az.InfrastructureTesting`, replace `Import-Module Az.InfrastructureTesting` with
`Import-Module "../../bin/BenchPress.Azure.psd1"`. Note that the final `AfterAll` step will properly remove the module
regardless of which method is chosen to load the module.

1. Run `KeyVault.Tests.ps1`:

   ```Powershell
   Invoke-Pester -Path .\KeyVault.Tests.ps1
   ```

1. Success!

   ```Powershell
   Tests completed in 3.03s
   Tests Passed: 4, Failed: 0, Skipped: 0 NotRun: 0
   ```

1. Don't forget to delete any deployed resources that are no longer needed.
