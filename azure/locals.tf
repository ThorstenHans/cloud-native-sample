locals {
  default_tags = {
    "com.thinktecture.project" = "Cloud-Native Sample"
    "com.thinktecture.owner"   = "Thorsten Hans"
  }
  tags = merge(local.default_tags, var.custom_tags)
}
