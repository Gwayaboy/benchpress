﻿BeforeAll {
  . $PSScriptRoot/../../Public/Confirm-ContainerRegistry.ps1
  . $PSScriptRoot/../../Private/Connect-Account.ps1
  Import-Module Az
}

Describe "Confirm-ContainerRegistry" {
  Context "unit tests" -Tag "Unit" {
    BeforeEach {
      Mock Connect-Account{}
    }

    It "Calls Get-AzContainerRegistry" {
      Mock Get-AzContainerRegistry{}
      Confirm-ContainerRegistry -Name "cr" -ResourceGroupName "rgn"
      Should -Invoke -CommandName "Get-AzContainerRegistry" -Times 1
    }

    It "Sets the ErrorRecord when an exception is thrown" {
      Mock Get-AzContainerRegistry{ throw [Exception]::new("Exception") }
      $Results = Confirm-ContainerRegistry -Name "cr" -ResourceGroupName "rgn"
      $Results.ErrorRecord | Should -Not -Be $null
    }
  }
}

AfterAll {
  Remove-Module Az
}
