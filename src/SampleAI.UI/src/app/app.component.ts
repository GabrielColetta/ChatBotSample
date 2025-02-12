import { Component } from '@angular/core';
import { IconDefinition, faGithub } from '@fortawesome/free-brands-svg-icons';
import { PrimeNG } from 'primeng/config';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.component.css'
})
export class AppComponent {
  public gitHubIcon: IconDefinition;

  constructor(private primeng: PrimeNG) {
    this.gitHubIcon = faGithub;
    this.primeng.ripple.set(true);
  }
}
