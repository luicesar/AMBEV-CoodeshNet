export interface SaleItem {
  id: string;
  productId: string;
  productName: string;
  quantity: number;
  unitPrice: number;
  discount: number;
  totalAmount: number;
  isCancelled: boolean;
}

export interface Sale {
  id: string;
  saleNumber: string;
  saleDate: string;
  customerId: string;
  customerName: string;
  branchId: string;
  branchName: string;
  totalAmount: number;
  isCancelled: boolean;
  createdAt: string;
  updatedAt?: string;
  items: SaleItem[];
}

export interface SaleListItem {
  id: string;
  saleNumber: string;
  saleDate: string;
  customerName: string;
  branchName: string;
  totalAmount: number;
  isCancelled: boolean;
  itemCount: number;
}

export interface SaleListResponse {
  sales: SaleListItem[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface SaleItemForm {
  productId: string;
  productName: string;
  quantity: number;
  unitPrice: number;
  discount: number;
  totalAmount: number;
}

export interface CreateSaleRequest {
  saleNumber: string;
  saleDate: string;
  customerId: string;
  customerName: string;
  branchId: string;
  branchName: string;
  items: { productId: string; productName: string; quantity: number; unitPrice: number }[];
}

export interface UpdateSaleRequest {
  saleDate: string;
  customerId: string;
  customerName: string;
  branchId: string;
  branchName: string;
  items: { productId: string; productName: string; quantity: number; unitPrice: number }[];
}

export function getDiscount(quantity: number): number {
  if (quantity > 20) return -1;
  if (quantity >= 10) return 0.20;
  if (quantity >= 4) return 0.10;
  return 0;
}

export function getDiscountLabel(quantity: number): string {
  if (quantity > 20) return 'Máx. 20 itens';
  if (quantity >= 10) return '20% de desconto';
  if (quantity >= 4) return '10% de desconto';
  return 'Sem desconto';
}
