import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.html',
  standalone: false,
  styleUrl: './login.scss'
})
export class Login {
  email = '';
  password = '';
  error = '';
  loading = false;

  constructor(private authService: AuthService, private router: Router) {}

  onSubmit(): void {
    if (!this.email || !this.password) {
      this.error = 'Preencha email e senha.';
      return;
    }
    this.loading = true;
    this.error = '';
    this.authService.login({ email: this.email, password: this.password }).subscribe({
      next: () => this.router.navigate(['/sales']),
      error: () => {
        this.error = 'Credenciais inválidas.';
        this.loading = false;
      }
    });
  }
}
