import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SidebarComponent } from '../../components/sidebar/sidebar.component';
import { ParentCardComponent } from '../../components/parent-card/parent-card.component';
import { EmptyStateComponent } from '../../components/ui/empty-state/empty-state.component';
import { AuthService } from '../../services/auth.service';
import { ParentsService } from '../../services/parents.service';
import { RequestsService } from '../../services/requests.service';
import { ParentProfile } from '../../services/models';

@Component({
  selector: 'app-dashboard-page',
  standalone: true,
  imports: [FormsModule, SidebarComponent, ParentCardComponent, EmptyStateComponent],
  templateUrl: './dashboard.page.html',
  styleUrl: './dashboard.page.css'
})
export class DashboardPage {
  private readonly parents = inject(ParentsService);
  private readonly requests = inject(RequestsService);
  private readonly auth = inject(AuthService);

  readonly query = signal('');
  readonly searching = signal(false);
  readonly results = signal<ParentProfile[]>([]);
  readonly searchedOnce = signal(false);
  readonly error = signal<string | null>(null);
  readonly sentTo = signal<Set<number>>(new Set());

  async search() {
    this.error.set(null);
    this.searching.set(true);
    try {
      const q = this.query().trim();
      this.searchedOnce.set(true);
      if (!q) {
        this.results.set([]);
        return;
      }

      const [list, sentReqs] = await Promise.all([
        this.parents.searchBySchoolName(q),
        this.requests.listAllRequests(this.auth.user()?.id ?? 0)
      ]);

      this.results.set(list);
      
      // Update sentTo based on real data from the server
      const sentIds = new Set(sentReqs.filter(r => r.senderId === this.auth.user()?.id).map(r => r.receiverId));
      this.sentTo.set(sentIds);
    } catch (e: any) {
      this.error.set(e?.message ?? 'Search failed.');
      this.results.set([]);
    } finally {
      this.searching.set(false);
    }
  }

  async sendRequest(p: ParentProfile) {
    if (this.sentTo().has(p.id)) return;
    const me = this.auth.user();
    const senderId = me?.id ?? 0;
    await this.requests.sendRequest(senderId, p.id);
    const next = new Set(this.sentTo());
    next.add(p.id);
    this.sentTo.set(next);
  }
}

