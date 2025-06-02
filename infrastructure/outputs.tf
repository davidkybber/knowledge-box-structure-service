output "container_app_url" {
  description = "The URL of the deployed container app"
  value       = "https://${azurerm_container_app.structure_service.latest_revision_fqdn}"
}

output "container_app_name" {
  description = "The name of the container app"
  value       = azurerm_container_app.structure_service.name
}

output "container_app_environment_name" {
  description = "The name of the container app environment"
  value       = azurerm_container_app_environment.env.name
}

output "container_app_identity_principal_id" {
  description = "The principal ID of the container app's managed identity"
  value       = azurerm_container_app.structure_service.identity[0].principal_id
}

output "key_vault_id" {
  description = "The ID of the Key Vault"
  value       = data.azurerm_key_vault.key_vault.id
}

output "resource_group_name" {
  description = "The name of the resource group"
  value       = data.azurerm_resource_group.rg.name
}

output "acr_login_server" {
  description = "The login server URL for the Azure Container Registry"
  value       = var.acr_enabled ? data.azurerm_container_registry.acr[0].login_server : null
} 