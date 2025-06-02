# Configure the Azure provider
terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 4.0"
    }
    random = {
      source  = "hashicorp/random"
      version = "~> 3.1"
    }
  }
  
  backend "azurerm" {
    resource_group_name  = "terraform-state-rg"
    storage_account_name = "knowledgeboxstate"
    container_name       = "structure-service-tfstate"
    key                  = "knowledge-box-structure.terraform.tfstate"
  }
}

provider "azurerm" {
  subscription_id = "de7e4d63-7e50-4def-8472-18a4de0ccb95"
  features {}
}

provider "random" {
} 