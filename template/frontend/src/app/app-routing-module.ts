import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './core/guards/auth.guard';
import { Login } from './features/auth/login/login';
import { SalesList } from './features/sales/sales-list/sales-list';
import { SaleForm } from './features/sales/sale-form/sale-form';
import { SaleDetail } from './features/sales/sale-detail/sale-detail';

const routes: Routes = [
  { path: 'login', component: Login },
  { path: 'sales', component: SalesList, canActivate: [AuthGuard] },
  { path: 'sales/new', component: SaleForm, canActivate: [AuthGuard] },
  { path: 'sales/:id/edit', component: SaleForm, canActivate: [AuthGuard] },
  { path: 'sales/:id', component: SaleDetail, canActivate: [AuthGuard] },
  { path: '', redirectTo: '/sales', pathMatch: 'full' },
  { path: '**', redirectTo: '/sales' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
