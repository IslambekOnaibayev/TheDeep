import { inject } from '@angular/core';
import { CanMatchFn, Router } from '@angular/router';
import { SessionService } from './session.service';

export const requireSession: CanMatchFn = () => {
  const session = inject(SessionService);
  const router = inject(Router);
  return session.isSignedIn() ? true : router.createUrlTree(['/']);
};
