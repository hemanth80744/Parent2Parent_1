import { Injectable, signal } from '@angular/core';
import { UsersApiService } from './users-api.service';

export type AuthUser = {
  id: number;
  name: string;
  username: string;
};

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly storageKey = 'p2p_user';

  readonly user = signal<AuthUser | null>(this.readUser());

  constructor(private readonly api: UsersApiService) {}

  async register(payload: { name: string; username: string; password: string; school: string; class: string }) {
    const res = await this.api.register(payload);
    if (!res?.success) throw new Error(res?.message ?? 'Registration failed');
    // Registration SP doesn't return a user object; send them to login.
  }

  async login(payload: { username: string; password: string }): Promise<AuthUser> {
    const res = await this.api.login(payload);
    if (!res?.success || !res.data) throw new Error(res?.message ?? 'Invalid username or password');
    const u: AuthUser = { id: res.data.userId, name: res.data.name, username: payload.username };
    localStorage.setItem(this.storageKey, JSON.stringify(u));
    this.user.set(u);
    return u;
  }

  logout() {
    localStorage.removeItem(this.storageKey);
    this.user.set(null);
  }

  private readUser(): AuthUser | null {
    try {
      const raw = localStorage.getItem(this.storageKey);
      if (!raw) return null;
      const saved = JSON.parse(raw) as AuthUser;
      return { id: saved.id, name: saved.name, username: saved.username };
    } catch {
      return null;
    }
  }
}

