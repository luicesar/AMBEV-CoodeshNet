import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { SaleService } from '../../../core/services/sale.service';
import { Sale } from '../../../core/models/sale.model';

@Component({
  selector: 'app-sale-detail',
  templateUrl: './sale-detail.html',
  standalone: false,
  styleUrl: './sale-detail.scss'
})
export class SaleDetail implements OnInit {
  sale: Sale | null = null;
  loading = false;
  error = '';
  actionLoading = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private saleService: SaleService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.loadSale(id);
  }

  loadSale(id: string): void {
    this.loading = true;
    this.saleService.getSale(id).subscribe({
      next: sale => { this.sale = sale; this.loading = false; },
      error: () => { this.error = 'Venda não encontrada.'; this.loading = false; }
    });
  }

  goToEdit(): void {
    this.router.navigate(['/sales', this.sale!.id, 'edit']);
  }

  goBack(): void {
    this.router.navigate(['/sales']);
  }

  cancelSale(): void {
    if (!confirm('Confirmar cancelamento da venda?')) return;
    this.actionLoading = true;
    this.saleService.cancelSale(this.sale!.id).subscribe({
      next: () => this.loadSale(this.sale!.id),
      error: () => { this.error = 'Erro ao cancelar venda.'; this.actionLoading = false; }
    });
  }

  cancelItem(itemId: string): void {
    if (!confirm('Cancelar este item?')) return;
    this.actionLoading = true;
    this.saleService.cancelSaleItem(this.sale!.id, itemId).subscribe({
      next: () => this.loadSale(this.sale!.id),
      error: () => { this.error = 'Erro ao cancelar item.'; this.actionLoading = false; }
    });
  }

  deleteSale(): void {
    if (!confirm('Excluir esta venda permanentemente?')) return;
    this.saleService.deleteSale(this.sale!.id).subscribe({
      next: () => this.router.navigate(['/sales']),
      error: () => this.error = 'Erro ao excluir venda.'
    });
  }

  discountLabel(discount: number): string {
    if (discount === 0.20) return '20%';
    if (discount === 0.10) return '10%';
    return '-';
  }
}
