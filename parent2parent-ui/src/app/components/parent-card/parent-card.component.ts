import { Component, input, output } from '@angular/core';
import { ParentProfile } from '../../services/models';

@Component({
  selector: 'app-parent-card',
  standalone: true,
  templateUrl: './parent-card.component.html',
  styleUrl: './parent-card.component.css'
})
export class ParentCardComponent {
  readonly parent = input.required<ParentProfile>();
  readonly requestState = input<'ready' | 'sent'>('ready');
  readonly request = output<ParentProfile>();

  send() {
    this.request.emit(this.parent());
  }
}

