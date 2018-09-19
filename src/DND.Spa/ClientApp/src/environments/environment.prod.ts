export const environment = {
  production: true,
  apiUrl: "https://localhost:44372/api/",
  openIdConnectSettings: {
    authority: 'https://localhost:44318',
    client_id: 'spa',
    redirect_uri: 'https://localhost:44332/signin-oidc',
    scope: 'openid profile roles api.full',
    response_type: 'id_token token',
    post_logout_redirect_uri: 'https://localhost:44332/',
    automaticSilentRenew: true,
    silent_redirect_uri: 'https://localhost:44332/redirect-silentrenew'
  }
};
