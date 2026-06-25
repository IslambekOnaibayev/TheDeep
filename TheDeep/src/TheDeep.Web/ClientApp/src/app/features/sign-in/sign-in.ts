import { ChangeDetectionStrategy, Component, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { TranslocoModule } from '@jsverse/transloco';

import { SessionService } from '../../core/session.service';

@Component({
  selector: 'app-sign-in',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    TranslocoModule,
  ],
  templateUrl: './sign-in.html',
  styleUrl: './sign-in.scss',
})
export class SignIn implements OnInit {
  private readonly session = inject(SessionService);
  private readonly router = inject(Router);

  readonly name = signal('');
  readonly submitting = signal(false);

  ngOnInit() {
    if (this.session.isSignedIn()) this.router.navigate(['/lobby']);
  }

  start() {
    const name = this.name().trim();
    if (!name || this.submitting()) return;

    this.submitting.set(true);
    this.session.signIn(name).subscribe({
      next: () => this.router.navigate(['/lobby']),
      error: () => this.submitting.set(false),
    });
  }
}
