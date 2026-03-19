import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register-page',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './register.page.html',
  styleUrl: './register.page.css'
})
export class RegisterPage {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  readonly submitting = signal(false);
  readonly error = signal<string | null>(null);

  readonly form = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
    username: ['', [Validators.required, Validators.maxLength(50)]],
    password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(100)]],
    school: ['', [Validators.required, Validators.maxLength(150)]],
    class: ['', [Validators.required, Validators.maxLength(50)]]
  });

  async submit() {
    this.error.set(null);
    this.form.markAllAsTouched();
    if (this.form.invalid) return;

    this.submitting.set(true);
    try {
      await this.auth.register(this.form.getRawValue() as any);
      // Next step: login
      await this.router.navigateByUrl('/login');
    } catch (e: any) {
      // Show a short, user-friendly message.
      const msg =
        e?.error?.message ??
        e?.message ??
        'Unable to register right now. Please try again.';
      this.error.set(msg);
    } finally {
      this.submitting.set(false);
    }
  }
}

