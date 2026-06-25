import { inject, Injectable } from '@angular/core';
import { TranslocoService } from '@jsverse/transloco';
import { AVAILABLE_LANGS, LANG_STORAGE_KEY } from './transloco-loader';

@Injectable({ providedIn: 'root' })
export class LanguageService {
  private readonly transloco = inject(TranslocoService);
  readonly languages = AVAILABLE_LANGS;

  get active(): string {
    return this.transloco.getActiveLang();
  }

  use(lang: string) {
    this.transloco.setActiveLang(lang);
    localStorage.setItem(LANG_STORAGE_KEY, lang);
  }
}
