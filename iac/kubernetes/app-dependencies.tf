resource "helm_release" "rabbitmq" {
  name       = "rabbit"
  repository = "https://charts.bitnami.com/bitnami"
  chart      = "rabbitmq"
  timeout    = 600
  // this must be deployed to the same namespace as the the dapr components and
  // the workloads that use that particular dapr component
  namespace        = "rabbitmq"
  create_namespace = true
}

resource "helm_release" "dapr" {
  name             = "dapr"
  repository       = "https://dapr.github.io/helm-charts/"
  chart            = "dapr"
  timeout          = 600
  namespace        = "dapr-system"
  create_namespace = true
}