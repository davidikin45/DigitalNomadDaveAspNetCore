// This file can be replaced during build by using the `fileReplacements` array.
// `ng build ---prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: false,
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

/*
 * In development mode, to ignore zone related error stack frames such as
 * `zone.run`, `zoneDelegate.invokeTask` for easier debugging, you can
 * import the following file, but please comment it out in production mode
 * because it will have performance impact when throw error
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
