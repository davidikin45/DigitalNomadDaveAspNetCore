import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common'
import { HttpClient } from '@angular/common/http/src/client';

import { RouterModule, Routes } from '@angular/router';
import { PageNotFoundComponent } from './not-found.component';

import { GlobalErrorHandler } from './shared/global-error-handler';
import { ErrorLoggerService } from './shared/error-logger.service';
import { HandleHttpErrorInterceptor } from './shared/handle-http-error-interceptor';
import { WriteOutJsonInterceptor } from './shared/write-out-json-interceptor';
import { EnsureAcceptHeaderInterceptor } from './shared/ensure-accept-header-interceptor';
import { OpenIdConnectService } from './shared/open-id-connect.service';
import { SigninOidcComponent } from './signin-oidc/signin-oidc.component';
import { RequireAuthenticatedUserRouteGuardService } from './shared/require-authenticated-user-route-guard.service';
import { AddAuthorizationHeaderInterceptor } from './shared/add-authorization-header-interceptor';
import { RedirectSilentRenewComponent } from './redirect-silent-renew/redirect-silent-renew.component';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { NavSidebarComponent } from './nav-sidebar/nav-sidebar.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';

import { AboutComponent } from './about';
import { AuthorsComponent, AuthorDetailComponent, AuthorUpdateComponent, AuthorAddComponent } from './authors';
import { AuthorService } from './authors/shared/author.service';
import { MasterDataService } from './shared/master-data.service';

const appRoutes: Routes = [
  { path: '', component: HomeComponent, pathMatch: 'full', canActivate: [RequireAuthenticatedUserRouteGuardService] },

  { path: 'about', component: AboutComponent },

  { path: 'authors', component: AuthorsComponent, canActivate: [RequireAuthenticatedUserRouteGuardService] },
  { path: 'authors/:id', component: AuthorDetailComponent, canActivate: [RequireAuthenticatedUserRouteGuardService]},
  { path: 'author-update/:id', component: AuthorUpdateComponent, canActivate: [RequireAuthenticatedUserRouteGuardService]},
  { path: 'author-add', component: AuthorAddComponent, canActivate: [RequireAuthenticatedUserRouteGuardService]},

  { path: 'counter', component: CounterComponent, canActivate: [RequireAuthenticatedUserRouteGuardService] },
  { path: 'fetch-data', component: FetchDataComponent, canActivate: [RequireAuthenticatedUserRouteGuardService] },
  { path: 'signin-oidc', component: SigninOidcComponent },
  { path: 'redirect-silentrenew', component: RedirectSilentRenewComponent },
  { path: '**', component: PageNotFoundComponent }
];

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    NavSidebarComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent,
    SigninOidcComponent,
    AboutComponent,
    AuthorsComponent,
    AuthorDetailComponent,
    AuthorAddComponent,
    AuthorUpdateComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot(appRoutes),
    ReactiveFormsModule
  ],
  providers: [
    {
    provide: HTTP_INTERCEPTORS,
    useClass: AddAuthorizationHeaderInterceptor,
    multi: true
  },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: EnsureAcceptHeaderInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: WriteOutJsonInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HandleHttpErrorInterceptor,
      multi: true,
    },
    GlobalErrorHandler, ErrorLoggerService, AuthorService, MasterDataService, DatePipe, OpenIdConnectService,
    RequireAuthenticatedUserRouteGuardService],
  bootstrap: [AppComponent]
})
export class AppModule {

  constructor() {
    automapper.createMap('AuthorFormModel', 'AuthorForCreation');
    automapper.createMap('AuthorFormModel', 'AuthorForUpdate');
  }
}
