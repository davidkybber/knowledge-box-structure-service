# Reference existing resource group
data "azurerm_resource_group" "rg" {
  name = "knowledge-box-rg"
}

# Azure Container Registry - use existing one
data "azurerm_container_registry" "acr" {
  count               = var.acr_enabled ? 1 : 0
  name                = var.acr_name
  resource_group_name = data.azurerm_resource_group.rg.name
}

# Container Apps Environment
resource "azurerm_container_app_environment" "env" {
  name                = "${var.project_name}-structure-env"
  location            = data.azurerm_resource_group.rg.location
  resource_group_name = data.azurerm_resource_group.rg.name
  tags                = var.tags
}

# Container App for Structure Service
resource "azurerm_container_app" "structure_service" {
  name                         = "${var.project_name}-structure-service"
  container_app_environment_id = azurerm_container_app_environment.env.id
  resource_group_name         = data.azurerm_resource_group.rg.name
  revision_mode               = "Single"
  tags                        = var.tags

  identity {
    type = "SystemAssigned"
  }

  template {
    container {
      name   = "structure-service"
      image  = "mcr.microsoft.com/azuredocs/containerapps-helloworld:latest"
      cpu    = 0.25
      memory = "0.5Gi"

      env {
        name  = "NODE_ENV"
        value = "production"
      }

      env {
        name  = "PORT"
        value = "8080"
      }

      # JWT Authentication settings
      env {
        name        = "Jwt__Key"
        secret_name = "jwt-key"
      }

      # CORS Configuration
      env {
        name  = "CORS_ALLOWED_ORIGINS"
        value = "https://ashy-island-0ba215203.6.azurestaticapps.net"
      }

      # Database connection string
      env {
        name        = "ConnectionStrings__DefaultConnection"
        secret_name = "db-connection-string"
      }
    }
  }

  dynamic "secret" {
    for_each = var.acr_admin_enabled && var.acr_enabled ? [1] : []
    content {
      name  = "acr-password"
      value = data.azurerm_container_registry.acr[0].admin_password
    }
  }

  secret {
    name  = "jwt-key"
    value = data.azurerm_key_vault_secret.jwt_key.value
  }

  secret {
    name  = "db-connection-string"
    value = data.azurerm_key_vault_secret.db_connection_string.value
  }

  dynamic "registry" {
    for_each = var.acr_enabled ? [1] : []
    content {
      server               = "${var.acr_name}.azurecr.io"
      username             = var.acr_admin_enabled ? var.acr_name : null
      password_secret_name = var.acr_admin_enabled ? "acr-password" : null
    }
  }

  ingress {
    external_enabled = true
    target_port      = 8080
    traffic_weight {
      percentage      = 100
      latest_revision = true
    }
    allow_insecure_connections = false
  }
}

# Grant ACR pull access to the Container App's managed identity
resource "azurerm_role_assignment" "acr_pull" {
  count                = var.acr_enabled ? 1 : 0
  scope                = data.azurerm_container_registry.acr[0].id
  role_definition_name = "AcrPull"
  principal_id         = azurerm_container_app.structure_service.identity[0].principal_id
}

# Grant ACR pull access to the deployment service principal
#resource "azurerm_role_assignment" "acr_pull_deployment" {
  #count                = var.acr_enabled ? 1 : 0
  #scope                = data.azurerm_container_registry.acr[0].id
  #role_definition_name = "AcrPull"
  #principal_id         = var.deployment_sp_object_id
#}

# Get current Azure configuration
data "azurerm_client_config" "current" {}

# Reference existing Key Vault
data "azurerm_key_vault" "key_vault" {
  name                = "knowledge-box-auth-kv"
  resource_group_name = data.azurerm_resource_group.rg.name
}

# Key Vault Access Policy for Container App
resource "azurerm_key_vault_access_policy" "container_app_access" {
  key_vault_id = data.azurerm_key_vault.key_vault.id
  tenant_id    = data.azurerm_client_config.current.tenant_id
  object_id    = azurerm_container_app.structure_service.identity[0].principal_id

  secret_permissions = [
    "Get", "List"
  ]
}

# Reference existing Key Vault secrets
data "azurerm_key_vault_secret" "jwt_key" {
  name         = "jwt-key"
  key_vault_id = data.azurerm_key_vault.key_vault.id
}

data "azurerm_key_vault_secret" "db_connection_string" {
  name         = "db-connection-string"
  key_vault_id = data.azurerm_key_vault.key_vault.id
}
