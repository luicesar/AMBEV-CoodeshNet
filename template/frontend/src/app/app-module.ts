import { NgModule, provideBrowserGlobalErrorListeners } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';

import { AppRoutingModule } from './app-routing-module';
import { App } from './app';

import { JwtInterceptor } from './core/interceptors/jwt.interceptor';
import { Login } from './features/auth/login/login';
import { SalesList } from './features/sales/sales-list/sales-list';
import { SaleForm } from './features/sales/sale-form/sale-form';
import { SaleDetail } from './features/sales/sale-detail/sale-detail';

@NgModule({
  declarations: [
    App,
    Login,
    SalesList,
    SaleForm,
    SaleDetail
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    HttpClientModule
  ],
  providers: [
    provideBrowserGlobalErrorListeners(),
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true }
  ],
  bootstrap: [App]
})
export class AppModule { }
