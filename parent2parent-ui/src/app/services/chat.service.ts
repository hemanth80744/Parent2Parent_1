import { Injectable, computed, signal } from '@angular/core';
import { ChatMessage, ParentProfile } from './models';
import { MessagesApiService } from './messages-api.service';
import { RequestsApiService } from './requests-api.service';

@Injectable({ providedIn: 'root' })
export class ChatService {
  readonly selectedUserId = signal<number | null>(null);
  readonly sending = signal(false);
  readonly loadingMessages = signal(false);
  readonly connections = signal<ParentProfile[]>([]);

  constructor(
    private readonly api: MessagesApiService,
    private readonly requestsApi: RequestsApiService
  ) {}

  async connectedUsersFor(userId: number): Promise<ParentProfile[]> {
    // Uses all accepted requests (sent or received) returned by sp_view_requests.
    const reqs = await this.requestsApi.listIncoming(userId);
    const list = reqs
      .filter((r) => r.status.toLowerCase() === 'accepted')
      .map((r) => {
        const isMeSender = r.senderId === userId;
        const otherId = isMeSender ? r.receiverId : r.senderId;
        const otherName = isMeSender 
          ? (r.receiverName || `Parent (${r.receiverId})`) 
          : (r.senderName || `Parent (${r.senderId})`);
        
        return {
          id: otherId,
          name: otherName,
          childClass: '',
          currentSchoolName: ''
        };
      });

    this.connections.set(list);
    return list;
  }

  selectedUser = computed(() => {
    const id = this.selectedUserId();
    if (!id) return null;
    return this.connections().find((p) => p.id === id) ?? null;
  });

  async messagesFor(meId: number, otherId: number): Promise<ChatMessage[]> {
    this.loadingMessages.set(true);
    try {
      const raw = await this.api.getChat(meId, otherId);
      return raw
        .map<ChatMessage>((m) => ({
          id: `${m.senderId}-${m.receiverId}-${m.sentAt}-${Math.random()}`,
          senderId: m.senderId,
          receiverId: m.receiverId,
          message: m.message,
          sentAt: new Date(m.sentAt)
        }))
        .sort((a, b) => a.sentAt.getTime() - b.sentAt.getTime());
    } finally {
      this.loadingMessages.set(false);
    }
  }

  async sendMessage(meId: number, otherId: number, text: string) {
    const trimmed = text.trim();
    if (!trimmed) return;
    this.sending.set(true);
    try {
      await this.api.send({ senderId: meId, receiverId: otherId, message: trimmed });
    } finally {
      this.sending.set(false);
    }
  }
}

