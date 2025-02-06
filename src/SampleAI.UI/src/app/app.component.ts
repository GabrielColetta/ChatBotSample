import { Component } from '@angular/core';
import { IconDefinition, faGithub } from '@fortawesome/free-brands-svg-icons';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.component.css'
})
export class AppComponent {
  public gitHubIcon: IconDefinition;

  constructor() {
    this.gitHubIcon = faGithub;
  }
}
