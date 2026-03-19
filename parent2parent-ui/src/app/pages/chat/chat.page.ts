import { Component, DestroyRef, computed, inject, signal } from '@angular/core';
import { SidebarComponent } from '../../components/sidebar/sidebar.component';
import { EmptyStateComponent } from '../../components/ui/empty-state/empty-state.component';
import { ChatBoxComponent } from '../../components/chat/chat-box/chat-box.component';
import { ChatUsersComponent } from '../../components/chat/chat-users/chat-users.component';
import { AuthService } from '../../services/auth.service';
import { ChatService } from '../../services/chat.service';
import { interval } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ParentProfile, ChatMessage } from '../../services/models';

@Component({
  selector: 'app-chat-page',
  standalone: true,
  imports: [SidebarComponent, EmptyStateComponent, ChatBoxComponent, ChatUsersComponent],
  templateUrl: './chat.page.html',
  styleUrl: './chat.page.css'
})
export class ChatPage {
  private readonly auth = inject(AuthService);
  private readonly chat = inject(ChatService);
  private readonly destroyRef = inject(DestroyRef);

  readonly meId = computed(() => this.auth.user()?.id ?? 999);
  readonly users = signal<ParentProfile[]>([]);
  readonly selectedId = this.chat.selectedUserId;
  readonly selectedUser = this.chat.selectedUser;
  readonly messages = signal<ChatMessage[]>([]);
  readonly error = signal<string | null>(null);

  constructor() {
    this.loadUsers();
    interval(3000)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.loadMessages());
  }

  pick(id: number) {
    this.selectedId.set(id);
    this.loadMessages();
  }

  send(text: string) {
    const other = this.selectedId();
    if (!other) return;
    this.chat.sendMessage(this.meId(), other, text)
      .then(() => this.loadMessages())
      .catch(e => {
        console.error('Failed to send message', e);
        alert(e?.error?.message ?? 'Failed to send message.');
      });
  }

  private async loadUsers() {
    this.error.set(null);
    try {
      const list = await this.chat.connectedUsersFor(this.meId());
      this.users.set(list);
      if (!this.selectedId() && list.length > 0) {
        this.selectedId.set(list[0].id);
        await this.loadMessages();
      }
    } catch (e: any) {
      this.error.set(e?.message ?? 'Failed to load connections.');
      this.users.set([]);
    }
  }

  private async loadMessages() {
    const other = this.selectedId();
    if (!other) return;
    try {
      const msgs = await this.chat.messagesFor(this.meId(), other);
      this.messages.set(msgs);
    } catch (e: any) {
      console.error('Failed to load messages', e);
      // We don't set this.error signal here to avoid hiding the user list,
      // but you might want to show a small toast or message area error.
    }
  }
}

