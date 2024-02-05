helm upgrade instana-agent \
   --repo https://agents.instana.io/helm \
   --namespace instana-agent \
   --create-namespace \
   --set agent.key=m2VxAzQJRUWvpTYEqnltvA \
   --set agent.downloadKey=m2VxAzQJRUWvpTYEqnltvA \
   --set agent.endpointHost=ingress-coral-saas.instana.io \
   --set agent.endpointPort=443 \
   --set cluster.name='my-instnet-cluster' \
   --set zone.name='Agent zone777' \
  --set opentelemetry.enabled=true \
  --set opentelemetry.grpc.enabled=true \
  --set opentelemetry.http.enabled=true \
   instana-agent