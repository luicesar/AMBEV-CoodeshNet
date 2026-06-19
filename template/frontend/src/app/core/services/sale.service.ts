import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import {
  Sale, SaleListResponse, CreateSaleRequest, UpdateSaleRequest
} from '../models/sale.model';

interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
}

@Injectable({ providedIn: 'root' })
export class SaleService {
  private readonly apiUrl = `${environment.apiUrl}/sales`;

  constructor(private http: HttpClient) {}

  getSales(page = 1, pageSize = 10): Observable<SaleListResponse> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    return this.http.get<ApiResponse<SaleListResponse>>(this.apiUrl, { params })
      .pipe(map(r => r.data));
  }

  getSale(id: string): Observable<Sale> {
    return this.http.get<ApiResponse<Sale>>(`${this.apiUrl}/${id}`)
      .pipe(map(r => r.data));
  }

  createSale(request: CreateSaleRequest): Observable<Sale> {
    return this.http.post<ApiResponse<Sale>>(this.apiUrl, request)
      .pipe(map(r => r.data));
  }

  updateSale(id: string, request: UpdateSaleRequest): Observable<Sale> {
    return this.http.put<ApiResponse<Sale>>(`${this.apiUrl}/${id}`, request)
      .pipe(map(r => r.data));
  }

  deleteSale(id: string): Observable<ApiResponse<any>> {
    return this.http.delete<ApiResponse<any>>(`${this.apiUrl}/${id}`);
  }

  cancelSale(id: string): Observable<{ id: string; isCancelled: boolean }> {
    return this.http.patch<ApiResponse<{ id: string; isCancelled: boolean }>>(
      `${this.apiUrl}/${id}/cancel`, {}
    ).pipe(map(r => r.data));
  }

  cancelSaleItem(saleId: string, itemId: string): Observable<any> {
    return this.http.patch<ApiResponse<any>>(
      `${this.apiUrl}/${saleId}/items/${itemId}/cancel`, {}
    ).pipe(map(r => r.data));
  }
}
