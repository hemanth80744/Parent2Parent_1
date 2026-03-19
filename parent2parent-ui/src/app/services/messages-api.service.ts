import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { API_BASE_URL } from './api.config';
import { ApiResponse } from './api.types';

export type SendMessageDto = {
  senderId: number;
  receiverId: number;
  message: string;
};

export type MessageDto = {
  senderId: number;
  receiverId: number;
  message: string;
  sentAt: string;
};

@Injectable({ providedIn: 'root' })
export class MessagesApiService {
  constructor(private readonly http: HttpClient) {}

  async send(dto: SendMessageDto): Promise<void> {
    await firstValueFrom(
      this.http.post<ApiResponse<unknown>>(`${API_BASE_URL}/api/messages/send`, dto)
    );
  }

  async getChat(user1: number, user2: number): Promise<MessageDto[]> {
    const res = await firstValueFrom(
      this.http.get<ApiResponse<MessageDto[]>>(`${API_BASE_URL}/api/messages/chat`, {
        params: { user1, user2 }
      })
    );
    return res.data ?? [];
  }
}

