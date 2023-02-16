resource "helm_release" "grafana_loki" {
  name             = "loki"
  repository       = "https://grafana.github.io/helm-charts"
  chart            = "loki-distributed"
  namespace        = "loki"
  create_namespace = true

  values = [
    templatefile("${path.module}/manifests/loki-values.yml", {
      storage_account_name      = var.loki_storage_account_name
      user_assigned_identity_id = var.loki_user_assigned_identity_id
    }),
  ]
}
