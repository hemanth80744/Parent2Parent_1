import { Injectable, signal } from '@angular/core';
import { ConnectionRequest } from './models';
import { RequestsApiService, ConnectionRequestDto } from './requests-api.service';

@Injectable({ providedIn: 'root' })
export class RequestsService {
  readonly loading = signal(false);

  constructor(
    private readonly api: RequestsApiService
  ) {}

  async listAllRequests(userId: number): Promise<ConnectionRequest[]> {
    this.loading.set(true);
    try {
      const all: any[] = await this.api.listIncoming(userId);
      return all.map((r) => ({
        requestId: r.requestId ?? r.RequestId ?? 0,
        senderId: r.senderId ?? r.SenderId ?? r.fromUserId ?? r.FromUserId ?? 0,
        senderName: r.senderName ?? r.SenderName ?? r.sender_name ?? r.Sender_Name ?? 'User',
        receiverId: r.receiverId ?? r.ReceiverId ?? r.toUserId ?? r.ToUserId ?? 0,
        receiverName: r.receiverName ?? r.ReceiverName ?? r.receiver_name ?? r.Receiver_Name ?? '',
        status: r.status ?? r.Status ?? 'Pending',
        createdAt: r.createdAt ?? r.CreatedAt
      }));
    } finally {
      this.loading.set(false);
    }
  }

  async listIncomingRequests(receiverId: number): Promise<ConnectionRequest[]> {
    const all = await this.listAllRequests(receiverId);
    return all.filter(r => r.receiverId === receiverId);
  }

  async sendRequest(senderId: number, receiverId: number) {
    await this.api.sendRequest({ senderId, receiverId });
  }

  async accept(requestId: number) {
    await this.api.accept(requestId);
  }

  async reject(requestId: number) {
    await this.api.reject(requestId);
  }
}

