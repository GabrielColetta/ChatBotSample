import { NgModule, provideZoneChangeDetection } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';;
import { FormsModule, ReactiveFormsModule } from '@angular/forms'
import { ToolbarModule } from 'primeng/toolbar';
import { TableModule } from 'primeng/table';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { FieldsetModule } from 'primeng/fieldset';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { ButtonModule } from 'primeng/button';
import { routes } from './app-routing.module';
import Aura from '@primeng/themes/aura';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ChatComponent } from './chat/chat.component';
import { HistoryComponent } from './history/history.component';
import { provideHttpClient } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { providePrimeNG } from 'primeng/config';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { PanelModule } from 'primeng/panel';
import { InputTextModule } from 'primeng/inputtext';
import { TextareaModule } from 'primeng/textarea';
import { CardModule } from 'primeng/card';

@NgModule({
  declarations: [
    AppComponent,
    ChatComponent,
    HistoryComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    ToolbarModule,
    TableModule,
    BreadcrumbModule,
    FieldsetModule,
    IconFieldModule,
    InputIconModule,
    ButtonModule,
    PanelModule,
    InputTextModule,
    TextareaModule,
    CardModule,
    ReactiveFormsModule
  ],
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideAnimationsAsync(),
    providePrimeNG({
      theme: {
        preset: Aura,
        options: {
          prefix: 'p',
          darkModeSelector: 'system',
          cssLayer: false
        }
      }
    }),
    provideHttpClient()
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
