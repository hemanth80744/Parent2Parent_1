import { Component, computed, inject, signal } from '@angular/core';
import { SidebarComponent } from '../../components/sidebar/sidebar.component';
import { EmptyStateComponent } from '../../components/ui/empty-state/empty-state.component';
import { AuthService } from '../../services/auth.service';
import { RequestsService } from '../../services/requests.service';
import { ConnectionRequest } from '../../services/models';

@Component({
  selector: 'app-requests-page',
  standalone: true,
  imports: [SidebarComponent, EmptyStateComponent],
  templateUrl: './requests.page.html',
  styleUrl: './requests.page.css'
})
export class RequestsPage {
  private readonly auth = inject(AuthService);
  private readonly requests = inject(RequestsService);

  readonly meId = computed(() => this.auth.user()?.id ?? 999);
  readonly allRequests = signal<ConnectionRequest[]>([]);
  readonly incoming = computed(() => this.allRequests().filter(r => r.receiverId === this.meId()));
  readonly outgoing = computed(() => this.allRequests().filter(r => r.senderId === this.meId()));

  constructor() {
    this.refresh();
  }

  async refresh() {
    const me = this.meId();
    console.log('--- REFRESHING REQUESTS ---');
    console.log('Current user ID (meId):', me);
    
    try {
      const raw = await this.requests.listAllRequests(me);
      console.log('Requests loaded from API:', raw);
      
      // Detailed inspection of the first item if it exists
      if (raw.length > 0) {
        console.log('Detailed check of first item:', {
          original: raw[0],
          senderId: raw[0].senderId,
          receiverId: raw[0].receiverId,
          status: raw[0].status
        });
      }

      this.allRequests.set(raw);
    } catch (error) {
      console.error('Failed to load requests:', error);
    }
  }

  accept(id: number) {
    this.requests.accept(id)
      .then(() => this.refresh())
      .catch(e => {
        console.error('Failed to accept request', e);
        alert(e?.error?.message ?? 'Failed to accept request.');
      });
  }

  reject(id: number) {
    this.requests.reject(id)
      .then(() => this.refresh())
      .catch(e => {
        console.error('Failed to reject request', e);
        alert(e?.error?.message ?? 'Failed to reject request.');
      });
  }
}

