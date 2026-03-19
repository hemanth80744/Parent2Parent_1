import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { API_BASE_URL } from './api.config';
import { ApiResponse } from './api.types';

export type SendRequestDto = {
  senderId: number;
  receiverId: number;
};

export type ConnectionRequestDto = {
  requestId: number;
  senderId: number;
  senderName: string;
  receiverId: number;
  receiverName?: string;
  status: string;
  createdAt?: string;
};

@Injectable({ providedIn: 'root' })
export class RequestsApiService {
  constructor(private readonly http: HttpClient) {}

  async sendRequest(dto: SendRequestDto): Promise<void> {
    await firstValueFrom(
      this.http.post<ApiResponse<unknown>>(`${API_BASE_URL}/api/requests/send`, dto)
    );
  }

  async listIncoming(userId: number): Promise<ConnectionRequestDto[]> {
    const res = await firstValueFrom(
      this.http.get<ApiResponse<ConnectionRequestDto[]>>(`${API_BASE_URL}/api/requests/${userId}`)
    );
    return res.data ?? [];
  }

  async listSent(userId: number): Promise<ConnectionRequestDto[]> {
    const res = await firstValueFrom(
      this.http.get<ApiResponse<ConnectionRequestDto[]>>(`${API_BASE_URL}/api/requests/sent/${userId}`)
    );
    return res.data ?? [];
  }

  async accept(requestId: number): Promise<void> {
    await firstValueFrom(
      this.http.put<ApiResponse<unknown>>(`${API_BASE_URL}/api/requests/accept/${requestId}`, {})
    );
  }

  async reject(requestId: number): Promise<void> {
    await firstValueFrom(
      this.http.put<ApiResponse<unknown>>(`${API_BASE_URL}/api/requests/reject/${requestId}`, {})
    );
  }
}

