import { Component, computed, inject } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent {
  private readonly auth = inject(AuthService);

  readonly user = this.auth.user;
  readonly isLoggedIn = computed(() => !!this.user());

  logout() {
    this.auth.logout();
  }
}

