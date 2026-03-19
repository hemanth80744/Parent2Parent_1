import { Component, input } from '@angular/core';

@Component({
  selector: 'app-empty-state',
  standalone: true,
  templateUrl: './empty-state.component.html',
  styleUrl: './empty-state.component.css'
})
export class EmptyStateComponent {
  readonly title = input('Nothing here yet');
  readonly subtitle = input('Try adjusting your search or come back later.');
  readonly icon = input('✨');
}

