import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { SaleService } from '../../../core/services/sale.service';
import { SaleItemForm, getDiscount, getDiscountLabel } from '../../../core/models/sale.model';
import { v4 as uuidv4 } from 'uuid';

@Component({
  selector: 'app-sale-form',
  templateUrl: './sale-form.html',
  standalone: false,
  styleUrl: './sale-form.scss'
})
export class SaleForm implements OnInit {
  isEdit = false;
  saleId = '';
  loading = false;
  saving = false;
  error = '';

  saleNumber = '';
  saleDate = new Date().toISOString().split('T')[0];
  customerId = '';
  customerName = '';
  branchId = '';
  branchName = '';

  items: SaleItemForm[] = [];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private saleService: SaleService
  ) {}

  ngOnInit(): void {
    this.saleId = this.route.snapshot.paramMap.get('id') ?? '';
    this.isEdit = !!this.saleId;
    if (this.isEdit) {
      this.loadSale();
    } else {
      this.addItem();
    }
  }

  loadSale(): void {
    this.loading = true;
    this.saleService.getSale(this.saleId).subscribe({
      next: sale => {
        this.saleNumber = sale.saleNumber;
        this.saleDate = sale.saleDate.split('T')[0];
        this.customerId = sale.customerId;
        this.customerName = sale.customerName;
        this.branchId = sale.branchId;
        this.branchName = sale.branchName;
        this.items = sale.items
          .filter(i => !i.isCancelled)
          .map(i => ({
            productId: i.productId,
            productName: i.productName,
            quantity: i.quantity,
            unitPrice: i.unitPrice,
            discount: i.discount,
            totalAmount: i.totalAmount
          }));
        this.loading = false;
      },
      error: () => { this.error = 'Erro ao carregar venda.'; this.loading = false; }
    });
  }

  addItem(): void {
    this.items.push({ productId: uuidv4(), productName: '', quantity: 1, unitPrice: 0, discount: 0, totalAmount: 0 });
  }

  removeItem(index: number): void {
    this.items.splice(index, 1);
  }

  onQuantityChange(item: SaleItemForm): void {
    item.discount = getDiscount(item.quantity);
    this.recalculateItem(item);
  }

  onPriceChange(item: SaleItemForm): void {
    this.recalculateItem(item);
  }

  private recalculateItem(item: SaleItemForm): void {
    if (item.discount < 0) { item.totalAmount = 0; return; }
    item.totalAmount = item.quantity * item.unitPrice * (1 - item.discount);
  }

  getDiscountLabel(qty: number): string {
    return getDiscountLabel(qty);
  }

  isItemInvalid(item: SaleItemForm): boolean {
    return item.quantity > 20 || item.quantity < 1;
  }

  get total(): number {
    return this.items.reduce((s, i) => s + (i.totalAmount || 0), 0);
  }

  get formValid(): boolean {
    return !!this.saleNumber && !!this.customerName && !!this.branchName
      && this.items.length > 0
      && this.items.every(i => i.productName && i.quantity >= 1 && i.quantity <= 20 && i.unitPrice > 0);
  }

  save(): void {
    if (!this.formValid) { this.error = 'Preencha todos os campos corretamente.'; return; }
    this.saving = true;
    this.error = '';

    const itemsPayload = this.items.map(i => ({
      productId: i.productId,
      productName: i.productName,
      quantity: i.quantity,
      unitPrice: i.unitPrice
    }));

    if (this.isEdit) {
      this.saleService.updateSale(this.saleId, {
        saleDate: new Date(this.saleDate).toISOString(),
        customerId: this.customerId,
        customerName: this.customerName,
        branchId: this.branchId,
        branchName: this.branchName,
        items: itemsPayload
      }).subscribe({
        next: () => this.router.navigate(['/sales', this.saleId]),
        error: err => { this.error = err?.error?.message ?? 'Erro ao atualizar.'; this.saving = false; }
      });
    } else {
      this.saleService.createSale({
        saleNumber: this.saleNumber,
        saleDate: new Date(this.saleDate).toISOString(),
        customerId: this.customerId || uuidv4(),
        customerName: this.customerName,
        branchId: this.branchId || uuidv4(),
        branchName: this.branchName,
        items: itemsPayload
      }).subscribe({
        next: sale => this.router.navigate(['/sales', sale.id]),
        error: err => { this.error = err?.error?.message ?? 'Erro ao criar venda.'; this.saving = false; }
      });
    }
  }

  cancel(): void {
    this.router.navigate(this.isEdit ? ['/sales', this.saleId] : ['/sales']);
  }
}
