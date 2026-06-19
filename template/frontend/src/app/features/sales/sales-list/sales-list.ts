import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { SaleService } from '../../../core/services/sale.service';
import { AuthService } from '../../../core/services/auth.service';
import { SaleListItem } from '../../../core/models/sale.model';

@Component({
  selector: 'app-sales-list',
  templateUrl: './sales-list.html',
  standalone: false,
  styleUrl: './sales-list.scss'
})
export class SalesList implements OnInit {
  sales: SaleListItem[] = [];
  totalCount = 0;
  page = 1;
  pageSize = 10;
  totalPages = 0;
  loading = false;
  error = '';

  constructor(
    private saleService: SaleService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadSales();
  }

  loadSales(): void {
    this.loading = true;
    this.error = '';
    this.saleService.getSales(this.page, this.pageSize).subscribe({
      next: res => {
        this.sales = res.sales;
        this.totalCount = res.totalCount;
        this.totalPages = res.totalPages;
        this.loading = false;
      },
      error: () => {
        this.error = 'Erro ao carregar vendas.';
        this.loading = false;
      }
    });
  }

  goToDetail(id: string): void {
    this.router.navigate(['/sales', id]);
  }

  goToCreate(): void {
    this.router.navigate(['/sales/new']);
  }

  prevPage(): void {
    if (this.page > 1) { this.page--; this.loadSales(); }
  }

  nextPage(): void {
    if (this.page < this.totalPages) { this.page++; this.loadSales(); }
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  get email(): string {
    return this.authService.getEmail() ?? '';
  }
}
