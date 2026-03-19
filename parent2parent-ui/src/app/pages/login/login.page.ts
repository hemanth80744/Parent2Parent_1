import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './login.page.html',
  styleUrl: './login.page.css'
})
export class LoginPage {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  readonly submitting = signal(false);
  readonly authError = signal<string | null>(null);

  readonly form = this.fb.group({
    username: ['', [Validators.required, Validators.maxLength(50)]],
    password: ['', [Validators.required, Validators.maxLength(100)]]
  });

  async submit() {
    this.authError.set(null);
    this.form.markAllAsTouched();
    if (this.form.invalid) return;

    this.submitting.set(true);
    try {
      await this.auth.login(this.form.getRawValue() as any);
      await this.router.navigateByUrl('/dashboard');
    } catch (e: any) {
      this.authError.set(e?.message ?? 'Invalid username or password.');
    } finally {
      this.submitting.set(false);
    }
  }
}

