import {
  ApplicationConfig,
  provideBrowserGlobalErrorListeners,
  provideZonelessChangeDetection,
} from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { provideHttpClient, withFetch } from '@angular/common/http';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideTransloco } from '@jsverse/transloco';

import { routes } from './app.routes';
import {
  AVAILABLE_LANGS,
  DEFAULT_LANG,
  LANG_STORAGE_KEY,
  TranslocoHttpLoader,
} from './core/transloco-loader';

const storedLang =
  (typeof localStorage !== 'undefined' && localStorage.getItem(LANG_STORAGE_KEY)) || DEFAULT_LANG;

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZonelessChangeDetection(),
    provideRouter(routes, withComponentInputBinding()),
    provideHttpClient(withFetch()),
    provideAnimationsAsync(),
    provideTransloco({
      config: {
        availableLangs: AVAILABLE_LANGS.map((l) => l.id),
        defaultLang: storedLang,
        fallbackLang: DEFAULT_LANG,
        reRenderOnLangChange: true,
        prodMode: false,
      },
      loader: TranslocoHttpLoader,
    }),
  ],
};
