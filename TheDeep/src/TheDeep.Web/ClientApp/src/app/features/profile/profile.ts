import { ChangeDetectionStrategy, Component, computed, inject, OnInit } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { TranslocoModule } from '@jsverse/transloco';

import { SessionService } from '../../core/session.service';

@Component({
  selector: 'app-profile',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [MatCardModule, MatIconModule, TranslocoModule],
  templateUrl: './profile.html',
  styleUrl: './profile.scss',
})
export class Profile implements OnInit {
  protected readonly session = inject(SessionService);

  readonly winRate = computed(() => {
    const p = this.session.player();
    return p ? `${Math.round(p.winRate * 100)}%` : '0%';
  });

  ngOnInit() {
    this.session.refreshStats();
  }
}
